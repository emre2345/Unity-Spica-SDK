using System.Collections;
using System.Net;
using Cysharp.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using SpicaSDK.Interfaces;
using SpicaSDK.Services;
using SpicaSDK.Services.Services.Identity;
using SpicaSDK.Services.Services.Identity.Models;
using UnityEngine;
using UnityEngine.TestTools;

namespace SpicaSDK.Tests.Editor.Integration
{
    public class IdentityServiceTests
    {
        private string identity = "spica";
        private string password = "spica";
        private float lifespan = float.MaxValue;
        
        private string url = "http://localhost:4500";

        [UnityTest]
        public IEnumerator LogsIn() => UniTask.ToCoroutine(async delegate
        {
            IHttpClient client = new HttpClient();
            ISpicaServer server = new SpicaServer(new SpicaServerUrl(url) , client);

            IdentityService identityService = new IdentityService(server, client);
            Identity identity = await identityService.LogInAsync(this.identity, password, lifespan);

            Debug.Log(identity);
            Assert.IsNotNull(identity.Token);
            Assert.IsNotEmpty(identity.Token);
        });
    }
}