using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class RelayCode : NetworkBehaviour
{

    public Button testRelayButton;

    public TMP_Text code_text;
    //public TMP_Text variable_text;

    // Start is called before the first frame update
    public void Start()
    {

        // Initializa Button
        testRelayButton.onClick.AddListener(CreateRelay);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void StartArenaScene(ulong clientID)
    {
        NetworkManager.SceneManager.LoadScene("ArenaScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    // On button press
    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log(joinCode);

            code_text.text = joinCode;

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();

            NetworkManager.Singleton.OnClientConnectedCallback += StartArenaScene;

        } catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Joining Relay with code " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();


        } catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}
