using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextFade : MonoBehaviour {

    public bool destroyAtZero = false;
    public float fadeTime = 1;
    public float delayTime = 3;
    float timer = 0;
    TextMeshPro text;
    public Vector3 velocity = Vector3.zero;
    static float FRICTION = 3;

	// Use this for initialization
	void Start () {
        text = GetComponent<TextMeshPro>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += velocity * Time.deltaTime;
        if(velocity.magnitude < Time.deltaTime * FRICTION)
        {
            velocity = Vector3.zero;
        } else
        {
            velocity -= velocity.normalized * FRICTION * Time.deltaTime;
        }

        timer += Time.deltaTime;
        if (timer >= delayTime + fadeTime)
        {
            text.color *= new Color(1, 1, 1, 0);
            if (destroyAtZero)
                Destroy(this.gameObject);
        }
        else if (timer > delayTime)
        {
            float alpha = 1 - Mathf.Clamp01((timer - delayTime) / fadeTime);
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        }
	}
}
