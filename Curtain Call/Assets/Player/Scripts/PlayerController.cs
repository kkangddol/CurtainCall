using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public enum ePlayerNumber
{
    PLAYER1,
    PLAYER2
}

public enum ePlayerState
{
    GRAB,
    JUMP,
    PUSH,
    RUN,
    STANDBY,
    STOP,
    WALK,
    CLIMB,
    TURN
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
    [SerializeField]
    private ePlayerNumber playerNumber;
    [SerializeField]
    private float moveSpeed = 25.0f;
    [SerializeField]
    private float speedLimit = 5.0f;
    [SerializeField]
    private float runSpeedRatio = 1.5f;

    [SerializeField]
    private SkeletonAnimation _frontSpineAnim;
    [SerializeField]
    private SkeletonAnimation _backSpineAnim;

    private Rigidbody2D _rigid;
    private Collider2D _collider;

    private ePlayerState _nowState = ePlayerState.STOP;
    private eDirection _nowDirection = eDirection.LEFT;
    private KeyCode _leftKeyCode = KeyCode.None;
    private KeyCode _upKeyCode = KeyCode.None;
    private KeyCode _rightKeyCode = KeyCode.None;
    private KeyCode _downKeyCode = KeyCode.None;
    private KeyCode _runKeyCode = KeyCode.None;
    private Vector2 _moveVec = Vector2.zero;

    private Dictionary<KeyCode, bool> _inputMap = new Dictionary<KeyCode, bool>();
    private bool _isClimbable = false;

    private void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        CheckPlayerNum();

