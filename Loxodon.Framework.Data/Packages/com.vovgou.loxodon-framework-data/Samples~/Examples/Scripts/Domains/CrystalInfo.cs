/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

#if NEWTONSOFT
using Newtonsoft.Json;
#endif
using System.Collections.Generic;
namespace Loxodon.Framework.Examples.Domains
{
    public class CrystalInfo
    {
        /// <summary>
        /// ID
        /// </summary>
#if NEWTONSOFT
        [JsonProperty("id")]
#endif
        public int Id { get; protected set; }

        /// <summary>
        /// 标识
        /// </summary>
#if NEWTONSOFT
        [JsonProperty("sign")]
#endif
        public string Sign { get; protected set; }

        /// <summary>
        /// 名称
        /// </summary>
#if NEWTONSOFT
        [JsonProperty("name")]
#endif
        public string Name { get; protected set; }

        /// <summary>
        /// 等级
        /// </summary>
#if NEWTONSOFT
        [JsonProperty("level")]
#endif
        public int Level { get; protected set; }

        /// <summary>
        /// 类型
        /// </summary>
#if NEWTONSOFT
        [JsonProperty("category")]
#endif
        public Category Category { get; protected set; }

        /// <summary>
        /// 品质
        /// </summary>
#if NEWTONSOFT
        [JsonProperty("quality")]
#endif
        public int Quality { get; protected set; }

        /// <summary>
        /// 水晶适配装备类型
        /// </summary>
#if NEWTONSOFT
        [JsonProperty("equipmentCategorys")]
#endif
        public List<int> EquipmentCategorys { get; protected set; }

        /// <summary>
        /// 属性名称
        /// </summary>
#if NEWTONSOFT
        [JsonProperty("key")]
#endif
        public string Key { get; protected set; }

        /// <summary>
        /// 属性数值
        /// </summary>
#if NEWTONSOFT
        [JsonProperty("value")]
#endif
        public int Value { get; protected set; }

        /// <summary>
        /// 合成所需水晶ID
        /// </summary>
#if NEWTONSOFT
        [JsonProperty("combineRequired")]
#endif
        public string CombineRequired { get; protected set; }

        /// <summary>
        /// 合成所需水晶数量
        /// </summary>
#if NEWTONSOFT
        [JsonProperty("combineRequiredCount")]
#endif
        public int CombineRequiredCount { get; protected set; }

        /// <summary>
        /// 镶嵌消耗金币
        /// </summary>
#if NEWTONSOFT
        [JsonProperty("inlayCostGold")]
#endif
        public int InlayCostGold { get; protected set; }

        /// <summary>
        /// 出售价格
        /// </summary>
#if NEWTONSOFT
        [JsonProperty("sale")]
#endif
        public int Sale { get; protected set; }
    }
}
