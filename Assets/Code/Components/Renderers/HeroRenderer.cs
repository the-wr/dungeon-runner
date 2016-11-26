using UnityEngine;
using System.Collections;

public class HeroRenderer: MonoBehaviour
{
    private CardDesc cardDesc;
    private HeroInfo heroInfo;

    private HeroSlotsRenderer heroSlotsRenderer;

    // Use this for initialization
    void Start()
    {
        heroInfo = GetComponent<HeroInfo>();

        CardDB.Instance.Cards.TryGetValue( heroInfo.CardId, out cardDesc );
        if ( cardDesc == null )
        {
            Debug.LogError( "Card not found: " + heroInfo.CardId );
            return;
        }

        heroSlotsRenderer = Helpers.LocalPlayer.GetComponent<HeroSlotsRenderer>();
        if ( heroSlotsRenderer != null )
            heroSlotsRenderer.OnHeroSpawned( gameObject );

        UpdateColor();
    }

    void OnGUI()
    {
        var rect = Helpers.GetScreenSpaceBounds( gameObject );
        GUI.Label( new Rect( rect.x + 3, rect.y, rect.width - 3, rect.height ), cardDesc.Name );

        string frags = "";
        for ( int i = 0; i < heroInfo.Frags; ++i )
            frags += "*";
        GUI.Label( new Rect( rect.x + 3, rect.y + 20, rect.width - 3, rect.height ), frags );

        if ( heroInfo.SummoningSickness )
            GUI.Label( rect, "Preparing", Helpers.TextCenter );

        GUI.Label( new Rect( rect.x + 3, rect.y + 3, rect.width - 8, rect.height - 6 ), heroInfo.Damage.ToString(),
            new GUIStyle { alignment = TextAnchor.LowerLeft, normal = { textColor = Color.white }, fontSize = 20 } );
        GUI.Label( new Rect( rect.x + 3, rect.y + 3, rect.width - 8, rect.height - 6 ), heroInfo.Health.ToString(),
            new GUIStyle { alignment = TextAnchor.LowerRight, normal = { textColor = Color.white }, fontSize = 20 } );
    }

    public void OnDestroy()
    {
        if ( heroSlotsRenderer != null )
            heroSlotsRenderer.OnHeroDestroyed( gameObject );
    }

    private void UpdateColor()
    {
        var sr = GetComponent<SpriteRenderer>();
        if ( sr == null )
            return;

        sr.color = Helpers.GetCardColor( cardDesc );
    }

}
