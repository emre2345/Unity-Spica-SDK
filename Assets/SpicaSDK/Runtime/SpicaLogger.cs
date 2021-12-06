using UnityEngine;

namespace SpicaSDK.Services
{
    public static class SpicaLogger
    {
        public static void Log(string message)
        {
            Debug.Log($"[ Spica ] - {message}");
        }
    }
}