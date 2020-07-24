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

local LogManager = CS.Loxodon.Log.LogManager
local Path = CS.System.IO.Path

local function toname(name)
	local s = name or "";
	s = string.gsub(s,"^@","")
	s = string.gsub(s,"%.lua","")
	s = string.gsub(s,"%.txt","")
	s = string.gsub(s,"%.bytes","")
	return Path.GetFileNameWithoutExtension(s)
end

local function log(self,fmt,...)	
	local content = fmt		
	if select('#', ...) > 0 then
		local status, msg = pcall(string.format, fmt, ...)
		if status then
			content = msg
		else
			content = "Error formatting log message: " .. msg
		end
	end
	
	local info = debug.getinfo(3,"l")
	return string.format("%s \nat (%s:%s)",content,self.fullname,info.currentline)
end

---
--模块
--@module Logger
local M = class("Logger")

function M.GetLogger(name)
	local info = debug.getinfo(2,"S")
	local fullname = info.source
	name = name or toname(info.source) or "Logger"
	
	local log = LogManager.GetLogger(name)	
	return M(log,fullname)
end

function M:ctor(log,fullname)
	self.log = log
	self.fullname = fullname
end

function M:debug(fmt,...)
	if self.log.IsDebugEnabled then
		self.log:Debug(log(self,fmt,...))
	end
end

function M:info(fmt,...)
	if self.log.IsInfoEnabled then
		self.log:Info(log(self,fmt,...))
	end
end

function M:warn(fmt,...)
	if self.log.IsWarnEnabled then
		self.log:Warn(log(self,fmt,...))
	end
end

function M:error(fmt,...)
	if self.log.IsErrorEnabled then
		self.log:Error(log(self,fmt,...))
	end
end

function M:fatal(fmt,...)
	if self.log.IsFatalEnabled then
		self.log:Fatal(log(self,fmt,...))
	end
end

return M