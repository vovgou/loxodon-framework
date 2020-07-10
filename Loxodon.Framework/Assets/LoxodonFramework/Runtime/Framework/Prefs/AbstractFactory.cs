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

namespace Loxodon.Framework.Prefs
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractFactory : IFactory
    {
        private IEncryptor encryptor;
        private ISerializer serializer;
        /// <summary>
        /// 
        /// </summary>
        public AbstractFactory() : this(null, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        public AbstractFactory(ISerializer serializer) : this(serializer, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="encryptor"></param>
        public AbstractFactory(ISerializer serializer, IEncryptor encryptor)
        {
#if UNITY_IOS
			Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
            this.serializer = serializer;
            this.encryptor = encryptor;

            if (this.serializer == null)
                this.serializer = new DefaultSerializer();

            if (this.encryptor == null)
                this.encryptor = new DefaultEncryptor();
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //protected virtual BinaryFormatter CreateBinaryFormatter()
        //{

        //    BinaryFormatter formatter = new BinaryFormatter();
        //    SurrogateSelector selector = new SurrogateSelector();

        //    Vector2SerializationSurrogate v2Surrogate = new Vector2SerializationSurrogate();
        //    selector.AddSurrogate(typeof(Vector2), new StreamingContext(StreamingContextStates.All), v2Surrogate);

        //    Vector3SerializationSurrogate v3Surrogate = new Vector3SerializationSurrogate();
        //    selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), v3Surrogate);

        //    Vector4SerializationSurrogate v4Surrogate = new Vector4SerializationSurrogate();
        //    selector.AddSurrogate(typeof(Vector4), new StreamingContext(StreamingContextStates.All), v4Surrogate);

        //    ColorSerializationSurrogate colorSurrogate = new ColorSerializationSurrogate();
        //    selector.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), colorSurrogate);

        //    Color32SerializationSurrogate color32Surrogate = new Color32SerializationSurrogate();
        //    selector.AddSurrogate(typeof(Color32), new StreamingContext(StreamingContextStates.All), color32Surrogate);

        //    formatter.SurrogateSelector = selector;
        //    return formatter;
        //}

        /// <summary>
        /// 
        /// </summary>
        public IEncryptor Encryptor
        {
            get { return this.encryptor; }
            protected set { this.encryptor = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ISerializer Serializer
        {
            get { return this.serializer; }
            protected set { this.serializer = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract Preferences Create(string name);
    }
}
