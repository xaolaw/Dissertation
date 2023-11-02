using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ArenaNetworkManager : NetworkBehaviour
{
    public Arena arena;
    public CardManager cardManager;

    /////////////////////////////////
    /// Variables for multiplayer ///
    /////////////////////////////////

    public enum GameSignal
    {
        EndTurn = 0,
        PlayCard,
        SetStart
    }

    /////////////////////////////////
    /// Functions for multiplayer ///
    /////////////////////////////////

    private void OrderRelay(GameSignal signal, int arg1, int arg2)
    {
        switch (signal)
        {
            case GameSignal.EndTurn:
                arena._EndTurn();
                break;

            case GameSignal.PlayCard:
                Debug.Log("Opponent played card: " + arg1 + " at " + arg2);

                Card card = cardManager.GetCardByID(arg1, true);
                card.OnPlay(arg2, true);
                break;

            case GameSignal.SetStart:
                arena.playerTurn = (arg1 == 1);
                break;

            default:
                Debug.LogError("unexpected signal with number: " + signal.ToString());
                break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendSignalServerRpc(GameSignal signal, int arg1, int arg2)
    {
        OrderRelay(signal, arg1, arg2);
    }

    [ClientRpc]
    private void SendSignalClientRpc(GameSignal signal, int arg1, int arg2)
    {
        if (!IsServer)
            OrderRelay(signal, arg1, arg2);
    }

    public void SendSignal(GameSignal signal, int arg1 = 0, int arg2 = 0)
    {
        if (IsServer)
        {
            SendSignalClientRpc(signal, arg1, arg2);
        }
        else
        {
            SendSignalServerRpc(signal, arg1, arg2);
        }
    }

    public bool SetStart(int side)
    {
        SendSignalClientRpc(GameSignal.SetStart, side, 0);

        return side == 0;
    }
}
