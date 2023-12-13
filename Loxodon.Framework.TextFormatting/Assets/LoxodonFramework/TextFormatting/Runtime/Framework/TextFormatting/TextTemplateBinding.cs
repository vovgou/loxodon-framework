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

using Loxodon.Framework.Binding.Paths;
using Loxodon.Framework.Binding.Proxy;
using Loxodon.Framework.Binding.Proxy.Sources;
using Loxodon.Framework.Binding.Proxy.Sources.Object;
using Loxodon.Framework.Binding.Reflection;
using Loxodon.Framework.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Loxodon.Framework.TextFormatting.IFormatter;

namespace Loxodon.Framework.TextFormatting
{
    public class TextTemplateBinding : IDisposable
    {
        private const int MAX_STACK_LIMIT = 1024;
        [ThreadStatic]
        private static StringBuilder BUFFER = new StringBuilder();
        private string m_Template;
        private object data;
        private IPathParser pathParser;
        private ISourceProxyFactory sourceProxyFactory;
        private EventHandler valueChangedHandler;
        private List<Token> tokens = new List<Token>();
        private Type dataType;
        private bool disposedValue;
        private Action<StringBuilder> output;
        protected IPathParser PathParser { get { return this.pathParser ?? (this.pathParser = Context.GetApplicationContext().GetService<IPathParser>()); } }
        protected ISourceProxyFactory SourceProxyFactory { get { return this.sourceProxyFactory ?? (this.sourceProxyFactory = Context.GetApplicationContext().GetService<ISourceProxyFactory>()); } }

        public TextTemplateBinding(Action<StringBuilder> output)
        {
            this.output = output;

        }
        public string Template
        {
            get { return this.m_Template; }
            set
            {
                if (string.Equals(this.m_Template, value))
                    return;

                this.m_Template = value;
                OnTemplateChanged();
            }
        }
        public object Data
        {
            get { return this.data; }
            set
            {
                if (Equals(this.data, value))
                    return;

                this.data = value;
                OnDataChanged();
            }
        }

        protected void OnTemplateChanged()
        {
            if (string.IsNullOrEmpty(m_Template) || data == null)
                return;

            this.Unbind();
            this.dataType = data.GetType();
            this.Parse(m_Template, dataType);
            this.Bind();
            this.OnValueChanged();

        }

        protected void OnDataChanged()
        {
            if (string.IsNullOrEmpty(m_Template) || data == null)
                return;

            if (this.dataType == null || !dataType.Equals(data.GetType()))
            {
                this.Unbind();
                this.dataType = data.GetType();
                this.Parse(m_Template, dataType);
                this.Bind();
                this.OnValueChanged();
            }
        }

        private void OnValueChanged()
        {
            StringBuilder buffer = BUFFER.Clear();
            foreach (var token in tokens)
            {
                token.WriteTo(buffer);
            }
            output(buffer);
        }

        protected void Bind()
        {
            if (this.valueChangedHandler == null)
                this.valueChangedHandler = (sender, args) => this.OnValueChanged();

            foreach (var token in tokens)
            {
                if (token is PathParameterToken parameterToken)
                {
                    var sourceProxy = this.SourceProxyFactory.CreateProxy(data, parameterToken.Description);
                    if (sourceProxy is INotifiable notifiable)
                        notifiable.ValueChanged += this.valueChangedHandler;
                    if (parameterToken.SourceProxy is INotifiable oldNotifiable)
                        oldNotifiable.ValueChanged -= this.valueChangedHandler;
                    parameterToken.SourceProxy = sourceProxy;
                }
            }
        }

        protected void Unbind()
        {
            try
            {
                this.dataType = null;
                for (int i = 0; i < tokens.Count; i++)
                {
                    if (tokens[i] is PathParameterToken token)
                    {
                        ISourceProxy sourceProxy = token.SourceProxy;
                        if (sourceProxy == null)
                            continue;

                        if (sourceProxy is INotifiable notifiable)
                            notifiable.ValueChanged -= this.valueChangedHandler;
                        sourceProxy.Dispose();
                    }
                }
            }
            catch (Exception) { }
            finally
            {
                this.tokens.Clear();
            }
        }

