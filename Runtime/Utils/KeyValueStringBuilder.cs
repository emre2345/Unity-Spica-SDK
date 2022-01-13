using System.Collections.Generic;

namespace SpicaSDK.Runtime.Utils
{
    public abstract class KeyValueStringBuilder
    {
        protected List<SingleParam> value;

        protected class SingleParam
        {
            public readonly string Key;
            public readonly object Value;

            public SingleParam(string key, object value)
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

        public void Add(string key, object value) => this.value.Add(new SingleParam(key, value));

        public abstract string GetString();
    }
}