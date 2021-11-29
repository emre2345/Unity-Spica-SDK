using UnityEngine;

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
                    instance = Resources.Load<SpicaServerConfiguration>(nameof(SpicaServerConfiguration));

                return instance;
            }
        }
        
        [SerializeField] private string rootUrl;

        public string RootUrl => rootUrl;
    }
}