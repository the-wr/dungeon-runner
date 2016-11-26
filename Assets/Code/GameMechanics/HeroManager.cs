using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public class HeroManager
{
    private readonly List<GameObject> heroes = new List<GameObject>();

    public void Init()
    {
    }

    public bool TrySummonHero( CardDesc card )
    {
        if ( heroes.Count >= Constants.MaxHeroCount )
            return false;

        var hero = GameObject.Instantiate( GameRoot.Instance.HeroPrefab );
        var heroInfo = hero.GetComponent<HeroInfo>();
        heroInfo.CardId = card.Id;

        heroInfo.Damage = card.Damage;
        heroInfo.Health = card.Health;
        heroInfo.SummoningSickness = true;

        NetworkServer.Spawn( hero );
        AddHero( hero );

        return true;
    }

    public void PrepareHeroes()
    {
        foreach ( var hero in heroes )
        {
            var hi = hero.GetComponent<HeroInfo>();
            hi.SummoningSickness = false;
        }
    }

    public void RemoveHero( GameObject hero )
    {
        heroes.Remove( hero );
    }

    public bool CanAttack()
    {
        return heroes.Any( h => !h.GetComponent<HeroInfo>().SummoningSickness );
    }

    // -----

    private void AddHero( GameObject hero )
    {
        heroes.Add( hero );
    }

}
