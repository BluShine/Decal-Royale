using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StickerScoring : MonoBehaviour {

    StickerPlacer placer;
    Collider2D target;

    public TextMeshPro scoreText;
    public GameObject bonusPrefab;
    public GameObject scorePopPrefab;

    public float score = 0;

    static Color BADCOLOR = new Color(.7f, .7f, .7f);
    static Color VERYBADCOLOR = new Color(.5f, .5f, .5f);

    public ComboList comboList;

	// Use this for initialization
	void Start () {
        placer = FindObjectOfType<StickerPlacer>();
        target = placer.target.GetComponent<Collider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ScoreSticker(GameObject sticker)
    {
        PolygonCollider2D collider = sticker.GetComponent<PolygonCollider2D>();
        SpriteRenderer sprite = sticker.GetComponent<SpriteRenderer>();

        float totalMultiplier = 1;

        Queue<GameObject> bonusTexts = new Queue<GameObject>();

        //measure how much of the sticker is off-target
        float onTargetAmount = 0;
        foreach(Vector2 point in collider.points)
        {
            if (target.OverlapPoint(point + new Vector2(sticker.transform.position.x, sticker.transform.position.y)))
            {
                onTargetAmount++;
            }
        }
        onTargetAmount = onTargetAmount / collider.points.Length;
        if(onTargetAmount == 1)
        {
            //Nice On!
        }
        else if(onTargetAmount >= .8f)
        {
            totalMultiplier *= .5f;
            bonusTexts.Enqueue(MakeBonusText("x0.5 OFF EDGE", BADCOLOR));
            sprite.color *= new Color(1, 1, 1, 0.9f);
        } else
        {
            totalMultiplier *= 0;
            bonusTexts.Enqueue(MakeBonusText("x0 MISS", VERYBADCOLOR));
            sprite.color *= new Color(1, 1, 1, 0.4f);
        }

        Collider2D[] collOut = new Collider2D[20];
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(placer.stickerMask);
        filter.useLayerMask = true;
        int collisions = Physics2D.OverlapCollider(collider, filter, collOut);

        //check for combos
        int comboCollisions = 0;
        List<SpriteRenderer> nonComboSprites = new List<SpriteRenderer>();
        StickerData topData = sticker.GetComponent<StickerData>();
        for(int i = 0; i < collisions; i++)
        {
            StickerData bottomData = collOut[i].GetComponent<StickerData>();
            bool comboFound = false;
            foreach (StickerCombo combo in comboList.combos)
            {
                if(combo.CheckCombo(topData.tags, bottomData.tags))
                {
                    totalMultiplier *= combo.multiplier;
                    bonusTexts.Enqueue(MakeBonusText(combo.GetComboString(topData, bottomData), combo.color));
                    comboCollisions++;
                    comboFound = true;
                }
            }
            if(!comboFound)
            {
                nonComboSprites.Add(collOut[i].GetComponent<SpriteRenderer>());
            }
        }
        foreach(SpriteRenderer s in nonComboSprites)
        {
            s.color *= new Color(1, 1, 1, 0.8f);
        }

        collisions -= comboCollisions;

        //check how many stickers are overlapping.
        if (collisions == 0)
        {
            //Nice placement!
        } else if(collisions == 1)
        {
            totalMultiplier *= 0.7f;
            bonusTexts.Enqueue(MakeBonusText("x0.7 SINGLE OVERLAP", BADCOLOR));
        } else if(collisions == 2)
        {
            totalMultiplier *= 0.5f;
            bonusTexts.Enqueue(MakeBonusText("x0.5 DOUBLE OVERLAP", BADCOLOR));
        } else
        {
            totalMultiplier *= 0;
            bonusTexts.Enqueue(MakeBonusText("x0 MULTI OVERLAP", VERYBADCOLOR));
        }

        float radialOffset = Mathf.PI * .5f;
        float radDir = 1;
        if(sticker.transform.position.x < 0)
        {
            radDir = -1;
        }
        radialOffset += radDir * Mathf.PI * .3f;
        foreach(GameObject t in bonusTexts)
        {
            t.transform.position = Vector3.Scale(sticker.transform.position, new Vector3(1,1,0)) +
                new Vector3(Mathf.Cos(radialOffset) * 70, Mathf.Sin(radialOffset) * 70,
                    t.transform.position.z);
            t.GetComponent<TextFade>().velocity = new Vector3(Mathf.Cos(radialOffset) * 10, Mathf.Sin(radialOffset) * 10, 0);
            radialOffset += radDir * Mathf.PI * .1f;
        }

        Vector3 bounds = collider.bounds.extents;
        float stickerScore = bounds.x + bounds.y;
        stickerScore *= totalMultiplier;
        stickerScore = Mathf.Round(stickerScore);
        ScorePop(stickerScore);
    }

    void ScorePop(float points)
    {
        score += points;
        scoreText.text = "SCORE: " + score;
        GameObject pop = Instantiate(scorePopPrefab);
        TextMeshPro sText = pop.GetComponent<TextMeshPro>();
        sText.text = "+" + points;
    }

    GameObject MakeBonusText(string text, Color color)
    {
        GameObject bonus = Instantiate(bonusPrefab);
        TextMeshPro tMesh = bonus.GetComponent<TextMeshPro>();
        tMesh.color = color;
        tMesh.text = text;
        return bonus;
    }
}
