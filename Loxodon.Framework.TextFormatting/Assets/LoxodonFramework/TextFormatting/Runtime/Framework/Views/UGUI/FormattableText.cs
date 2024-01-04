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
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.Views.UGUI
{

    public class FormattableText : Text
    {
        internal static StringBuilder BUFFER = new StringBuilder();

        [SerializeField]
        protected string m_Format = "{0}";
        [SerializeField]
        protected int m_ParameterCount = 1;
        protected internal Parameters m_Parameters;
        public string Format
        {
            get { return this.m_Format; }
            set { this.m_Format = value; }
        }

        public int ParameterCount
        {
            get { return this.m_ParameterCount; }
            set { this.m_ParameterCount = value; }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Initialize();
        }

        public override void SetAllDirty()
        {
            base.SetAllDirty();
            Initialize();
        }

        protected virtual void Initialize()
        {
            if (m_Parameters != null)
                m_Parameters.OnParameterChanged();
            else
                SetText(BUFFER.Clear().Append(m_Format));
        }

        public void SetText(StringBuilder builder)
        {
            this.text = builder.ToString();
        }

        public ArrayParameters<T> AsArray<T>()
        {
            if (m_Parameters == null)
                m_Parameters = new ArrayParameters<T>(this, this.ParameterCount);

            if (m_Parameters is ArrayParameters<T> parameters)
                return parameters;

            throw new NotSupportedException($"The current parameter type has been set to \"{m_Parameters.GetType()}\" and cannot be converted to other types.");
        }

        public GenericParameters<P1> AsParameters<P1>()
        {
            if (m_Parameters == null)
                m_Parameters = new GenericParameters<P1>() { Text = this };

            if (m_Parameters is GenericParameters<P1> parameters)
                return parameters;

            throw new NotSupportedException($"The current parameter type has been set to \"{m_Parameters.GetType()}\" and cannot be converted to other types.");
        }

        public GenericParameters<P1, P2> AsParameters<P1, P2>()
        {
            if (m_Parameters == null)
                m_Parameters = new GenericParameters<P1, P2>() { Text = this };

            if (m_Parameters is GenericParameters<P1, P2> parameters)
                return parameters;

            throw new NotSupportedException($"The current parameter type has been set to \"{m_Parameters.GetType()}\" and cannot be converted to other types.");
        }

        public GenericParameters<P1, P2, P3> AsParameters<P1, P2, P3>()
        {
            if (m_Parameters == null)
                m_Parameters = new GenericParameters<P1, P2, P3>() { Text = this };

            if (m_Parameters is GenericParameters<P1, P2, P3> parameters)
                return parameters;

            throw new NotSupportedException($"The current parameter type has been set to \"{m_Parameters.GetType()}\" and cannot be converted to other types.");
        }

        public GenericParameters<P1, P2, P3, P4> AsParameters<P1, P2, P3, P4>()
        {
            if (m_Parameters == null)
                m_Parameters = new GenericParameters<P1, P2, P3, P4>() { Text = this };

            if (m_Parameters is GenericParameters<P1, P2, P3, P4> parameters)
                return parameters;

            throw new NotSupportedException($"The current parameter type has been set to \"{m_Parameters.GetType()}\" and cannot be converted to other types.");
        }
    }
}