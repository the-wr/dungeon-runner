using UnityEngine;
using System.Collections;

public class RunRenderer: MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        if ( GameState.Instance == null )
            return;

        if ( Helpers.IsLocalPlayerRunner() && Helpers.LocalPlayer == gameObject )
            RunnerGUI();
        if ( Helpers.IsLocalPlayerOverlord() && Helpers.LocalPlayer == gameObject )
            OverlordGUI();
       
    }

    private void RunnerGUI()
    {
        if ( !Helpers.IsRunnerTurn() )
            return;

        var canAttack = GetComponent<HeroSlotsRenderer>().HasHeroesToAttack() && GameState.Instance.RunState == null;

        if ( canAttack )
        {
            var startRun = GUI.Button( new Rect( 12, 600, 140, 40 ), "Attack Dungeon" );
            if ( startRun )
                TargetSelector.Instance.SelectWingAndStartRun();
        }

        var runStateObj = GameState.Instance.RunState;
        if ( runStateObj != null )
        {
            var runState = runStateObj.GetComponent<RunState>();
            if ( runState.Room != null )
            {
                var rect = Helpers.GetScreenSpaceBounds( runState.Room );

                if ( runState.Stage == RunStage.Approach )
                {
                    GUI.Label( new Rect( rect.x, rect.y - 25, rect.width, 25 ), "Approach", Helpers.TextCenterH );

                    var btnEnter = GUI.Button( new Rect( rect.x - 20, rect.y + rect.height + 3, rect.width / 2 + 18, 30 ), "Enter" );
                    var btnLeave = GUI.Button( new Rect( rect.x + rect.width / 2 + 2, rect.y + rect.height + 3, rect.width / 2 + 18, 30 ), "Retreat" );

                    if ( btnEnter )
                        PlayerCommands.Instance.CmdEnterRoom();
                    if ( btnLeave )
                        PlayerCommands.Instance.CmdRetreat();
                }
                if ( runState.Stage == RunStage.Combat )
                {
                    GUI.Label( new Rect( rect.x, rect.y - 25, rect.width, 30 ), "Room Combat", Helpers.TextCenterH );

                    var btnLeave = GUI.Button( new Rect( rect.x + 20, rect.y + rect.height + 3, rect.width - 40, 30 ), "Retreat" );

                    if ( btnLeave )
                        PlayerCommands.Instance.CmdRetreat();
                }
            }
        }
    }

    private void OverlordGUI()
    {
        var runStateObj = GameState.Instance.RunState;
        if ( runStateObj != null )
        {
            var runState = runStateObj.GetComponent<RunState>();
            if ( runState.Room != null )
            {
                var rect = Helpers.GetScreenSpaceBounds( runState.Room );

                if ( runState.Stage == RunStage.Approach )
                    GUI.Label( new Rect( rect.x, rect.y - 25, rect.width, 25 ), "Approach", Helpers.TextCenterH );
                if ( runState.Stage == RunStage.Combat )
                    GUI.Label( new Rect( rect.x, rect.y - 25, rect.width, 25 ), "Room Combat", Helpers.TextCenterH );
            }
        }
    }
}
