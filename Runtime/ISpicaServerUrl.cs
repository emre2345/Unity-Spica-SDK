namespace SpicaSDK.Services
{
    public interface ISpicaServerUrl
    {
        string RootUrl { get; }
    }
    
    public class SpicaServerUrl : ISpicaServerUrl
    {
        public string RootUrl { get; private set; }

        public SpicaServerUrl(string rootUrl)
        {
            RootUrl = rootUrl;
        }
    }
}