using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerPlacer : MonoBehaviour {

    public Transform sticker;

    Collider2D collider2D;
    SpriteRenderer sprite;
    object2DOutlines.concaveOut outline;

    static Vector2 PIXELSCALE = new Vector2(320, 180);

    //movement
    Vector2 velocity = Vector2.zero;
    public float maxSpeed = 10;
    public float minSpeed = 3;
    public float accel = 100;
    public float velocityChange = 500;

    static float RELEASEWAIT = .3f;
    float releaseTime = 0;
    bool push = false;

	// Use this for initialization
	void Start () {
        collider2D = sticker.GetComponent<Collider2D>();
        sprite = sticker.GetComponent<SpriteRenderer>();
        outline = sticker.GetComponent<object2DOutlines.concaveOut>();
    }
	
	// Update is called once per frame
	void Update () {
        /*Vector2 pos = Input.mousePosition;
        pos.Scale(new Vector2(1f / Screen.width, 1f / Screen.height));
        pos = new Vector2(Mathf.Clamp01(pos.x), Mathf.Clamp01(pos.y));
        pos.Scale(PIXELSCALE);
        pos = pos - (PIXELSCALE * .5f);*/

        //COLLISION---------------------
        Collider2D[] collOut = new Collider2D[20];
        ContactFilter2D filter = new ContactFilter2D();
        if(Physics2D.OverlapCollider(collider2D, filter, collOut) > 0)
        {
            //collision detected!
            outline.Color_O = Color.red;
            if(releaseTime >= RELEASEWAIT)
            {
                Vector3 pushOut = collider2D.transform.position - collOut[0].transform.position;
                Vector2 pushDir = new Vector2(pushOut.x, pushOut.y).normalized;
                velocity = pushDir * minSpeed;
                push = true;
            }
        }
        else
        {
            outline.Color_O = Color.white;
            push = false;
        }


        //MOVEMENT----------------------
        #region Movement
        Vector2 pos = new Vector2(sticker.transform.position.x, sticker.transform.position.y);

        Vector2 inputVec = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (inputVec.magnitude > 1)
        {
            inputVec.Normalize();
        }

        if (inputVec.magnitude == 0)
        {
            if (!push)
            {
                velocity = Vector2.zero;
            }
            releaseTime += Time.deltaTime;
        }
        else
        {
            releaseTime = 0;
            if (velocity.magnitude < minSpeed)
            {
                velocity = inputVec * minSpeed;
            }
            else if(velocity.magnitude < maxSpeed * .95f)
            {
                velocity += accel * inputVec * Time.deltaTime;
            } else
            {
                velocity += velocityChange * inputVec * Time.deltaTime;
            }
        }
        if (velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }
        pos += velocity * Time.deltaTime;
        sticker.transform.position = pos;

        #endregion
    }
}
