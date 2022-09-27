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

using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Loxodon.Framework.Data
{
    public static class VectorUtility
    {
        private static readonly char[] COMMA_SEPARATOR = new char[] { ',' };
        private static readonly string PATTERN = @"(^\()|(\)$)";

        public static Vector2 ParseVector2(string value)
        {
            string val = Regex.Replace(value.Trim(), PATTERN, "");
            string[] s = val.Split(COMMA_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
            if (s.Length == 2)
                return new Vector2(float.Parse(s[0]), float.Parse(s[1]));

            throw new FormatException(string.Format("This value \"{0}\" cannot be converted to the type \"Vector2\"", value));
        }

        public static Vector3 ParseVector3(string value)
        {
            string val = Regex.Replace(value.Trim(), PATTERN, "");
            string[] s = val.Split(COMMA_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
            if (s.Length == 3)
                return new Vector3(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]));

            throw new FormatException(string.Format("This value \"{0}\" cannot be converted to the type \"Vector3\"", value));
        }

        public static Vector4 ParseVector4(string value)
        {
            string val = Regex.Replace(value.Trim(), PATTERN, "");
            string[] s = val.Split(COMMA_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
            if (s.Length == 4)
                return new Vector4(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]), float.Parse(s[3]));

            throw new FormatException(string.Format("This value \"{0}\" cannot be converted to the type \"Vector4\"", value));
        }

        public static Vector2Int ParseVector2Int(string value)
        {
            string val = Regex.Replace(value.Trim(), PATTERN, "");
            string[] s = val.Split(COMMA_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
            if (s.Length == 2)
                return new Vector2Int(int.Parse(s[0]), int.Parse(s[1]));

            throw new FormatException(string.Format("This value \"{0}\" cannot be converted to the type \"Vector2Int\"", value));
        }

        public static Vector3Int ParseVector3Int(string value)
        {
            string val = Regex.Replace(value.Trim(), PATTERN, "");
            string[] s = val.Split(COMMA_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
            if (s.Length == 3)
                return new Vector3Int(int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]));

            throw new FormatException(string.Format("This value \"{0}\" cannot be converted to the type \"Vector3Int\"", value));
        }
    }
}
