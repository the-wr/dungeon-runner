using UnityEngine;
using System.Collections;

public class TurnAnnouncementAnimator: MonoBehaviour
{
    private float lifeTime;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;

        var sr = gameObject.GetComponent<SpriteRenderer>();
        if ( lifeTime <= 0.3 )
            sr.color = new Color( 1, 1, 1, lifeTime / 0.3f );
        else if ( lifeTime > 2 )
            sr.color = new Color( 1, 1, 1, 5 - lifeTime * 2 );
        else if ( lifeTime > 3 )
            sr.enabled = false;
        else
            sr.color = new Color( 1, 1, 1, 1 );
    }
}
