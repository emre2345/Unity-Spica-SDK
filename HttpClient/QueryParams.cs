using System.Collections.Generic;
using System.Text;

namespace SpicaSDK
{
    public class QueryParams
    {
        public static string Empty => string.Empty;
        private Dictionary<string, string> value;

        public QueryParams() : this(16)
        {
        }

        public QueryParams(int count)
        {
            value = new Dictionary<string, string>(count);
        }

        public void AddQuery(string key, string value) => this.value.Add(key, value);

        public string QueryString
        {
            get
            {
                StringBuilder queryBuilder = new StringBuilder();
                foreach (var queryStr in value)
                {
                    queryBuilder.Append($"{queryStr.Key}:{queryStr.Value}");
                }

                return queryBuilder.ToString();
            }
        }
    }
}