using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using SpicaSDK.Services.Models;
using SpicaSDK.Services.Services.Identity.Models;

namespace SpicaSDK.Interfaces
{
    public interface ISpicaServer
    {
        UniTask<bool> Initialize();
        bool IsAvailable { get; }

        string BucketUrl(Id bucketId);
        string BucketDataUrl(Id bucketId);

        string BucketDataDocumentUrl(Id bucketId, Id documentId);

        string IdentityUrl { get; }
        
        Identity Identity { get; set; }
    }
}