using Loxodon.Framework.Utilities;
using System;
using System.Collections.Generic;

namespace Loxodon.Framework.Configurations
{
    class SubsetConfiguration : ConfigurationBase
    {
        private string prefix;
        private ConfigurationBase parent;
        public SubsetConfiguration(ConfigurationBase parent, string prefix)
        {
            this.parent = parent;
            this.prefix = prefix;
        }

        protected string GetParentKey(string key)
        {
            if ("".Equals(key) || key == null)
                throw new ArgumentNullException(key);

            return prefix + KEY_DELIMITER + key;
        }

        protected string GetChildKey(string key)
        {
            if (!key.StartsWith(prefix))
                throw new ArgumentException(string.Format("The parent key '{0}' is not in the subset.", key));

            return key.Substring(prefix.Length + KEY_DELIMITER.Length);
        }

        public override IConfiguration Subset(string prefix)
        {
            return parent.Subset(GetParentKey(prefix));
        }

        public override bool ContainsKey(string key)
        {
            return parent.ContainsKey(GetParentKey(key));
        }

        public override IEnumerator<string> GetKeys()
        {
            return new TransformEnumerator<string, string>(parent.GetKeys(prefix), (key) => GetChildKey(key));
        }

        public override object GetProperty(string key)
        {
            return parent.GetProperty(GetParentKey(key));
        }

        public override void AddProperty(string key, object value)
        {
            parent.AddProperty(GetParentKey(key), value);
        }

        public override void SetProperty(string key, object value)
        {
            parent.SetProperty(GetParentKey(key), value);
        }

        public override void RemoveProperty(string key)
        {
            parent.RemoveProperty(GetParentKey(key));
        }

        public override void Clear()
        {
            IEnumerator<string> it = GetKeys();
            for (; it.MoveNext();)
            {
                RemoveProperty(it.Current);
            }
        }
    }
}
