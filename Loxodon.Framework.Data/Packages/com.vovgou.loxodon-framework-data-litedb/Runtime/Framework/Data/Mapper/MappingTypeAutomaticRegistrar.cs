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

using LiteDB;
using System;
using UnityEngine;

namespace Loxodon.Framework.Data.Mapper
{
    public static class MappingTypeAutomaticRegistrar
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            BsonMapper mapper = BsonMapper.Global;
            mapper.RegisterType(value =>
            {
                if (value == null)
                    return BsonValue.Null;
                return value.ToString();
            }, value =>
             {
                 if (value.IsNull)
                     return default(Vector2);
                 return VectorUtility.ParseVector2(value.AsString);
             });

            mapper.RegisterType(value =>
            {
                if (value == null)
                    return BsonValue.Null;
                return value.ToString();
            }, value =>
            {
                if (value.IsNull)
                    return default(Vector3);
                return VectorUtility.ParseVector3(value.AsString);
            });

            mapper.RegisterType(value =>
            {
                if (value == null)
                    return BsonValue.Null;
                return value.ToString();
            }, value =>
            {
                if (value.IsNull)
                    return default(Vector4);
                return VectorUtility.ParseVector4(value.AsString);
            });

            mapper.RegisterType(value =>
            {
                if (value == null)
                    return BsonValue.Null;
                return value.ToString();
            }, value =>
            {
                if (value.IsNull)
                    return default(Vector2Int);
                return VectorUtility.ParseVector2Int(value.AsString);
            });

            mapper.RegisterType(value =>
            {
                if (value == null)
                    return BsonValue.Null;
                return value.ToString();
            }, value =>
            {
                if (value.IsNull)
                    return default(Vector3Int);
                return VectorUtility.ParseVector3Int(value.AsString);
            });

            mapper.RegisterType(value =>
            {
                if (value == null)
                    return BsonValue.Null;
                return value.ToString();
            }, value =>
            {
                if (value.IsNull)
                    return default(Color);
                Color color;
                ColorUtility.TryParseHtmlString(value.AsString, out color);
                return color;
            });

            mapper.RegisterType(value =>
            {
                if (value == null)
                    return BsonValue.Null;
                return value.ToString();
            }, value =>
            {
                if (value.IsNull)
                    return null;
                return Version.Parse(value.AsString);
            });
        }
    }
}