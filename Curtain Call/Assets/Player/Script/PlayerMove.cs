using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D rigid;
    public float moveSpeed = 25.0f;
    public float speedLimit = 5.0f;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rigid.AddForce(-(transform.right * moveSpeed));
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            rigid.AddForce((transform.right * moveSpeed));
        }

        if(rigid.velocity.magnitude > speedLimit)
        {
            rigid.velocity = rigid.velocity.normalized;
            rigid.velocity *= speedLimit;
        }
    }
}
