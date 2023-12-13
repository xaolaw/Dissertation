using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class NetworkSetup : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        // Initialize services
        await UnityServices.InitializeAsync();

        // If already signed in
        if (AuthenticationService.Instance.SessionTokenExists)
        {
            // Sign out (temporary)
            AuthenticationService.Instance.SignOut();
        }

        // Sign in anonymously
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
