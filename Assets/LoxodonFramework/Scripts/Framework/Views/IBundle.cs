using System.Collections.Generic;

namespace Loxodon.Framework.Views
{
    public interface IBundle
    {
        IDictionary<string, object> Data { get; }

        ICollection<string> Keys { get; }

        ICollection<object> Values { get; }

        void Clear();

        int Count { get; }

        bool ContainsKey(string key);

        bool Remove(string key);

        T Get<T>(string key) where T : new();

        T Get<T>(string key,T defaultValue) where T : new();

        void Put<T>(string key, T value);

        void PutAll(IBundle bundle);

    }
}
