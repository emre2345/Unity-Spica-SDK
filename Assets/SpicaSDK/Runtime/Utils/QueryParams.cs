using System.Text;

namespace SpicaSDK.Runtime.Utils
{
    public class QueryParams : KeyValueStringBuilder
    {
        public static QueryParams Empty => new QueryParams(0);

        public QueryParams()
        {
        }

        public QueryParams(int count) : base(count)
        {
        }

        public override string GetString()
        {
            StringBuilder queryBuilder = new StringBuilder();
            foreach (var queryStr in value)
            {
                queryBuilder.Append($"{queryStr.Key}={queryStr.Value}&");
            }

            if (queryBuilder.Length > 0)
                queryBuilder.Remove(queryBuilder.Length - 1, 1);

            return queryBuilder.ToString();
        }
    }
}