using System.Collections.Generic;

namespace SpicaSDK.Runtime.Utils
{
    public abstract class KeyValueStringBuilder
    {
        protected List<SingleParam> value;
        
        protected class SingleParam
        {
            public readonly string Key;
            public readonly string Value;

            public SingleParam(string key, string value)
            {
                Key = key;
                Value = value;
            }
        }

        public KeyValueStringBuilder() : this(16)
        {
        }

        public KeyValueStringBuilder(int count)
        {
            value = new List<SingleParam>(count);
        }

        public void Add(string key, string value) => this.value.Add(new SingleParam(key, value));

        public abstract string GetString();
    }
}