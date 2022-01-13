using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SpicaSDK.Runtime.Utils
{
    public class MongoAggregation : KeyValueStringBuilder
    {
        public MongoAggregation()
        {
        }

        public MongoAggregation(int count) : base(count)
        {
        }

        public override string GetString()
        {
            // StringBuilder stringBuilder = new StringBuilder();
            // stringBuilder.Append("{");
            // foreach (var singleParam in value)
            // {
            //     stringBuilder.Append($"\"{singleParam.Key}\":\"{singleParam.Value}\",");
            // }
            //
            // if (stringBuilder.Length > 0)
            //     stringBuilder.Remove(stringBuilder.Length - 1, 1);
            //
            // stringBuilder.Append("}");
            //
            // return stringBuilder.ToString();
            return JsonConvert.SerializeObject(value.ToDictionary(param => param.Key, param => param.Value));
        }
    }
}