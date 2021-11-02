namespace SpicaSDK
{
    public readonly struct Request
    {
        public readonly string Url;

        public readonly string Payload;

        public Request(string url) : this()
        {
            Url = url;
        }

        public Request(string url, string payload)
        {
            Url = url;
            Payload = payload;
        }

        public bool Equals(Request other)
        {
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
                return ((Url != null ? Url.GetHashCode() : 0) * 397) ^ (Payload != null ? Payload.GetHashCode() : 0);
            }
        }
    }
}