        private void Parse(string template, Type dataType)
        {
            bool isArray = dataType.IsArray;
            int pos = 0;
            int len = template.Length;
            char ch = '\x0';
            int bufferIndex = 0;
            Span<char> buffer = len < MAX_STACK_LIMIT ? stackalloc char[len] : new char[len];
            while (true)
            {
                bufferIndex = 0;
                while (pos < len)
                {
                    ch = template[pos];

                    pos++;
                    if (ch == '}')
                    {
                        if (pos < len && template[pos] == '}') // Treat as escape character for }}
                            pos++;
                        else
                            FormatError("missing '}'", template);
                    }

                    if (ch == '{')
                    {
                        if (pos < len && template[pos] == '{') // Treat as escape character for {{
                            pos++;
                        else
                        {
                            pos--;
                            break;
                        }
                    }
                    buffer[bufferIndex++] = ch;
                }

                if (bufferIndex > 0)
                    tokens.Add(new TextToken(buffer.Slice(0, bufferIndex).ToString()));

                if (pos == len)
                    break;

                pos++;
                int index = 0;
                string pathStr = null;
                if (pos == len)
                    FormatError(null, template);

                ch = template[pos];
                if (isArray && IsValidNumber(ch))
                {
                    do
                    {
                        index = index * 10 + ch - '0';
                        pos++;
                        if (pos == len)
                            FormatError(null, template);
                        ch = template[pos];
                    } while (IsValidNumber(ch) && index < 1000000);
                }
                else if (IsValidPathFirstChar(ch))
                {
                    bufferIndex = 0;
                    do
                    {
                        buffer[bufferIndex++] = ch;
                        pos++;
                        if (pos == len)
                            FormatError(null, template);
                        ch = template[pos];
                    } while (IsValidPathChar(ch));

                    if (bufferIndex <= 0)
                        throw new FormatException("The format is illegal.");

                    pathStr = buffer.Slice(0, bufferIndex).ToString();
                }
                else
                {
                    FormatError("Illegal path parameter", template);
                }

                while (pos < len && (ch = template[pos]) == ' ')
                    pos++;

                //Turn off format alignment
                if (ch == ',')
                    FormatError("Alignment is not supported", template);

                //bool leftJustify = false;
                //int width = 0;
                //if (ch == ',')
                //{
                //    pos++;
                //    while (pos < len && template[pos] == ' ')
                //        pos++;

                //    if (pos == len)
                //        FormatError();
                //    ch = template[pos];
                //    if (ch == '-')
                //    {
                //        leftJustify = true;
                //        pos++;
                //        if (pos == len)
                //            FormatError();
                //        ch = template[pos];
                //    }
                //    if (!IsValidNumber(ch))
                //        FormatError();
                //    do
                //    {
                //        width = width * 10 + ch - '0';
                //        pos++;
                //        if (pos == len)
                //            FormatError();
                //        ch = template[pos];
                //    } while (IsValidNumber(ch) && width < 1000000);
                //}

                while (pos < len && (ch = template[pos]) == ' ')
                    pos++;

                bufferIndex = 0;
                if (ch == ':')
                {
                    pos++;
                    while (true)
                    {
                        if (pos == len)
                            FormatError(null, template);
                        ch = template[pos];
                        if (!IsValidFormatChar(ch))
                            break;

                        buffer[bufferIndex++] = ch;
                        pos++;

                        ///Delete the following code. There is a bug here. When there are three curly brackets next to each other, 
                        ///the format will not meet the expectations. For example, {{this is test,my age is {Age:D2}}}, 
                        ///the format after parsing is "D2} " instead of "D2"

                        //if (ch == '{')
                        //{
                        //    if (pos < len && template[pos] == '{')  // Treat as escape character for {{
                        //        pos++;
                        //    else
                        //        FormatError(null, template);
                        //}
                        //else if (ch == '}')
                        //{
                        //    if (pos < len && template[pos] == '}')  // Treat as escape character for }}
                        //        pos++;
                        //    else
                        //    {
                        //        pos--;
                        //        break;
                        //    }
                        //}                      
                    }
                }

                while (pos < len && (ch = template[pos]) == ' ')
                    pos++;

                if (ch != '}')
                    FormatError(null, template);
                pos++;

                ////StringBuilder s = Format(fmt, value);
                //formatter.Format(fmt, value, resultBuilder.Clear());
                //int pad = width - resultBuilder.Length;
                //if (!leftJustify && pad > 0)
                //    builder.Append(' ', pad);
                //builder.AppendStringBuilder(resultBuilder);
                //resultBuilder.Clear();
                //if (leftJustify && pad > 0)
                //    builder.Append(' ', pad);

                string format = bufferIndex > 0 ? buffer.Slice(0, bufferIndex).ToString() : string.Empty;
                Path path = null;
                if (string.IsNullOrEmpty(pathStr))
                {
                    path = new Path();
                    path.AppendIndexed(index);
                }
                else
                {
                    path = PathParser.Parse(pathStr);
                }
                Type valueType = GetValueType(path, dataType);
                tokens.Add(CreatePathParameterToken(format, path, valueType));
            }
        }

