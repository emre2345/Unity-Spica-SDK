using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SpicaSDK.Services
{
    [CreateAssetMenu(fileName = "SpicaServerConfig", menuName = "SpicaSDK/SpicaServerConfig", order = 0)]
    public class SpicaServerConfiguration : ScriptableObject, ISpicaServerUrl
    {
        private static SpicaServerConfiguration instance;

        public static SpicaServerConfiguration Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.LoadAll<SpicaServerConfiguration>(string.Empty)[0];
                }

                return instance;
            }
        }

        [SerializeField] protected string prodRootUrl;
        
        public virtual string RootUrl => prodRootUrl;
    }
}