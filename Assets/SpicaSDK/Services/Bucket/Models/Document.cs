using System.Collections.Generic;
using System.Text;

namespace SpicaSDK.Services.Models
{
    public readonly struct Document
    {
        public readonly struct DocumentOptions
        {
            public readonly Dictionary<string, string> Headers;

            public readonly Dictionary<string, string> Query;

            public DocumentOptions(Dictionary<string, string> headers, Dictionary<string, string> query)
            {
                Headers = headers;
                Query = query;
            }

            public readonly string QueryString
            {
                get
                {
                    StringBuilder queryBuilder = new StringBuilder();
                    foreach (var queryStr in Query)
                    {
                        queryBuilder.Append($"{queryStr.Key}:{queryStr.Value}");
                    }

                    return queryBuilder.ToString();
                }
            }
        }

        public readonly Id Id;

        public readonly DocumentOptions Options;

        public Document(Id id, DocumentOptions options)
        {
            Id = id;
            Options = options;
        }
    }
}