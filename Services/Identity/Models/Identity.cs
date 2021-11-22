using Newtonsoft.Json;

namespace SpicaSDK.Services.Services.Identity.Models
{
    public readonly struct Identity
    {
        public readonly string Token;
        public readonly string Scheme;
        public readonly string Issuer;

        public Identity(string token, string scheme, string issuer)
        {
            Token = token;
            Scheme = scheme;
            Issuer = issuer;
        }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}