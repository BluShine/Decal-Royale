using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogMover : MonoBehaviour {

    public AudioSource bark;

    float barkDelay = 1;
    public float moveSpeed = 20;
    public float accel = 5;
    Vector3 velocity = Vector3.zero;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        velocity += accel * Time.deltaTime * new Vector3(Random.Range(-1f, 1), Random.Range(-1f, 1), 0);
        if(velocity.magnitude > moveSpeed)
        {
            velocity = velocity.normalized * moveSpeed;
        }
        transform.position += velocity * Time.deltaTime;
        if(transform.position.x < -160)
        {
            transform.position = new Vector3(-160, transform.position.y, transform.position.z);
            velocity.x = -velocity.x;
        }
        if (transform.position.x > 160)
        {
            transform.position = new Vector3(160, transform.position.y, transform.position.z);
            velocity.x = -velocity.x;
        }
        if (transform.position.y < -90)
        {
            transform.position = new Vector3(transform.position.x, -90, transform.position.z);
            velocity.y = -velocity.y;
        }
        if (transform.position.y > 90)
        {
            transform.position = new Vector3(transform.position.x, 90, transform.position.z);
            velocity.y = -velocity.y;
        }

        barkDelay -= Time.deltaTime;
        if(barkDelay <= 0)
        {
            bark.pitch = Random.Range(.5f, 2);
            bark.Play();
            barkDelay = .3f + (Mathf.Pow(Random.value, 5)) * 4;
        }
    }
}
