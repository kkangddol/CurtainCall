using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public enum ePlayerNumber
{
    PLAYER1,
    PLAYER2
}

public enum ePlayerStatus
{
    GRAB,
    JUMP,
    PUSH,
    RUN,
    STANDBY,
    STOP,
    WALK
}

public enum eDirection
{
    LEFT,
    RIGHT
}

/// <summary>
/// 플레이어 움직임 및 스파인 애니메이션 조절.
/// 이 클래스만 있어도 움직임 및 스파인 애니메이션 조절이 가능했으면 좋겠어서 하나로 합쳐둠.
/// 
/// 23.10.23 강석원 인재원.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public ePlayerNumber playerNumber;
    public float moveSpeed = 25.0f;
    public float speedLimit = 5.0f;

    private Rigidbody2D rigid;
    private SkeletonAnimation spineAnim;
    private ePlayerStatus playerStatus = ePlayerStatus.STANDBY;
    private eDirection direction = eDirection.LEFT;
    private bool isRunning = false;

    private KeyCode rightKeyCode = KeyCode.None;
    private KeyCode leftKeyCode = KeyCode.None;
    private KeyCode runKeyCode = KeyCode.None;
    private Vector2 moveVec = Vector2.zero;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spineAnim = GetComponentInChildren<SkeletonAnimation>();
        CheckPlayerNum();
    }

    /// <summary>
    /// Input 의 결과를 FixedUpdate 에서 처리한다.
    /// LifeCycle 상으로는 Fixed가 앞에있지만 물리적으로 움직이므로..?
    /// </summary>
    private void FixedUpdate()
    {
        rigid.AddForce(moveVec);

        float limit = speedLimit;

        if(playerStatus.Equals(ePlayerStatus.RUN))
        {
            limit *= 1.5f;
        }

        if (Mathf.Abs(rigid.velocity.x) > limit)
        {
            if(rigid.velocity.x < 0)
            {
                limit = -limit;
            }

            rigid.velocity = new Vector2(limit, rigid.velocity.y);
        }

        if (rigid.velocity.magnitude <= 0.0f)
        {
            playerStatus = ePlayerStatus.STANDBY;
        }
    }

    private void Update()
    {
        moveVec = Vector2.zero;
        CheckInput();
    }

    private void LateUpdate()
    {
        AnimateSpine();
    }

    /// <summary>
    /// Start 하면서 Player Enum에 따라 움직임 방식 다르게 부여.
    /// </summary>
    void CheckPlayerNum()
    {
        switch (playerNumber)
        {
            case ePlayerNumber.PLAYER1:
                {
                    leftKeyCode = KeyCode.LeftArrow;
                    rightKeyCode = KeyCode.RightArrow;
                    runKeyCode = KeyCode.RightShift;
                }
                break;
            case ePlayerNumber.PLAYER2:
                {
                    leftKeyCode = KeyCode.A;
                    rightKeyCode = KeyCode.D;
                    runKeyCode = KeyCode.LeftShift;
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Input을 받아서 처리하는 함수
    /// 움직이고자 하는 벡터를 입력에 따라 만들어내고, 상태를 업데이트 해준다.
    /// </summary>
    void CheckInput()
    {
        // TODO : 조이스틱 입력도 받아야 하고

        if (Input.GetKey(runKeyCode) && !playerStatus.Equals(ePlayerStatus.RUN))
        {
            playerStatus = ePlayerStatus.RUN;
        }

        if (Input.GetKey(leftKeyCode))
        {
            if(playerStatus.Equals(ePlayerStatus.RUN) && !direction.Equals(eDirection.LEFT))
            {
                playerStatus = ePlayerStatus.STOP;
                return;
            }

            direction = eDirection.LEFT;
            moveVec = -(transform.right * moveSpeed);
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        else if (Input.GetKey(rightKeyCode))
        {
            direction = eDirection.RIGHT;
            moveVec = transform.right * moveSpeed;
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }

        if (moveVec != Vector2.zero && !playerStatus.Equals(ePlayerStatus.RUN))
        {
            playerStatus = ePlayerStatus.WALK;
        }

        if (playerStatus.Equals(ePlayerStatus.RUN))
        {
            moveVec *= 1.5f;
        }
    }

    /// <summary>
    /// 현재 상태에 맞는 스파인 애니메이션을 출력하는 함수.
    /// </summary>
    void AnimateSpine()
    {
        switch (playerStatus)
        {
            case ePlayerStatus.GRAB:
                {
                    spineAnim.AnimationName = "grab";
                }
                break;
            case ePlayerStatus.JUMP:
                {
                    spineAnim.AnimationName = "jump";
                }
                break;
            case ePlayerStatus.PUSH:
                {
                    spineAnim.AnimationName = "push";
                }
                break;
            case ePlayerStatus.RUN:
                {
                    spineAnim.AnimationName = "run";
                }
                break;
            case ePlayerStatus.STANDBY:
                {
                    spineAnim.AnimationName = "standby";
                }
                break;
            case ePlayerStatus.STOP:
                {
                    spineAnim.AnimationName = "stop";
                }
                break;
            case ePlayerStatus.WALK:
                {
                    spineAnim.AnimationName = "walk";
                }
                break;
            default:
                break;
        }
    }
}
