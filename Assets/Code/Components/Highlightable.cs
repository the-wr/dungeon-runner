using System;
using UnityEngine;
using System.Collections;

public class Highlightable: MonoBehaviour
{
    public bool IsDefaultStateVisible;

    public bool IsHighlighted { get; set; }

    public Color Color { set { GetComponent<SpriteRenderer>().color = value; } }

    void Start()
    {
        IsHighlighted = false;
    }

    void Update()
    {
        GetComponent<BoxCollider2D>().enabled = IsHighlighted || IsDefaultStateVisible;

        var sr = GetComponent<SpriteRenderer>();
        sr.enabled = IsHighlighted || IsDefaultStateVisible;

        if ( IsHighlighted )
            sr.color = new Color( sr.color.r, sr.color.g, sr.color.b, (float)( 0.8 + 0.3 * Math.Sin( Time.timeSinceLevelLoad * 10 ) ) );
        else if ( IsDefaultStateVisible )
            sr.color = new Color( sr.color.r, sr.color.g, sr.color.b, 1 );
    }

    void OnMouseDown()
    {
        // TODO: Crap
        TargetSelector.Instance.OnTargetSelected( gameObject );
    }
}
