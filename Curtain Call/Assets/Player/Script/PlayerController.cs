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
    WALK
}

public enum eDirection
{
    LEFT,
    RIGHT
}

/// <summary>
/// �÷��̾� ������ �� ������ �ִϸ��̼� ����.
/// �� Ŭ������ �־ ������ �� ������ �ִϸ��̼� ������ ���������� ���ھ �ϳ��� ���ĵ�.
/// 
/// 23.10.23 ������ �����.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public ePlayerNumber playerNumber;
    public float moveSpeed = 25.0f;
    public float speedLimit = 5.0f;
    public float runSpeedRatio = 1.5f;

    private Rigidbody2D _rigid;
    private SkeletonAnimation _spineAnim;

    private ePlayerState _nowState = ePlayerState.STOP;
    private eDirection _nowDirection = eDirection.LEFT;
    private KeyCode _leftKeyCode = KeyCode.None;
    private KeyCode _rightKeyCode = KeyCode.None;
    private KeyCode _runKeyCode = KeyCode.None;
    private Vector2 _moveVec = Vector2.zero;

    private Dictionary<KeyCode, bool> _inputMap = new Dictionary<KeyCode, bool>();

    private void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _spineAnim = GetComponentInChildren<SkeletonAnimation>();
        CheckPlayerNum();

        _inputMap.Add(_leftKeyCode, false);
        _inputMap.Add(_rightKeyCode, false);
        _inputMap.Add(_runKeyCode, false);
    }

    /// <summary>
    /// Input �� ����� FixedUpdate ���� ó���Ѵ�.
    /// LifeCycle �����δ� Fixed�� �տ������� ���������� �����̹Ƿ�..?
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

    private void Update()
    {
        Flush();
        CheckInput();
        SetAction();
    }

    private void LateUpdate()
    {
        CheckIdle();
        AnimateSpine();
    }

    /// <summary>
    /// Start �ϸ鼭 Player Enum�� ���� ������ ��� �ٸ��� �ο�.
    /// </summary>
    void CheckPlayerNum()
    {
        switch (playerNumber)
        {
            case ePlayerNumber.PLAYER1:
                {
                    _leftKeyCode = KeyCode.LeftArrow;
                    _rightKeyCode = KeyCode.RightArrow;
                    _runKeyCode = KeyCode.RightShift;
                }
                break;
            case ePlayerNumber.PLAYER2:
                {
                    _leftKeyCode = KeyCode.A;
                    _rightKeyCode = KeyCode.D;
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
    /// Input�� �޾Ƽ� ó���ϴ� �Լ�
    /// �����̰��� �ϴ� ���͸� �Է¿� ���� ������, ���¸� ������Ʈ ���ش�.
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
    }

    void SetAction()
    {
        if (CanTranstition(ePlayerState.STOP)
            && ((!_inputMap[_leftKeyCode] && !_inputMap[_rightKeyCode])
                || (_nowDirection == eDirection.RIGHT && _inputMap[_leftKeyCode])
                || (_nowDirection == eDirection.LEFT && _inputMap[_rightKeyCode])))
        {
            // �ƹ� �Է��� ���ų�
            // �ݴ� �������� ���ڱ� �ٷ��� �� ��?
            _moveVec = Vector2.zero;
            ChangeState(ePlayerState.STOP);
        }
        else if (_inputMap[_runKeyCode] && CanTranstition(ePlayerState.RUN))
        {
            if (_inputMap[_leftKeyCode])
            {
                ChangeDirection(eDirection.LEFT);
                _moveVec = -(transform.right * moveSpeed) * runSpeedRatio;
            }
            else if (_inputMap[_rightKeyCode])
            {
                ChangeDirection(eDirection.RIGHT);
                _moveVec = transform.right * moveSpeed * runSpeedRatio;
            }

            ChangeState(ePlayerState.RUN);
        }
        else if(CanTranstition(ePlayerState.WALK))
        {
            if (_inputMap[_leftKeyCode])
            {
                ChangeDirection(eDirection.LEFT);
                _moveVec = -(transform.right * moveSpeed);
            }
            else if (_inputMap[_rightKeyCode])
            {
                ChangeDirection(eDirection.RIGHT);
                _moveVec = transform.right * moveSpeed;
            }

            ChangeState(ePlayerState.WALK);
        }
    }

    void DoAction()
    {

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
                        default:
                            break;
                    }
                }
                break;
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
                            return true;
                        default:
                            return false;
                    }
                }
            default:
                return false;
        }

        return false;
    }


    /// <summary>
    /// ���� ���¿� �´� ������ �ִϸ��̼��� ����ϴ� �Լ�.
    /// </summary>
    void AnimateSpine()
    {
        Debug.Log(_nowState);

        switch (_nowState)
        {
            case ePlayerState.GRAB:
                {
                    _spineAnim.loop = true;
                    _spineAnim.AnimationName = "grab";
                }
                break;
            case ePlayerState.JUMP:
                {
                    _spineAnim.loop = true;
                    _spineAnim.AnimationName = "jump";
                }
                break;
            case ePlayerState.PUSH:
                {
                    _spineAnim.loop = true;
                    _spineAnim.AnimationName = "push";
                }
                break;
            case ePlayerState.RUN:
                {
                    _spineAnim.loop = true;
                    _spineAnim.AnimationName = "run";
                }
                break;
            case ePlayerState.STANDBY:
                {
                    _spineAnim.loop = true;
                    _spineAnim.AnimationName = "standby";
                }
                break;
            case ePlayerState.STOP:
                {
                    _spineAnim.loop = false;
                    _spineAnim.AnimationName = "stop";
                }
                break;
            case ePlayerState.WALK:
                {
                    _spineAnim.loop = true;
                    _spineAnim.AnimationName = "walk";
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
                    _spineAnim.skeleton.ScaleX = Mathf.Abs(_spineAnim.skeleton.ScaleX);
                }
                break;
            case eDirection.RIGHT:
                {
                    _nowDirection = eDirection.RIGHT;
                    _spineAnim.skeleton.ScaleX = -Mathf.Abs(_spineAnim.skeleton.ScaleX);
                }
                break;
            default:
                break;
        }
    }

    void CheckIdle()
    {
        if (Mathf.Abs(_rigid.velocity.x) <= 0.0f && CanTranstition(ePlayerState.STANDBY))
        {
            ChangeState(ePlayerState.STANDBY);
        }
    }
}