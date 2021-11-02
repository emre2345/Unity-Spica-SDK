using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using SpicaSDK.Services.Services.Identity.Models;

namespace SpicaSDK.Interfaces
{
    public interface ISpicaServer
    {
        UniTask<bool> Initialize();
        bool IsAvailable { get; }

        string BucketUrl(string bucketId);
        string BucketDataUrl(string bucketId);

        string IdentityUrl { get; }
        
        Identity Identity { get; set; }
    }
}