        _inputMap.Add(_leftKeyCode, false);
        _inputMap.Add(_upKeyCode, false);
        _inputMap.Add(_rightKeyCode, false);
        _inputMap.Add(_downKeyCode, false);
        _inputMap.Add(_runKeyCode, false);
    }

    /// <summary>
    /// Input 의 결과를 FixedUpdate 에서 처리한다.
    /// LifeCycle 상으로는 Fixed가 앞에있지만 물리적으로 움직이므로..?
    /// </summary>
    private void FixedUpdate()
    {
        _rigid.AddForce(_moveVec);

        float limit = speedLimit;

        if (_nowState.Equals(ePlayerState.RUN))
        {
            limit *= runSpeedRatio;
        }

        if (Mathf.Abs(_rigid.velocity.x) > limit)
        {
            if (_rigid.velocity.x < 0)
            {
                limit = -limit;
            }

            _rigid.velocity = new Vector2(limit, _rigid.velocity.y);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        _isClimbable = false;
        if (collision.CompareTag("Ladder"))
        {
            _isClimbable = true;
        }
    }

    private void Update()
    {
        Flush();
        CheckInput();
        SetAction();
    }

    private void LateUpdate()
    {
        if(CheckIdle())
        {
            ChangeState(ePlayerState.STANDBY);
        }

        if(CheckAerial())
        {
            ChangeState(ePlayerState.JUMP);
        }

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
                    _leftKeyCode = KeyCode.LeftArrow;
                    _upKeyCode = KeyCode.UpArrow;
                    _rightKeyCode = KeyCode.RightArrow;
                    _downKeyCode = KeyCode.DownArrow;
                    _runKeyCode = KeyCode.RightShift;
                }
                break;
            case ePlayerNumber.PLAYER2:
                {
                    _leftKeyCode = KeyCode.A;
                    _upKeyCode = KeyCode.W;
                    _rightKeyCode = KeyCode.D;
                    _downKeyCode = KeyCode.S;
                    _runKeyCode = KeyCode.LeftShift;
                }
                break;
            default:
                break;
        }
    }

    void Flush()
    {
        _moveVec = Vector2.zero;
        foreach (var item in _inputMap.ToList())
        {
            _inputMap[item.Key] = false;
        }
    }

    /// <summary>
    /// Input을 받아서 처리하는 함수
    /// 움직이고자 하는 벡터를 입력에 따라 만들어내고, 상태를 업데이트 해준다.
    /// </summary>
    void CheckInput()
    {
        if (Input.GetKey(_leftKeyCode))
        {
            _inputMap[_leftKeyCode] = true;
        }
        else if (Input.GetKey(_rightKeyCode))
        {
            _inputMap[_rightKeyCode] = true;
        }

        if (Input.GetKey(_runKeyCode))
        {
            _inputMap[_runKeyCode] = true;
        }

        if (Input.GetKey(_upKeyCode))
        {
            _inputMap[_upKeyCode] = true;
        }
        else if (Input.GetKey(_downKeyCode))
        {
            _inputMap[_downKeyCode] = true;
        }
    }

    void SetAction()
    {
        if (_isClimbable)
        {
            if (_inputMap[_downKeyCode])
            {
                _moveVec = -(transform.up * moveSpeed);
                ChangeState(ePlayerState.CLIMB);
                return;
            }
            else if (_inputMap[_upKeyCode])
            {
                _moveVec = (transform.up * moveSpeed);
                ChangeState(ePlayerState.CLIMB);
                return;
            }
        }

        if (CanTranstition(ePlayerState.STOP)
            && ((!_inputMap[_leftKeyCode] && !_inputMap[_rightKeyCode])
                || (_nowDirection == eDirection.RIGHT && _inputMap[_leftKeyCode])
                || (_nowDirection == eDirection.LEFT && _inputMap[_rightKeyCode])))
        {
            // 아무 입력이 없거나
            // 반대 방향으로 갑자기 뛰려고 할 때?
            _moveVec = Vector2.zero;
            ChangeState(ePlayerState.STOP);
        }
        else if (_inputMap[_runKeyCode] && CanTranstition(ePlayerState.RUN))
        {
            if (_inputMap[_leftKeyCode])
            {
                ChangeDirection(eDirection.LEFT);
                _moveVec = -(transform.right * moveSpeed) * runSpeedRatio;
                ChangeState(ePlayerState.RUN);
            }
            else if (_inputMap[_rightKeyCode])
            {
                ChangeDirection(eDirection.RIGHT);
                _moveVec = transform.right * moveSpeed * runSpeedRatio;
                ChangeState(ePlayerState.RUN);
            }
        }
        else if(CanTranstition(ePlayerState.WALK))
        {
            if (_inputMap[_leftKeyCode])
            {
                ChangeDirection(eDirection.LEFT);
                _moveVec = -(transform.right * moveSpeed);
                ChangeState(ePlayerState.WALK);
            }
            else if (_inputMap[_rightKeyCode])
            {
                ChangeDirection(eDirection.RIGHT);
                _moveVec = transform.right * moveSpeed;
                ChangeState(ePlayerState.WALK);
            }
        }
    }

    bool CanTranstition(ePlayerState state)
    {
        switch (_nowState)
        {
            case ePlayerState.GRAB:
                {
                    switch (state)
                    {
                        default:
                            break;
                    }
                }
                break;
            case ePlayerState.JUMP:
                {
                    switch (state)
                    {
                        case ePlayerState.JUMP:
                        case ePlayerState.RUN:
                        case ePlayerState.STANDBY:
                        case ePlayerState.WALK:
                        case ePlayerState.CLIMB:
                            return true;
                        default:
                            return false;
                    }
                }
            case ePlayerState.PUSH:
                {
                    switch (state)
                    {
                        default:
                            break;
                    }
                }
                break;
            case ePlayerState.RUN:
                {
                    switch (state)
                    {
                        case ePlayerState.GRAB:
                        case ePlayerState.JUMP:
                        case ePlayerState.PUSH:
                        case ePlayerState.RUN:
                        case ePlayerState.STOP:
                        case ePlayerState.WALK:
                        case ePlayerState.CLIMB:
                            return true;
                        default:
                            return false;
                    }
                }
            case ePlayerState.STANDBY:
                {
                    switch (state)
                    {
                        case ePlayerState.GRAB:
                        case ePlayerState.JUMP:
                        case ePlayerState.PUSH:
                        case ePlayerState.RUN:
                        case ePlayerState.STANDBY:
                        case ePlayerState.WALK:
                        case ePlayerState.CLIMB:
                            return true;
                        default:
                            return false;
                    }
                }
            case ePlayerState.STOP:
                {
                    switch (state)
                    {
                        case ePlayerState.GRAB:
                        case ePlayerState.JUMP:
                        case ePlayerState.PUSH:
                        case ePlayerState.RUN:
                        case ePlayerState.STANDBY:
                        case ePlayerState.STOP:
                        case ePlayerState.WALK:
                        case ePlayerState.CLIMB:
                            return true;
                        default:
                            return false;
                    }
                }
            case ePlayerState.WALK:
                {
                    switch (state)
                    {
                        case ePlayerState.GRAB:
                        case ePlayerState.JUMP:
                        case ePlayerState.PUSH:
                        case ePlayerState.RUN:
                        case ePlayerState.STANDBY:
                        case ePlayerState.WALK:
                        case ePlayerState.CLIMB:
                            return true;
                        default:
                            return false;
                    }
                }
            case ePlayerState.CLIMB:
                {
                    switch (state)
                    {
                        case ePlayerState.JUMP:
                        case ePlayerState.RUN:
                        case ePlayerState.STANDBY:
                        case ePlayerState.WALK:
                        case ePlayerState.CLIMB:
                            return true;
                        default:
                            return false;
                    }
                }
            case ePlayerState.TURN:
                {
                    switch (state)
                    {
                        default:
                            break;
                    }
                }
                break;
            default:
                return false;
        }

        return false;
    }


    /// <summary>
    /// 현재 상태에 맞는 스파인 애니메이션을 출력하는 함수.
    /// </summary>
    void AnimateSpine()
    {
        Debug.Log(_nowState);

        switch (_nowState)
        {
            case ePlayerState.GRAB:
                {
                    _backSpineAnim.gameObject.SetActive(false);
                    _frontSpineAnim.gameObject.SetActive(true);
                    _frontSpineAnim.loop = true;
                    _frontSpineAnim.AnimationName = "grab";
                }
                break;
            case ePlayerState.JUMP:
                {
                    _backSpineAnim.gameObject.SetActive(false);
                    _frontSpineAnim.gameObject.SetActive(true);
                    _frontSpineAnim.loop = false;
                    _frontSpineAnim.AnimationName = "jump";
                }
                break;
            case ePlayerState.PUSH:
                {
                    _backSpineAnim.gameObject.SetActive(false);
                    _frontSpineAnim.gameObject.SetActive(true);
                    _frontSpineAnim.loop = true;
                    _frontSpineAnim.AnimationName = "push";
                }
                break;
            case ePlayerState.RUN:
                {
                    _backSpineAnim.gameObject.SetActive(false);
                    _frontSpineAnim.gameObject.SetActive(true);
                    _frontSpineAnim.loop = true;
                    _frontSpineAnim.AnimationName = "run";
                }
                break;
            case ePlayerState.STANDBY:
                {
                    _backSpineAnim.gameObject.SetActive(false);
                    _frontSpineAnim.gameObject.SetActive(true);
                    _frontSpineAnim.loop = true;
                    _frontSpineAnim.AnimationName = "standby";
                }
                break;
            case ePlayerState.STOP:
                {
                    _backSpineAnim.gameObject.SetActive(false);
                    _frontSpineAnim.gameObject.SetActive(true);
                    _frontSpineAnim.loop = false;
                    _frontSpineAnim.AnimationName = "stop";
                }
                break;
            case ePlayerState.WALK:
                {
                    _backSpineAnim.gameObject.SetActive(false);
                    _frontSpineAnim.gameObject.SetActive(true);
                    _frontSpineAnim.loop = true;
                    _frontSpineAnim.AnimationName = "walk";
                }
                break;

            case ePlayerState.CLIMB:
                {
                    _frontSpineAnim.gameObject.SetActive(false);
                    _backSpineAnim.gameObject.SetActive(true);
                    _backSpineAnim.loop = true;
                    _backSpineAnim.AnimationName = "climb";
                }
                break;
            case ePlayerState.TURN:
                {
                    _frontSpineAnim.gameObject.SetActive(false);
                    _backSpineAnim.gameObject.SetActive(true);
                    _backSpineAnim.loop = true;
                    _backSpineAnim.AnimationName = "turn";
                }
                break;
            default:
                break;
        }
    }

    void ChangeState(ePlayerState state)
    {
        _nowState = state;
    }

    void ChangeDirection(eDirection direction)
    {
        switch (direction)
        {
            case eDirection.LEFT:
                {
                    _nowDirection = eDirection.LEFT;
                    _frontSpineAnim.skeleton.ScaleX = Mathf.Abs(_frontSpineAnim.skeleton.ScaleX);
                }
                break;
            case eDirection.RIGHT:
                {
                    _nowDirection = eDirection.RIGHT;
                    _frontSpineAnim.skeleton.ScaleX = -Mathf.Abs(_frontSpineAnim.skeleton.ScaleX);
                }
                break;
            default:
                break;
        }
    }

    bool CheckIdle()
    {
        return Mathf.Abs(_rigid.velocity.x) <= 0.0f && CanTranstition(ePlayerState.STANDBY);
    }

    bool CheckAerial()
    {
        return Mathf.Abs(_rigid.velocity.y) >= 3.0f;
    }
}