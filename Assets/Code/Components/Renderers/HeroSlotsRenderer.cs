using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HeroSlotsRenderer: MonoBehaviour
{
    private readonly List<GameObject> heroes = new List<GameObject>();

    public void OnHeroSpawned( GameObject hero )
    {
        heroes.Add( hero );
    }

    public void OnHeroDestroyed( GameObject hero )
    {
        heroes.Remove( hero );
    }

    public bool HasHeroesToAttack()
    {
        return GetCanAttackHeroes().Count > 0;
    }

    public List<GameObject> GetCanAttackHeroes()
    {
        var result = new List<GameObject>();
        foreach ( var hero in heroes )
        {
            if ( !hero.GetComponent<HeroInfo>().SummoningSickness )
                result.Add( hero );
        }

        return result;
    }

    void Update()
    {
        if ( Helpers.LocalPlayer != gameObject )
            return;

        if ( GameState.Instance == null )
            return;

        bool isInRun = GameState.Instance.RunState != null;
        bool isInCombat = isInRun && GameState.Instance.RunState.GetComponent<RunState>().Stage == RunStage.Combat;

        var heroesInRoster = new List<GameObject>( heroes );
        var heroesInAttack = new List<GameObject>();

        if ( isInRun )
            heroesInAttack = GetCanAttackHeroes();

        foreach ( var hero in heroesInAttack )
            heroesInRoster.Remove( hero );

        // Roster

        DrawHeroes( heroesInRoster, false, -6, 1, true, 4 );

        // Attack
        if ( GameState.Instance.RunState != null )
        {
            var runState = GameState.Instance.RunState.GetComponent<RunState>();
            if ( runState.Room != null )
            {
                var roomInfo = runState.Room.GetComponent<RoomInfo>();
                
                DrawHeroes( heroesInAttack, isInCombat, roomInfo.Slot.transform.position.x - 1.5f, roomInfo.Slot.transform.position.y, false, 3 );
            }
        }
    }

    private static void DrawHeroes( List<GameObject> heroes, bool isInCombat, float x, float y, bool columnsGoRight, int maxInRow )
    {
        int rows = Mathf.Min( heroes.Count, maxInRow );

        for ( int i = 0; i < heroes.Count; ++i )
        {
            int col = i / maxInRow;
            int row = i % maxInRow;

            float rowOffset = row - (float)( rows - 1 ) / 2;
            heroes[i].transform.position = new Vector3( x + col * ( columnsGoRight ? 1.2f : -1.2f ), y - rowOffset * 1.2f, 0 );

            heroes[i].GetComponent<Highlightable>().IsHighlighted = Helpers.IsLocalPlayerRunner() && isInCombat;
        }
    }
}
