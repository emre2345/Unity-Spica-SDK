using Cysharp.Threading.Tasks;
using SpicaSDK.Runtime.Utils;
using SpicaSDK.Services.Models;

namespace SpicaSDK.Services.Orm
{
    public abstract class BucketConnector<BucketDataModelType> where BucketDataModelType : class
    {
        protected abstract string BucketId { get; }

        public UniTask<BucketConnection<BucketDataModelType>> Connect()
        {
            return Connect(new QueryParams());
        }

        public UniTask<BucketConnection<BucketDataModelType>> Connect(QueryParams queryParams)
        {
            return SpicaSDK.Bucket.Realtime.Connect<BucketDataModelType>(new Id(BucketId), queryParams);
        }

        public UniTask<DocumentChange<BucketDataModelType>> WatchDocument(Id documentId)
        {
            return SpicaSDK.Bucket.Realtime.WatchDocument<BucketDataModelType>(new Id(BucketId), documentId);
        }

        public UniTask<BucketDataModelType> Insert(BucketDataModelType document)
        {
            return SpicaSDK.Bucket.Data.Insert(new Id(BucketId), document);
        }

        public UniTask<bool> Delete(Id documentId)
        {
            return SpicaSDK.Bucket.Data.Delete<BucketDataModelType>(new Id(BucketId), documentId);
        }

        public UniTask<BucketDataModelType> Replace(Id documentId, BucketDataModelType document)
        {
            return SpicaSDK.Bucket.Data.Replace(new Id(BucketId), documentId, document);
        }

        public UniTask<BucketDataModelType> Patch(Id documentId, BucketDataModelType document)
        {
            return SpicaSDK.Bucket.Data.Patch(new Id(BucketId), documentId, document);
        }

        public UniTask<BucketDataModelType> Get(Id documentId)
        {
            return SpicaSDK.Bucket.Data.Get<BucketDataModelType>(new Id(BucketId), documentId, new QueryParams());
        }

        public UniTask<BucketDataModelType> Get(Id documentId, QueryParams queryParams)
        {
            return SpicaSDK.Bucket.Data.Get<BucketDataModelType>(new Id(BucketId), documentId, queryParams);
        }

        public virtual UniTask<BucketDataModelType[]> GetAll()
        {
            return SpicaSDK.Bucket.Data.GetAll<BucketDataModelType>(new Id(BucketId), new QueryParams());
        }

        public UniTask<BucketDataModelType[]> GetAll(QueryParams queryParams)
        {
            return SpicaSDK.Bucket.Data.GetAll<BucketDataModelType>(new Id(BucketId), queryParams);
        }
    }
}