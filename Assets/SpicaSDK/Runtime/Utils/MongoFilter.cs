using System.Text;

namespace SpicaSDK.Runtime.Utils
{
    public class MongoFilter : KeyValueStringBuilder
    {
        public MongoFilter()
        {
        }

        public MongoFilter(int count) : base(count)
        {
        }

        public override string GetString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{");
            foreach (var singleParam in value)
            {
                stringBuilder.Append($"\"{singleParam.Key}\":\"{singleParam.Value}\",");
            }

            if (stringBuilder.Length > 0)
                stringBuilder.Remove(stringBuilder.Length - 1, 1);

            stringBuilder.Append("}");

            return stringBuilder.ToString();
        }
    }
}