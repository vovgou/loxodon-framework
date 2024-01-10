using System;
using UnityEngine;
using static Loxodon.Framework.Views.TextMeshPro.IFormattableText;

namespace Loxodon.Framework.Views.TextMeshPro
{
    public class FormattableTextMeshPro : TMPro.TextMeshPro, IFormattableText
    {
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

        public Parameters Parameters
        {
            get { return m_Parameters; }
            set { this.m_Parameters = value; }
        }

        //protected override void OnEnable()
        //{
        //    base.OnEnable();
        //    Initialize();
        //}

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
