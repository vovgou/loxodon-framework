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

---
--Event模块
--@module Event
local M=class("Event")

M.__eq = function(e1, e2)
	if getmetatable(e1) ~= M or getmetatable(e2) ~= M then
		return false
	end
	
	if #e1.actions ~= #e2.actions then return false end
	
	for _,a1 in ipairs(e1.actions) do
		local found = false
		for _,a2 in ipairs(e2.actions) do
			if a1 == a2 then
				found = true
				break
			end
		end
		if found == false then return false end
	end
	return true
end

--[[--
构造函数
@param #table self
@param #table t 初始化参数
]]
function M:ctor()
	self.actions = {}
end

--[[--
订阅
@param #table self
@param #function action 监听函数
]]
function M:subscribe(action)
	if action then
		table.insert(self.actions,action)
	end
end

--[[--
退订
@param #table self
@param #function action 监听函数
]]
function M:unsubscribe(action)
	if action then
		for k,v in pairs(self.actions) do
			if action == v then
				table.remove(self.actions,k)
				return
			end
		end
	end
end

--[[--
抛出属性值改变事件
@param #table self
@param #table t 初始化参数
]]
function M:trigger(...)
	if self.actions then
		for _, v in pairs(self.actions) do
			v(...) 
		end
	end
end

return M
