using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class PhaseManager: MonoBehaviour
{
    private GameObject turnObject;

    public event Action GameStarting;
    public event Action<GameObject> NewTurn;
    public event Action<GameObject> TurnEnding;

    private bool mulliganOverlordFinished;
    private bool mulliganRunnerFinished;

    public bool MulliganOverlordFinished { get { return mulliganOverlordFinished; } }
    public bool MulliganRunnerFinished { get { return mulliganRunnerFinished; } }

    public void StartMulliganPhase()
    {
        GameState.Instance.IsMulliganState = true;
    }

    public void StartGame()
    {
        turnObject = Instantiate( GameRoot.Instance.TurnRunnerPrefab );
        NetworkServer.Spawn( turnObject );

        if ( NewTurn != null )
            NewTurn( GameState.Instance.PlayerRunner );
    }

    public void EndTurn()
    {
        var nextTurnIsRunner = !IsRunnerTurn;
        var newTurn = nextTurnIsRunner
            ? GameRoot.Instance.TurnRunnerPrefab : GameRoot.Instance.TurnOverlordPrefab;

        if ( TurnEnding != null )
            TurnEnding( nextTurnIsRunner ? GameState.Instance.PlayerOverlord : GameState.Instance.PlayerRunner );

        if ( turnObject != null )
            Destroy( turnObject );

        if ( GameObject.FindObjectOfType<VictoryAnnouncer>() != null )
            return;

        turnObject = Instantiate( newTurn );
        NetworkServer.Spawn( turnObject );

        if ( NewTurn != null )
            NewTurn( nextTurnIsRunner ? GameState.Instance.PlayerRunner : GameState.Instance.PlayerOverlord );
    }

    public bool IsRunnerTurn
    {
        get { return turnObject.GetComponent<RunnerTurn>() != null; }
    }

    public bool IsOverlordTurn
    {
        get { return turnObject.GetComponent<OverlordTurn>() != null; }
    }

    public void MulliganFinished( GameObject player )
    {
        var gameState = GameState.Instance;

        if ( player == gameState.PlayerOverlord )
            mulliganOverlordFinished = true;
        if ( player == gameState.PlayerRunner )
            mulliganRunnerFinished = true;

        if ( mulliganRunnerFinished && mulliganOverlordFinished )
        {
            gameState.IsMulliganState = false;

            if ( GameStarting != null )
                GameStarting();

            StartGame();
        }
    }
}
