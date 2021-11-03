using System.Collections.Generic;
using System.Text;

namespace SpicaSDK
{
    public class QueryParams
    {
        public static string Empty => string.Empty;
        public Dictionary<string, string> Value { get; private set; }

        public QueryParams() : this(16)
        {
        }

        public QueryParams(int count)
        {
            Value = new Dictionary<string, string>(count);
        }

        public string QueryString
        {
            get
            {
                StringBuilder queryBuilder = new StringBuilder();
                foreach (var queryStr in Value)
                {
                    queryBuilder.Append($"{queryStr.Key}:{queryStr.Value}");
                }

                return queryBuilder.ToString();
            }
        }
    }
}