﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StickerPlacer : MonoBehaviour {

    public Transform sticker;
    public Material outlineMat;
    public float outlineRadius = 3;
    public float stickerScale = 1;

    public string nextLevel;

    public AudioSource stickerSound;
    public AudioSource movementSound;

    public Transform warnings;
    public MeshRenderer overlapWarning;
    public MeshRenderer offEdgeWarning;
    public TextMeshPro remainingStickersText;

    public GameObject winText;
    public GameObject loseText;
    bool finished = false;
    bool win = false;

    public StickerList stickerList;
    Queue<GameObject> stickerQueue;
    public float outlineSize = 3;

    public Transform target;
    Collider2D targetCollider;
    public LayerMask targetMask;
    public LayerMask stickerMask;

    PolygonCollider2D stickerCollider;
    SpriteRenderer sprite;
    SpriteOutline outline;

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

    float stickerDepth = 500;

    StickerScoring scoring;

	// Use this for initialization
	void Start () {
        targetCollider = target.GetComponent<Collider2D>();
        scoring = FindObjectOfType<StickerScoring>();
        overlapWarning.enabled = false;
        offEdgeWarning.enabled = false;

        stickerQueue = new Queue<GameObject>();
        foreach(GameObject g in stickerList.stickers)
        {
            stickerQueue.Enqueue(g);
        }

        remainingStickersText.text = "STICKERS LEFT: " + stickerQueue.Count;

        winText.SetActive(false);
        loseText.SetActive(false);
    }

    GameObject MakeSticker(GameObject spritePrefab)
    {
        GameObject nSticker = Instantiate(spritePrefab);
        nSticker.name = spritePrefab.name;
        nSticker.transform.localScale = nSticker.transform.localScale * stickerScale;

        SpriteOutline nOut = nSticker.AddComponent<SpriteOutline>();
        nOut.material = outlineMat;
        nOut.radius = outlineRadius;
        nOut.Color = Color.white;
        nOut.GenerateOutline();

        SpriteRenderer stickerSprite = nSticker.GetComponent<SpriteRenderer>();
        stickerSprite.color = Random.ColorHSV(0, 1, 0, 1, 1, 1);

        nSticker.transform.position = Vector3.zero;
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
            stickerSound.pitch = Random.Range(1, 2.5f);
            stickerSound.Play();
            if(sticker != null)
            {
                scoring.ScoreSticker(sticker.gameObject);
                sticker.position = new Vector3(sticker.position.x, sticker.position.y, stickerDepth);
                sprite.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                stickerDepth -= .1f;
                outline.Color = Color.white;
                outline.GenerateOutline();
                sticker.SetParent(target);
            }

            if (stickerQueue.Count == 0)
            {
                sticker = null;
                if(!finished)
                    FinishLevel();
            }
            else
            {
                sticker = MakeSticker(stickerQueue.Dequeue()).transform;
                stickerCollider = sticker.GetComponent<PolygonCollider2D>();
                sprite = sticker.GetComponent<SpriteRenderer>();
                outline = sticker.GetComponent<SpriteOutline>();
            }

            remainingStickersText.text = "STICKERS LEFT: " + stickerQueue.Count;
        }

        if(finished && Input.GetButtonDown("Submit"))
        {
            if(win)
            {
                SceneManager.LoadScene(nextLevel);
            } else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        if(sticker == null)
        {
            movementSound.Stop();
            return;
        }

        //COLLISION---------------------
        Collider2D[] collOut = new Collider2D[20];
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(stickerMask);
        filter.useLayerMask = true;
        int overlapCount = Physics2D.OverlapCollider(stickerCollider, filter, collOut);
        if (overlapCount > 0)
        {
            bool allCombos = true;
            Collider2D notCombo = null;
            for(int i = 0; i < overlapCount; i++)
            {
                Collider2D c = collOut[i];
                StickerData sticker1 = sticker.GetComponent<StickerData>();
                StickerData sticker2 = c.GetComponent<StickerData>();
                bool comboFound = false;
                foreach(StickerCombo combo in scoring.comboList.combos)
                {
                    comboFound = comboFound || combo.CheckCombo(sticker1.tags, sticker2.tags);
                }
                if (!comboFound)
                {
                    allCombos = false;
                    notCombo = c;
                }
            }
            if (!allCombos)
            {
                //collision detected!
                overlapWarning.enabled = true;
                if (releaseTime >= RELEASEWAIT)
                {
                    Vector3 pushOut = stickerCollider.transform.position - notCombo.transform.position;
                    Vector2 pushDir = new Vector2(pushOut.x, pushOut.y).normalized;
                    velocity = pushDir * minSpeed;
                    push = true;
                }
            } else
            {
                overlapWarning.enabled = false;
                push = false;
            }
        }
        else
        {
            overlapWarning.enabled = false;
            push = false;
        }

        //check if the target is overlapped
        bool withinTarget = true;
        Vector2 stickerPos = new Vector2(sticker.transform.position.x, sticker.transform.position.y);
        Vector2 stickerScale = new Vector2(sticker.lossyScale.x, sticker.lossyScale.y);
        foreach (Vector2 point in stickerCollider.points)
        {
            withinTarget = withinTarget && targetCollider.OverlapPoint(point * stickerScale + stickerPos);
        }
        if(!withinTarget)
        {
            offEdgeWarning.enabled = true;
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
        } else
        {
            offEdgeWarning.enabled = false;
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
            movementSound.Stop();
            if (!push || releaseTime < RELEASEWAIT)
            {
                velocity = Vector2.zero;

                /*//friction
                if(velocity.magnitude < Time.deltaTime * accel)
                {
                    velocity = Vector2.zero;
                } else
                {
                    velocity -= velocity.normalized * Time.deltaTime * accel;
                }*/
            }
            releaseTime += Time.deltaTime;
        }
        else
        {
            movementSound.pitch = Mathf.Clamp(movementSound.pitch + Time.deltaTime * Random.Range(-1f,1f) * 2, .75f, 1.5f);
            if (!movementSound.isPlaying)
            {
                movementSound.Play();
            }
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
        warnings.position = new Vector3(pos.x, pos.y, warnings.position.z);

        #endregion
    }

    void FinishLevel()
    {
        finished = true;
        if(scoring.score >= scoring.targetScore)
        {
            win = true;
            winText.SetActive(true);
        }
        else
        {
            loseText.SetActive(true);
        }
    }
}
