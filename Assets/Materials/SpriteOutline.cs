using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOutline : MonoBehaviour {

    public List<SpriteRenderer> sprites = null;
    public float radius = 1;
    [Range(1,4)]
    public int quality = 2;
    public Material material = null;
    private Color color;

    public Color Color
    {
        get
        {
            return color;
        }

        set
        {
            color = value;
            UpdateOutlineSprites();
        }
    }

    // Use this for initialization
    void Start () {
        GenerateOutline();
	}
	
	// Update is called once per frame
	void Update () {

	}

    void UpdateOutlineSprites()
    {
        if (sprites == null)
            return;
        foreach(SpriteRenderer s in sprites)
        {
            s.color = color;
            s.maskInteraction = GetComponent<SpriteRenderer>().maskInteraction;
        }
    }

    public void GenerateOutline()
    {
        SpriteRenderer parentSprite = GetComponent<SpriteRenderer>();
        if (parentSprite == null || material == null)
            return;

        if(sprites != null)
        {
            foreach(SpriteRenderer s in sprites)
            {
                Destroy(s.gameObject);
            }
        }

        sprites = new List<SpriteRenderer>();

        for(int i = 0; i < 4 * quality; i++)
        {
            float r = (i * Mathf.PI * 2) / (4 * quality);
            Vector3 offset = new Vector3(Mathf.Cos(r) * radius, Mathf.Sin(r) * radius, .01f);
            GameObject outlineObj = new GameObject("Outline" + i);
            outlineObj.transform.parent = this.transform;
            outlineObj.transform.localPosition = offset;
            outlineObj.transform.localRotation = Quaternion.Euler(Vector3.zero);

            SpriteRenderer spr = outlineObj.AddComponent<SpriteRenderer>();
            spr.sharedMaterial = material;
            spr.sprite = parentSprite.sprite;
            spr.maskInteraction = parentSprite.maskInteraction;
            spr.color = color;

            sprites.Add(spr);
        }
    }
}
