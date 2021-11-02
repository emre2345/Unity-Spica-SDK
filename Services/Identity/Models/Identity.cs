namespace SpicaSDK.Services.Services.Identity.Models
{
    public readonly struct Identity
    {
        public readonly string Token;

        public Identity(string token)
        {
            Token = token;
        }
    }
}