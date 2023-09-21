using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D rigid;
    private SkeletonAnimation spineAnim;
    public float moveSpeed = 25.0f;
    public float speedLimit = 5.0f;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spineAnim = GetComponentInChildren<SkeletonAnimation>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rigid.AddForce(-(transform.right * moveSpeed));
            spineAnim.AnimationName = "walk";
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            rigid.AddForce((transform.right * moveSpeed));
            spineAnim.AnimationName = "walk";
            transform.localScale = new Vector3(-1.0f,1.0f,1.0f);
        }

        if (rigid.velocity.magnitude > speedLimit)
        {
            rigid.velocity = rigid.velocity.normalized;
            rigid.velocity *= speedLimit;
        }
    }
}
