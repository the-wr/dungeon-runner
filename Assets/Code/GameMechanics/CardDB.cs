using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CardDB
{
    private static CardDB instance;

    private Dictionary<string, CardDesc> cards = new Dictionary<string, CardDesc>();

    // -----

    public static CardDB Instance
    {
        get
        {
            if ( instance == null )
                instance = new CardDB();

            return instance;
        }
    }

    public Dictionary<string, CardDesc> Cards { get { return cards; } }

    public CardDB()
    {
        CardDesc c = new CardDesc() {Effects = {new EffectBonusStats() { TargetSelector = new TargetSelectorSelf(), Damage = 1}}};
        XmlSerializeHelper.SaveToXml( c, "card.xml" );
    }

    public void Init()
    {
        Helpers.ProcessFilesInDirectory( Path.Combine( Constants.DataFolder, "Cards" ), LoadCard, true );
        Debug.Log( cards.Count + " cards loaded." );
    }

    private void LoadCard( string fileName )
    {
        var id = Helpers.GetFileShortNameNoExt( fileName );
        var card = XmlSerializeHelper.LoadFromXml<CardDesc>( fileName );

        if ( card != null )
        {
            card.Id = id;
            cards.Add( id, card );
        }
    }
}
