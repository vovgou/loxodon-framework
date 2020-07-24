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

local util = require("xlua.util")
local InterceptableEnumerator = CS.Loxodon.Framework.Execution.InterceptableEnumerator
local Logger = require("framework.Logger")

local log = Logger.GetLogger("Executors")
---
--模块
--@module Executors
local M = class("Executors",CS.Loxodon.Framework.Execution.Executors)

---
--运行一个协程
--示例如下：
--function M.DoTask(n)
--	for i = 1, n do
--		coroutine.yield(nil)
--	end
--end
--
--Executors.RunLuaOnCoroutineNoReturn(function() self:DoTask(20) end)
--
--或者使用带参数的方式执行
--Executors.RunLuaOnCoroutineNoReturn(self.DoTask,self,20)
--
--@param #function func Lua函数
--@param #object[] ... Lua函数的参数
--
function M.RunLuaOnCoroutineNoReturn(func,...)
	local ie = InterceptableEnumerator(util.cs_generator(func,...))	
	ie:RegisterCatchBlock(function(e)			
			log:error("%s",e)
		end)
	M.RunOnCoroutineNoReturn(ie)
end

---
--运行一个协程
--示例如下：
--function M.DoTask(n)
--	for i = 1, n do
--		coroutine.yield(nil)
--	end
--end
--
--return Executors.RunLuaOnCoroutine(function() self:DoTask(20) end)
--
--或者使用带参数的方式执行
--return Executors.RunLuaOnCoroutine(self.DoTask,self,20)
--
--@param #function func Lua函数
--@param #object[] ... Lua函数的参数
--@return #userdata  返回一个C#对象IAsyncResult
--
function M.RunLuaOnCoroutine(func,...)
	return M.RunOnCoroutine(util.cs_generator(func,...))
end

return M