        private static void FormatError(string reason, string content)
        {
            if (string.IsNullOrEmpty(reason))
                throw new FormatException($"Invalid Format:{content}");
            throw new FormatException($"{reason},Invalid Format:{content}");
        }

        private PathParameterToken CreatePathParameterToken(string format, Path path, Type valueType)
        {
            TypeCode code = Type.GetTypeCode(valueType);
            switch (code)
            {
                case TypeCode.Boolean:
                    return new PathParameterToken<bool>(format, new ObjectSourceDescription(path), BOOLEAN_FORMATTER);
                case TypeCode.Char:
                    return new PathParameterToken<char>(format, new ObjectSourceDescription(path), CHAR_FORMATTER);
                case TypeCode.Byte:
                    return new PathParameterToken<byte>(format, new ObjectSourceDescription(path), BYTE_FORMATTER);
                case TypeCode.SByte:
                    return new PathParameterToken<sbyte>(format, new ObjectSourceDescription(path), SBYTE_FORMATTER);
                case TypeCode.Int16:
                    return new PathParameterToken<short>(format, new ObjectSourceDescription(path), SHORT_FORMATTER);
                case TypeCode.UInt16:
                    return new PathParameterToken<ushort>(format, new ObjectSourceDescription(path), USHORT_FORMATTER);
                case TypeCode.Int32:
                    return new PathParameterToken<int>(format, new ObjectSourceDescription(path), INT_FORMATTER);
                case TypeCode.UInt32:
                    return new PathParameterToken<uint>(format, new ObjectSourceDescription(path), UINT_FORMATTER);
                case TypeCode.Int64:
                    return new PathParameterToken<long>(format, new ObjectSourceDescription(path), LONG_FORMATTER);
                case TypeCode.UInt64:
                    return new PathParameterToken<ulong>(format, new ObjectSourceDescription(path), ULONG_FORMATTER);
                case TypeCode.Single:
                    return new PathParameterToken<float>(format, new ObjectSourceDescription(path), FLOAT_FORMATTER);
                case TypeCode.Double:
                    return new PathParameterToken<double>(format, new ObjectSourceDescription(path), DOUBLE_FORMATTER);
                case TypeCode.Decimal:
                    return new PathParameterToken<decimal>(format, new ObjectSourceDescription(path), DECIMAL_FORMATTER);
                case TypeCode.DateTime:
                    return new PathParameterToken<DateTime>(format, new ObjectSourceDescription(path), DATETIME_FORMATTER);
                case TypeCode.String:
                    return new PathParameterToken<object>(format, new ObjectSourceDescription(path), DEFAULT_FORMATTER);
                case TypeCode.Object:
                    {
                        if (valueType.Equals(typeof(TimeSpan)))
                            return new PathParameterToken<TimeSpan>(format, new ObjectSourceDescription(path), TIMESPAN_FORMATTER);
                        else if (valueType.Equals(typeof(Vector2)))
                            return new PathParameterToken<Vector2>(format, new ObjectSourceDescription(path), VECTOR2_FORMATTER);
                        else if (valueType.Equals(typeof(Vector3)))
                            return new PathParameterToken<Vector3>(format, new ObjectSourceDescription(path), VECTOR3_FORMATTER);
                        else if (valueType.Equals(typeof(Vector4)))
                            return new PathParameterToken<Vector4>(format, new ObjectSourceDescription(path), VECTOR4_FORMATTER);
                        else if (valueType.Equals(typeof(Rect)))
                            return new PathParameterToken<Rect>(format, new ObjectSourceDescription(path), RECT_FORMATTER);
                        else
                            return new PathParameterToken<object>(format, new ObjectSourceDescription(path), DEFAULT_FORMATTER);
                    }
                default:
                    return new PathParameterToken<object>(format, new ObjectSourceDescription(path), DEFAULT_FORMATTER);
            }
        }

