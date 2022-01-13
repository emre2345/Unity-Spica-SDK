using System.Collections;
using NUnit.Framework;
using SpicaSDK.Runtime.Utils;
using UnityEditor;
using UnityEngine.TestTools;

namespace SpicaSDK.Tests.Editor.Unit
{
    public class UtilsTest
    {
        [Test]
        public void QueryParamsBuilt()
        {
            QueryParams queryParams = new QueryParams(2);
            queryParams.Add("v1", "v1");
            queryParams.Add("v2", "v2");
            
            Assert.AreEqual("v1=v1&v2=v2", queryParams.GetString());
        }
        
        [Test]
        public void MongoFilterBuilt()
        {
            MongoAggregation mongoAggregation = new MongoAggregation(2);
            mongoAggregation.Add("v1", "v1");
            mongoAggregation.Add("v2", "v2");
            
            Assert.AreEqual("{\"v1\":\"v1\",\"v2\":\"v2\"}", mongoAggregation.GetString());
        }
    }
}