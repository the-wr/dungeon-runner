using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class Helpers
{
    private static GameObject localPlayer;

    public static System.Random Random = new System.Random( Guid.NewGuid().GetHashCode() );

    public static Color ColorSelectorNewRoom = new Color( 0, 150 / 255f, 0 );
    public static Color ColorSelectorReplaceRoom = new Color( 255 / 255f, 174 / 255f, 0 );
    public static Color ColorSelectorAttackRoom = new Color( 211 / 255f, 0, 0 );

    public static Color ColorHero = new Color( 167 / 255f, 100 / 255f, 10 / 255f );
    public static Color ColorSpell = new Color( 205 / 255f, 9 / 255f, 193 / 255f );

    public static Color ColorRoomRevealed = new Color( 140 / 255f, 140 / 255f, 140 / 255f );
    public static Color ColorRoomNotRevealed = new Color( 90 / 255f, 90 / 255f, 90 / 255f );
    public static Color ColorHeart = new Color( 144 / 255f, 0, 3 / 255f );
    public static Color ColorEnchantmentRevealed = new Color( 36 / 255f, 178 / 255f, 180 / 255f );
    public static Color ColorEnchantmentNotRevealed = new Color( 36 / 255f / 2, 178 / 255f / 2, 180 / 255f / 2 );

    public static GUIStyle TextCenter
    {
        get
        {
            return new GUIStyle { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } };
        }
    }

    public static GUIStyle TextCenterH
    {
        get
        {
            return new GUIStyle { alignment = TextAnchor.UpperCenter, normal = { textColor = Color.white } };
        }
    }

    public static GameObject LocalPlayer
    {
        get { return localPlayer ?? ( localPlayer = MatchMaker.Instance.LocalPlayer ); }
    }

    public static bool IsLocalPlayerOverlord()
    {
        if ( GameState.Instance == null )
            return false;

        return LocalPlayer == GameState.Instance.PlayerOverlord;
    }

    public static bool IsLocalPlayerRunner()
    {
        if ( GameState.Instance == null )
            return false;

        return LocalPlayer == GameState.Instance.PlayerRunner;
    }

    public static bool IsOverlordTurn()
    {
        return GameObject.FindObjectOfType<OverlordTurn>() != null;
    }

    public static bool IsRunnerTurn()
    {
        return GameObject.FindObjectOfType<RunnerTurn>() != null;
    }

    public static bool EndTurnButton()
    {
        return GUI.Button( new Rect( 1024 - 100 - 20, 768 - 60 - 20 - 12, 100, 60 + 12 + 4 ), "End Turn" );
    }

    public static void StateLabel( object state )
    {
        GUI.Label( new Rect( 10, 50, 200, 100 ), state.GetType().Name );
    }

    public static void ProcessFilesInDirectory( string dir, Action<string> callback, bool recursive )
    {
        string[] files = Directory.GetFiles( dir );
        //var info = new DirectoryInfo(dir);
        //var files = info.GetFiles().Select(fileInto => fileInto.Name);
        foreach ( string file in files )
        {
            // Subdir?
            if ( recursive && Directory.Exists( file ) )
                ProcessFilesInDirectory( file, callback, true );
            // File
            else
                callback( file );
        }
    }

    public static string GetFileShortName( string fileName )
    {
        if ( string.IsNullOrEmpty( fileName ) )
            return string.Empty;

        string[] parts = fileName.Replace( "\\", "/" ).Split( '/' );
        return parts[parts.Length - 1];
    }

    public static string GetFileShortNameNoExt( string fileName )
    {
        return GetFileShortName( fileName ).Split( '.' ).First();
    }

    public static Camera GetMainCamera()
    {
        return GameObject.FindGameObjectWithTag( "MainCamera" ).GetComponent<Camera>();
    }

    public static Color GetCardColor( CardDesc cardDesc )
    {
        if ( cardDesc.Type == CardType.Hero )
            return ColorHero;
        if ( cardDesc.Type == CardType.Room)
            return ColorRoomRevealed;
        if ( cardDesc.Type == CardType.Heart )
            return ColorHeart;
        if ( cardDesc.Type == CardType.Enchantment)
            return ColorEnchantmentRevealed;
        if ( cardDesc.Type == CardType.Spell )
            return ColorSpell;

        return Color.gray;
    }

    public static Rect GetScreenSpaceBounds( GameObject gameObject )
    {
        var bounds = gameObject.GetComponent<SpriteRenderer>().bounds;

        var screenPos = Helpers.GetMainCamera()
            .WorldToScreenPoint( new Vector3( gameObject.transform.localPosition.x, gameObject.transform.localPosition.y ) );
        var extents = Helpers.GetMainCamera()
            .WorldToScreenPoint( bounds.extents ) - new Vector3( Screen.width / 2f, Screen.height / 2f );

        return new Rect( screenPos.x - extents.x, Screen.height - screenPos.y - extents.y, extents.x * 2, extents.y * 2 );
    }

    public static void Shuffle<T>( this IList<T> list )
    {
        int n = list.Count;
        while ( n > 1 )
        {
            n--;
            int k = Random.Next( n + 1 );
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
