using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMover : MonoBehaviour {

    public float moveSpeed = 20;
    public float accel = 5;
    Vector3 velocity = Vector3.zero;

    public float fadeSpeed = 1;
    public float fadeAccel = 1;
    SpriteRenderer sprite;
    float opacity = 1;
    float opacityVelocity = 0;

    static float XBOUND = 250;
    static float YBOUND = 180;

    // Use this for initialization
    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity += accel * Time.deltaTime * new Vector3(Random.Range(-1f, 1), Random.Range(-1f, 1), 0);
        if (velocity.magnitude > moveSpeed)
        {
            velocity = velocity.normalized * moveSpeed;
        }
        transform.position += velocity * Time.deltaTime;
        if (transform.position.x < -XBOUND)
        {
            transform.position = new Vector3(-XBOUND, transform.position.y, transform.position.z);
            velocity.x = -velocity.x;
        }
        if (transform.position.x > XBOUND)
        {
            transform.position = new Vector3(XBOUND, transform.position.y, transform.position.z);
            velocity.x = -velocity.x;
        }
        if (transform.position.y < -YBOUND)
        {
            transform.position = new Vector3(transform.position.x, -YBOUND, transform.position.z);
            velocity.y = -velocity.y;
        }
        if (transform.position.y > YBOUND)
        {
            transform.position = new Vector3(transform.position.x, YBOUND, transform.position.z);
            velocity.y = -velocity.y;
        }

        opacityVelocity += Time.deltaTime * Random.Range(-1f, 1f) * fadeAccel;
        opacityVelocity = Mathf.Clamp(opacityVelocity, -fadeSpeed, fadeSpeed);
        opacity = Mathf.Clamp01(opacity + opacityVelocity * Time.deltaTime);
        sprite.color = new Color(1, 1, 1, opacity);
    }
}
