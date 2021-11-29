using System.Collections.Generic;

namespace SpicaSDK
{
    public readonly struct Request
    {
        public readonly string Url;

        public readonly string Payload;

        public readonly Dictionary<string, string> Headers;

        public Request(string url) : this(url, string.Empty)
        {
        }

        public Request(string url, string payload) : this(url, payload, new Dictionary<string, string>())
        {
        }

        public Request(string url, string payload, Dictionary<string, string> headers)
        {
            Url = url;
            Payload = payload;
            Headers = headers;
        }

        public bool Equals(Request other)
        {
            if (Headers.Count != other.Headers.Count)
                return false;

            foreach (var header in Headers)
            {
                string otherValue;
                if (other.Headers.TryGetValue(header.Key, out otherValue))
                {
                    if (!header.Value.Equals(otherValue))
                        return false;
                }
            }

            return Url == other.Url && Payload == other.Payload;
        }

        public override bool Equals(object obj)
        {
            return obj is Request other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Url != null ? Url.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Payload != null ? Payload.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Headers != null ? Headers.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}