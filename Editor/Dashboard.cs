using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SpicaSDK.Services.Services.Identity.Models;
using UnityEngine;

[CreateAssetMenu]
public class Dashboard : ScriptableObject
{
    [EditorButton(nameof(Login), nameof(Login), nameof(LoginButtonEnabled))]
    public string UserName;
    public string Password;

    async UniTask Login()
    {
        logInProgress = true;
        // Identity identity = await SpicaSDK.Services.SpicaSDK.LogIn(UserName, Password);
        // SpicaSDK.Services.SpicaSDK.SetIdentity(identity);
        await UniTask.Delay(1000);
        logInProgress = false;
    }

    private bool logInProgress = false;
    private bool LoginButtonEnabled() => !logInProgress;
}