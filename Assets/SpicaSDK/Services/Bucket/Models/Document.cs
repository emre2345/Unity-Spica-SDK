namespace SpicaSDK.Services.Models
{
    public readonly struct Document
    {
        public struct DocumentOptions
        {
            
        }
        
        public readonly string Id;

        public readonly DocumentOptions Options;

        public Document(string id, DocumentOptions options)
        {
            Id = id;
            Options = options;
        }
    }
}