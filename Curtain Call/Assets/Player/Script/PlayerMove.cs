using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private PlayerInfo playerInfo;
    private Rigidbody2D rigid;
    private SkeletonAnimation spineAnim;
    public float moveSpeed = 25.0f;
    public float speedLimit = 5.0f;

    [SerializeField]
    private float babo = 3.0f;

    private void Start()
    {
        playerInfo = GetComponent<PlayerInfo>();
        rigid = GetComponent<Rigidbody2D>();
        spineAnim = GetComponentInChildren<SkeletonAnimation>();
    }

    private void FixedUpdate()
    {
        switch(playerInfo.playerNumber)
        {
            case ePlayerNumber.PLAYER1:
                Player1Move();
                break;
            case ePlayerNumber.PLAYER2:
                Player2Move();
                break;
            default:
                break;
        }

        if (rigid.velocity.magnitude > speedLimit)
        {
            rigid.velocity = rigid.velocity.normalized;
            rigid.velocity *= speedLimit;
        }
    }

    void Player1Move()
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
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }
    }

    void Player2Move()
    {
        if (Input.GetKey(KeyCode.A))
        {
            rigid.AddForce(-(transform.right * moveSpeed));
            spineAnim.AnimationName = "walk";
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        if (Input.GetKey(KeyCode.D))
        {
            rigid.AddForce((transform.right * moveSpeed));
            spineAnim.AnimationName = "walk";
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }
    }
}
