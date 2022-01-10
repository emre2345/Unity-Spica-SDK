using Cysharp.Threading.Tasks;
using SpicaSDK.Runtime.Utils;
using SpicaSDK.Services.Models;

namespace SpicaSDK.Services.Orm
{
    public abstract class BucketConnector<BucketDataModelType> where BucketDataModelType : class
    {
        protected abstract string BucketId { get; }

        public UniTask<BucketConnection<BucketDataModelType>> ConnectAsync()
        {
            return ConnectAsync(new QueryParams());
        }

        public UniTask<BucketConnection<BucketDataModelType>> ConnectAsync(QueryParams queryParams)
        {
            return SpicaSDK.ConnectToBucket<BucketDataModelType>(new Id(BucketId), queryParams);
        }

        public UniTask<DocumentChange<BucketDataModelType>> WatchDocument(Id documentId)
        {
            return SpicaSDK.WatchDocument<BucketDataModelType>(new Id(BucketId), documentId);
        }

        public UniTask<BucketDataModelType> Insert(BucketDataModelType document)
        {
            return SpicaSDK.InsertAsync(new Id(BucketId), document);
        }

        public UniTask<bool> Delete(Id documentId)
        {
            return SpicaSDK.DeleteAsync<BucketDataModelType>(new Id(BucketId), documentId);
        }

        public UniTask<BucketDataModelType> Replace(Id documentId, BucketDataModelType document)
        {
            return SpicaSDK.Replace(new Id(BucketId), documentId, document);
        }

        public UniTask<BucketDataModelType> Patch(Id documentId, BucketDataModelType document)
        {
            return SpicaSDK.Patch(new Id(BucketId), documentId, document);
        }

        public UniTask<BucketDataModelType> Get(Id documentId)
        {
            return SpicaSDK.Get<BucketDataModelType>(new Id(BucketId), documentId, new QueryParams());
        }

        public UniTask<BucketDataModelType> Get(Id documentId, QueryParams queryParams)
        {
            return SpicaSDK.Get<BucketDataModelType>(new Id(BucketId), documentId, queryParams);
        }

        public UniTask<BucketDataModelType[]> GetAll()
        {
            return SpicaSDK.GetAll<BucketDataModelType>(new Id(BucketId), new QueryParams());
        }

        public UniTask<BucketDataModelType[]> GetAll(QueryParams queryParams)
        {
            return SpicaSDK.GetAll<BucketDataModelType>(new Id(BucketId), queryParams);
        }
    }
}