        private Type GetValueType(Path path, Type dataType)
        {
            if (path.IsEmpty)
                return dataType;

            IProxyType type = dataType.AsProxy();
            PathToken token = path.AsPathToken();
            while (true)
            {
                IPathNode node = token.Current;
                if (node is IndexedNode)
                {
                    IProxyItemInfo itemInfo = type.GetItem();
                    if (itemInfo != null)
                        type = itemInfo.ValueType.AsProxy();
                    else
                        return typeof(object);// throw new ArgumentException($"Illegal path:{path}");
                }
                else if (node is MemberNode memberNode)
                {
                    IProxyMemberInfo memberInfo = type.GetMember(memberNode.Name);
                    if (memberInfo is IProxyPropertyInfo propertyInfo)
                        type = propertyInfo.ValueType.AsProxy();
                    else if (memberInfo is IProxyFieldInfo fieldInfo)
                        type = fieldInfo.ValueType.AsProxy();
                    else
                        return typeof(object);//throw new ArgumentException($"Illegal path:{path}");
                }

                if (!token.HasNext())
                    return type.Type;

                token = token.NextToken();
            }
        }

        private bool IsValidFormatChar(char ch)
        {
            if (ch == 123 || ch == 125)//{ }
                return false;

            if ((ch >= 32 && ch <= 122) || ch == 124)
                return true;
            return false;
        }

        private bool IsValidNumber(char ch)
        {
            if (ch >= 48 && ch <= 57)
                return true;
            return false;
        }

        private bool IsValidPathFirstChar(char ch)
        {
            if ((ch >= 65 && ch <= 91) || (ch >= 97 && ch <= 122) || ch == 93 || ch == 95)
                return true;
            return false;
        }

        private bool IsValidPathChar(char ch)
        {
            if ((ch >= 48 && ch <= 57) || (ch >= 65 && ch <= 91) || (ch >= 97 && ch <= 122) || ch == 93 || ch == 95 || ch == 46)
                return true;
            return false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                this.Unbind();
                disposedValue = true;
            }
        }

        ~TextTemplateBinding()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        public abstract class Token
        {
            public abstract void WriteTo(StringBuilder builder);
        }

        private class TextToken : Token
        {
            private string text;
            public TextToken(string text)
            {
                this.text = text;
            }

            public override void WriteTo(StringBuilder builder)
            {
                builder.Append(text);
            }

            public override string ToString()
            {
                return string.Format("TextToken{{ Text:{0} }}", new string(text));
            }
        }

        private abstract class PathParameterToken : Token
        {
            public PathParameterToken(string format, SourceDescription description)
            {
                this.Format = format;
                this.Description = description;
            }

            public string Format { get; }
            public SourceDescription Description { get; }
            public ISourceProxy SourceProxy { get; set; }
        }

        private class PathParameterToken<TValue> : PathParameterToken
        {
            private readonly IFormatter<TValue> formatter;
            public PathParameterToken(string format, SourceDescription description, IFormatter<TValue> formatter) : base(format, description)
            {
                this.formatter = formatter;
            }

            public override void WriteTo(StringBuilder builder)
            {
                if (SourceProxy is IObtainable obtainable)
                {
                    TValue v = obtainable.GetValue<TValue>();
                    formatter.Format(Format, v, builder);
                }
            }

            public override string ToString()
            {
                return string.Format("PathParameterToken{{ Format:{0}, {1} }}", Format, Description);
            }
        }
    }
}
