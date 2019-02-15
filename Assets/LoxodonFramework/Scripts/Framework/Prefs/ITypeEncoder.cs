using System;

namespace Loxodon.Framework.Prefs
{
    public interface ITypeEncoder
    {
        /// <summary>
        /// Positive or negative, the default value is 0.the type encoder with the higher priority will be executed first.
        /// </summary>
        int Priority { get; set; }

        bool IsSupport(Type type);

        string Encode(object value);

        object Decode(Type type, string value);
    }
}
