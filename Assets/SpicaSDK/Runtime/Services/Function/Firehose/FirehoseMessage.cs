using System.Collections.Generic;
using Newtonsoft.Json;

namespace Plugins.SpicaSDK.Runtime.Services.Function.Firehose
{
    public class FirehoseMessage
    {
        [JsonProperty] private string name;
        [JsonProperty] private Dictionary<string, object> data;

        public FirehoseMessage(string name)
        {
            this.name = name;
            data = new Dictionary<string, object>(16);
        }

        public void SetData(string key, object value)
        {
            if (!data.ContainsKey(key))
            {
                data.Add(key, value);
                return;
            }

            data[key] = value;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}