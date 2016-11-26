using System;
using UnityEngine;
using System.Collections;

public class RoomRenderer: MonoBehaviour
{
    public Sprite spriteNormal;
    public Sprite spriteBroken;

    private RoomSlotsRenderer roomSlotsRenderer;
    private CardDesc cardDesc;

    private RoomInfo roomInfo;
    private bool hideUI;

    public void Start()
    {
        roomInfo = GetComponent<RoomInfo>();

        gameObject.transform.position = roomInfo.Slot.transform.position;

        roomSlotsRenderer = Helpers.LocalPlayer.GetComponent<RoomSlotsRenderer>();
        if ( roomSlotsRenderer != null )
            roomSlotsRenderer.OnRoomSpawned( roomInfo.Slot, gameObject );

        CardDB.Instance.Cards.TryGetValue( roomInfo.CardId, out cardDesc );
        if ( cardDesc == null )
        {
            Debug.LogError( "Card not found: " + roomInfo.CardId );
            return;
        }

        UpdateColor();
    }

    public void Update()
    {
        UpdateColor();

        GetComponent<SpriteRenderer>().sprite = roomInfo.IsBroken ? spriteBroken : spriteNormal;
    }

    public void OnDestroy()
    {
        if ( roomSlotsRenderer != null )
            roomSlotsRenderer.OnRoomDestroyed( roomInfo.Slot, gameObject );
    }

    private void UpdateColor()
    {
        var sr = GetComponent<SpriteRenderer>();
        if ( sr == null )
            return;

        if ( cardDesc.Type == CardType.Room )
            sr.color = roomInfo.IsRevealed ? Helpers.ColorRoomRevealed : Helpers.ColorRoomNotRevealed;
        else if ( cardDesc.Type == CardType.Enchantment )
            sr.color = roomInfo.IsRevealed ? Helpers.ColorEnchantmentRevealed : Helpers.ColorEnchantmentNotRevealed;
        else
            sr.color = Helpers.GetCardColor( cardDesc );
    }

    private void OnGUI()
    {
        if ( hideUI )
            return;

        var rect = Helpers.GetScreenSpaceBounds( gameObject );

        if ( Helpers.IsLocalPlayerOverlord() || roomInfo.IsRevealed )
        {
            GUI.Label( new Rect( rect.x + 4, rect.y, rect.width - 4, rect.height ), cardDesc.Name );

            var damageText = roomInfo.Damage.ToString();
            var healthText = ( roomInfo.TotalHealth - roomInfo.DamageDealt ).ToString();

            // Crap
            if ( cardDesc.Type == CardType.Enchantment )
            {
                foreach ( var effectResource in cardDesc.Effects )
                {
                    if ( effectResource is EffectBonusStats )
                    {
                        damageText = "+" + ( effectResource as EffectBonusStats ).Damage;
                        healthText = "+" + ( effectResource as EffectBonusStats ).Health;
                    }
                }
            }

            if ( cardDesc.Type == CardType.Room || cardDesc.Type == CardType.Heart || damageText != "0" )
            {
                GUI.Label( new Rect( rect.x + 5, rect.y + 3, rect.width - 8, rect.height - 6 ), damageText,
                    new GUIStyle {alignment = TextAnchor.LowerLeft, normal = {textColor = Color.white}, fontSize = 20} );
            }

            if ( cardDesc.Type == CardType.Room || cardDesc.Type == CardType.Heart || healthText != "0" )
            {
                GUI.Label( new Rect( rect.x + 3, rect.y + 3, rect.width - 8, rect.height - 6 ), healthText,
                    new GUIStyle { alignment = TextAnchor.LowerRight, normal = { textColor = roomInfo.DamageDealt > 0 ? new Color( 0.8f, 0, 0 ) : Color.white }, fontSize = 20 } );
            }

            if ( roomInfo.DamageDealt > 0 )
            {
                GUI.Label( new Rect( rect.x + 3, rect.y + 3, rect.width - 8, rect.height - 6 - 22 ),
                    "(" + roomInfo.TotalHealth + ")",
                    new GUIStyle { alignment = TextAnchor.LowerRight, normal = { textColor = Color.white } } );
            }
        }

        if ( Helpers.IsLocalPlayerOverlord() )
        {
            if ( !roomInfo.IsActive )
                GUI.Label( rect, "Not active!\r\n(" + cardDesc.ManaCost + ")", Helpers.TextCenter );
        }

        if ( Helpers.IsLocalPlayerRunner() && !roomInfo.IsRevealed )
        {
            GUI.Label( rect, "???", Helpers.TextCenter );
        }
    }

    private void OnMouseDown()
    {
        if ( TargetSelector.Instance.IsActive )
            return;

        if ( Helpers.IsLocalPlayerOverlord() && Helpers.IsOverlordTurn() && !roomInfo.IsActive )
        {
            hideUI = true;

            var dialog = Instantiate( GameRoot.Instance.DialogActivateRoomPrefab );

            var data = dialog.GetComponent<ActivateRoomDialog>();

            data.Room = gameObject;
            data.CardDesc = cardDesc;

            data.Done += delegate { hideUI = false; };
        }
    }
}
