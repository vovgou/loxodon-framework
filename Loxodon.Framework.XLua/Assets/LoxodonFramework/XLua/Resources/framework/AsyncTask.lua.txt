---
-- MIT License
--
-- Copyright (c) 2018 Clark Yang
--
-- Permission is hereby granted, free of charge, to any person obtaining a copy of 
-- this software and associated documentation files (the "Software"), to deal in 
-- the Software without restriction, including without limitation the rights to 
-- use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
-- of the Software, and to permit persons to whom the Software is furnished to do so, 
-- subject to the following conditions:
--
-- The above copyright notice and this permission notice shall be included in all 
-- copies or substantial portions of the Software.
--
-- THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
-- IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
-- FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
-- AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
-- LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
-- OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
-- SOFTWARE.
---

require("framework.System")

local Executors = require("framework.Executors")
local Logger = require("framework.Logger")

local log = Logger.GetLogger("AsyncTask")

---
--LuaTaskAwaiter
--@module LuaTaskAwaiter
local TaskAwaiter = class("TaskAwaiter")

function TaskAwaiter:ctor()
	self.IsCompleted=false
	self.Packaged = false
	self.actions = {}
end

function TaskAwaiter:GetException()
	return self.exception
end

function TaskAwaiter:GetResult()
	if not self.IsCompleted then
		error("The task is not finished yet")
	end
	if self.exception then
		error(self.exception)
	end
	
	return self.result
end

function TaskAwaiter:SetResult(result,exception,packaged)	
	if exception then
		self.exception = exception
	else
		self.result = result
	end
	
	self.IsCompleted = true
	self.Packaged = packaged
	
	if not self.actions then
		return
	end
	
	for _, v in pairs(self.actions) do
		if v then
			xpcall(v,function(err)
					log:error("%s \n%s",err,debug.traceback())
				end)
		end
	end
end

function TaskAwaiter:OnCompleted(action)
	if not action then
		return
	end
	
	if self.IsCompleted then
		xpcall(action,function(err)
				log:error("%s \n%s",err,debug.traceback())
			end)
		return
	end
	
	table.insert(self.actions,action)
end

---
--AsyncTask
--@module AsyncTask
local M = class("AsyncTask",TaskAwaiter)

function async(action)
	return function(...)
		local task = M()
		if type(action)~='function' then
			task:SetResult(nil,"please enter a function")
			return task
		end
		
		local co = coroutine.create(function(...)
				local results = table.pack(xpcall(action,function(err)
							task:SetResult(nil,err,false)
							log:error("%s \n%s",err,debug.traceback())
						end,...))
				
				local status = results[1]				
				if status then
					table.remove(results,1)
					if #results <=1 then				
						task:SetResult(results[1],nil,false)
					else
						task:SetResult(results,nil,true)
					end
				end	
			end)
		coroutine.resume(co,...)
		return task
	end
end

function await(result)
	assert(result ~= nil,"The result is nil")
	
	local status, awaiter
	if type(result)=='table' and iskindof(result,"TaskAwaiter") then	
		awaiter = result
	elseif type(result) == 'userdata' or type(result) == 'table' then
		status, awaiter = pcall(result.GetAwaiter,result)
		if not status then
			error("The parameter of the await() is error,not found the GetAwaiter() in the "..tostring(result))
		end
	else
		error("The parameter of the await() is error, this is a function, please enter a table or userdata")
	end
	
	if awaiter.IsCompleted then
		local value = awaiter:GetResult()
		if type(value) == 'table' and awaiter.Packaged then
			return table.unpack(value)
		else
			return value
		end
	end
	
	local id = coroutine.running()
	local isYielded= false
	awaiter:OnCompleted(function()			
			if isYielded then
				coroutine.resume(id)
			end
		end)
	
	if not awaiter.IsCompleted then
		isYielded = true
		coroutine.yield()
	end
	
	local value = awaiter:GetResult()
	if type(value) == 'table' and awaiter.Packaged then
		return table.unpack(value)
	else
		return value
	end
end

function try(block)
	local main = block[1]
	local catch = block.catch
	local finally = block.finally
	
	local results = table.pack(pcall(main))
	local status = results[1]
	local e = results[2]
	table.remove(results,1)
	local result = results
	local catched = false
	if (not status) and catch and type(catch)=='function' then
		catched = true
		local results = table.pack(pcall(catch,e))
		if results[1] then
			table.remove(results,1)
			result = results
			e = nil
		else
			e = results[2]
		end		
	end
	
	if finally and 	type(finally)=='function' then
		pcall(finally)
	end	
	
	if status then
		return table.unpack(result)
	elseif catched then
		if not e then
			return table.unpack(result)
		else
			error(e)
		end
	else
		error(e)
	end
end

function M.Delay(millisecond)
	local action = async(function(millisecond)
			await(Executors.RunLuaOnCoroutine(function(delay)
						local wait = CS.UnityEngine.WaitForSecondsRealtime(delay)
						coroutine.yield(wait)					
					end,millisecond/1000.0))
		end)
	
	return action(millisecond)
end

function M.Run(func,...)
	local action = async(func)
	return action(...)
end

return M
