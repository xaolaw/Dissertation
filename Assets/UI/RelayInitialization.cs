using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class TestRelayButton : MonoBehaviour
{
    public Button testRelayButton;

    // Start is called before the first frame update
    async void Start()
    {
        // Initialize services
        await UnityServices.InitializeAsync();

        // Sign in anonymously
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        // Initializa Button
        testRelayButton.onClick.AddListener(TestRelay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // On button press
    async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log(joinCode);

        } catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Joining Relay with code " + joinCode);
            await RelayService.Instance.JoinAllocationAsync(joinCode);
        } catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}
