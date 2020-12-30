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
--模块
--@module ObservableObject
local M=class("ObservableObject")

--[[--
可观察对象的__index函数
@param #table t 表
@param #string k 属性名称
]]
local __index = function(t,k)
	if string.find(k, "^_") == 1 then
		return getmetatable(t)[k]
	else
		local v = rawget(t,"_attr_"..k)
		if v ~= nil then
			return v
		end 
		return getmetatable(t)[k]
	end	
end

--[[--
可观察对象的__newindex函数
@param #table t 表
@param #string k 属性名称
@param #object v 属性值
]]
local __newindex = function(t,k,v)
	if type(v)=="function" or string.find(k, "^_") == 1 then
		rawset(t,k,v)
	else
		rawset(t,"_attr_"..k,v)
		getmetatable(t).raisePropertyChanged(t,k)
	end
end

--[[--
初始化函数，设置一个可观察对象实例的原表
@param #table t 表
]]
local function init(t)
	t.__actions = {}
	local orginal_meta = getmetatable(t)
	local meta = setmetatable({__index = __index , __newindex = __newindex}, orginal_meta)
	setmetatable(t, meta)
end

--[[--
构造函数
@param #table self
@param #table t 初始化参数
]]
function M:ctor(t)
	init(self)
	
	if t and type(t)=="table" then
		for k,v in pairs(t) do self[k] = v end
	end
end

--[[--
订阅
@param #table self
@param #string key 属性名称
@param #function action 订阅的委托函数
]]
function M:subscribe(key,action)
	if not self.__actions then
		self.__actions = {}
	end
	
	if key and action then
		table.insert(self.__actions,{key= key,action = action})
	end
end

--[[--
退订
@param #table self
@param #string key 属性名称
@param #function action 退订的委托函数
]]
function M:unsubscribe(key,action)
	if not self.__actions then
		return
	end
	
	if key and action then
		for k,v in pairs(self.__actions) do
			if key == v.key and action == v.action then
				table.remove(self.__actions,k)
				return
			end
		end
	end
end

--[[--
触发属性改变的通知
@param #table self
@param #string key 属性名称
]]
function M:raisePropertyChanged(key)
	if not self.__actions then
		return
	end
	
	if self.__actions then
		for _, v in pairs(self.__actions) do
			if key == nil or v.key == key then			
				v.action()
			end
		end
	end
end

--[[--
实现IViewModel.Dispose，这样可以将ObservableObject在C#边转为IViewModel类型
]]
function M:Dispose()
	
end

return M
