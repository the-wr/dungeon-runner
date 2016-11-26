using System;
using UnityEngine;
using System.Collections;

public class CardInHand : MonoBehaviour
{
    public CardDesc CardDesc { get; set; }

    void Update()
    {
        var sr = GetComponent<SpriteRenderer>();
        if ( sr != null )
            sr.color = Helpers.GetCardColor( CardDesc );
    }

    void OnMouseDown()
    {
        if ( CardDesc == null )
            return;

        TargetSelector.Instance.SelectTargetAndCastSpell( CardDesc );
    }

    void OnGUI()
    {
        if ( CardDesc == null )
            return;

        var rect = Helpers.GetScreenSpaceBounds( gameObject );

        GUI.Label( new Rect( rect.x + 3, rect.y, rect.width - 3, rect.height ), CardDesc.Name );

        GUI.Label( new Rect( rect.x + 3, rect.y + 30, rect.width - 3, 25 ), "(" + CardDesc.ManaCost + ")" );

        if ( CardDesc.Type == CardType.Hero || CardDesc.Type == CardType.Room )
        {
            GUI.Label( new Rect( rect.x + 3, rect.y + 3, rect.width - 8, rect.height - 6 ), CardDesc.Damage.ToString(),
                new GUIStyle { alignment = TextAnchor.LowerLeft, normal = { textColor = Color.white }, fontSize = 20 } );
            GUI.Label( new Rect( rect.x + 3, rect.y + 3, rect.width - 8, rect.height - 6 ), CardDesc.Health.ToString(),
                new GUIStyle { alignment = TextAnchor.LowerRight, normal = { textColor = Color.white }, fontSize = 20 } );
        }
    }
}
