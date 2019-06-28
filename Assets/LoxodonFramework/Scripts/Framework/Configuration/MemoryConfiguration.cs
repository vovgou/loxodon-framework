using System.Collections.Generic;

namespace Loxodon.Framework.Configurations
{
    public class MemoryConfiguration : ConfigurationBase
    {
        private readonly Dictionary<string, object> dict = new Dictionary<string, object>();

        public MemoryConfiguration()
        {
        }

        public MemoryConfiguration(Dictionary<string, object> dict)
        {
            if (dict != null && dict.Count > 0)
            {
                foreach (var kv in dict)
                {
                    dict.Add(kv.Key, kv.Value);
                }
            }
        }

        public override bool ContainsKey(string key)
        {
            return dict.ContainsKey(key);
        }

        public override IEnumerator<string> GetKeys()
        {
            return dict.Keys.GetEnumerator();
        }

        public override object GetProperty(string key)
        {
            object value = null;
            dict.TryGetValue(key, out value);
            return value;
        }

        public override void AddProperty(string key, object value)
        {
            if (dict.ContainsKey(key))
                throw new AlreadyExistsException(key);

            dict.Add(key, value);
        }

        public override void SetProperty(string key, object value)
        {
            dict[key] = value;
        }

        public override void RemoveProperty(string key)
        {
            dict.Remove(key);
        }

        public override void Clear()
        {
            dict.Clear();
        }
    }
}