using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMover : MonoBehaviour {

    public AudioSource startSound;
    public AudioSource passSound;
    Animator anim;

    float delay = 0;
    int clicks = 0;
    bool started = false;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump"))
        {
            clicks++;
        }
        if(clicks > 2)
        {
            if(!started)
            {
                startSound.Play();
                anim.SetTrigger("Start");
                delay = Random.Range(5.5f, 8);
                started = true;
            }

            delay -= Time.deltaTime;
            if (delay <= 0)
            {
                delay = Random.Range(5.5f, 8);
                anim.SetTrigger("Pass");
                passSound.Play();
            }
        }
	}
}
