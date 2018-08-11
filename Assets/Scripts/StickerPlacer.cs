using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using object2DOutlines;

public class StickerPlacer : MonoBehaviour {

    public Transform sticker;

    public StickerList stickerList;
    public float outlineSize = 3;

    public Transform target;
    Collider2D targetCollider;
    public LayerMask targetMask;
    public LayerMask stickerMask;

    PolygonCollider2D stickerCollider;
    SpriteRenderer sprite;
    concaveOut outline;

    static Vector2 PIXELSCALE = new Vector2(320, 180);

    //movement
    Vector2 velocity = Vector2.zero;
    public float maxSpeed = 10;
    public float minSpeed = 3;
    public float accel = 100;
    public float velocityChange = 500;

    static float RELEASEWAIT = 1;
    float releaseTime = 0;
    bool push = false;

	// Use this for initialization
	void Start () {
        targetCollider = target.GetComponent<Collider2D>();
    }

    GameObject MakeSticker(GameObject spritePrefab)
    {
        GameObject nSticker = Instantiate(spritePrefab);
        concaveOut nOut = nSticker.AddComponent<concaveOut>();
        nOut.Size_O = outlineSize;
        nOut.Color_O = Color.white;
        nOut.active_SO = false;
        nOut.OrderInLayer_O = 0;
        SpriteRenderer sprite = nSticker.GetComponent<SpriteRenderer>();
        sprite.color = Random.ColorHSV(0, 1, 0, 1, 1, 1);
        return nSticker;
    }
	
	// Update is called once per frame
	void Update () {
        /*Vector2 pos = Input.mousePosition;
        pos.Scale(new Vector2(1f / Screen.width, 1f / Screen.height));
        pos = new Vector2(Mathf.Clamp01(pos.x), Mathf.Clamp01(pos.y));
        pos.Scale(PIXELSCALE);
        pos = pos - (PIXELSCALE * .5f);*/

        if(Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump"))
        {
            sticker = MakeSticker(stickerList.stickers[Random.Range(0, stickerList.stickers.Count)]).transform;
            stickerCollider = sticker.GetComponent<PolygonCollider2D>();
            sprite = sticker.GetComponent<SpriteRenderer>();
            outline = sticker.GetComponent<object2DOutlines.concaveOut>();
        }

        if(sticker == null)
        {
            return;
        }

        //COLLISION---------------------
        Collider2D[] collOut = new Collider2D[20];
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(stickerMask);
        filter.useLayerMask = true;
        if(Physics2D.OverlapCollider(stickerCollider, filter, collOut) > 0)
        {
            //collision detected!
            outline.Color_O = Color.red;
            if(releaseTime >= RELEASEWAIT)
            {
                Vector3 pushOut = stickerCollider.transform.position - collOut[0].transform.position;
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

        //check if the target is overlapped
        bool withinTarget = true;
        Vector2 stickerPos = new Vector2(sticker.transform.position.x, sticker.transform.position.y);
        for(int i = 0; i < stickerCollider.pathCount; i++)
        {
            foreach (Vector2 point in stickerCollider.points)
            {
                withinTarget = withinTarget && targetCollider.OverlapPoint(point + stickerPos);
            }
        }
        if(!withinTarget)
        {
            outline.Color_O = Color.red;
            if (releaseTime >= RELEASEWAIT)
            {
                Vector3 pushOut = target.position - sticker.position;
                Vector2 pushDir = new Vector2(pushOut.x, pushOut.y).normalized;
                if (push)
                {
                    velocity += pushDir * minSpeed * 2;
                } else
                {
                    velocity = pushDir * minSpeed;
                    push = true;
                }
            }
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
            if (releaseTime != 0)
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
            releaseTime = 0;
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
