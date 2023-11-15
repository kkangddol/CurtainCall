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
    CLIMBING,
    TURN
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
    [SerializeField]
    private ePlayerNumber _playerNumber;
    [SerializeField]
    private float _moveSpeed = 25.0f;
    [SerializeField]
    private float _climbSpeed = 3.0f;
    [SerializeField]
    private float _speedLimit = 5.0f;
    [SerializeField]
    private float _runSpeedRatio = 1.5f;
    [SerializeField]
    private float _jumpPower = 2.0f;

    [SerializeField]
    private SkeletonAnimation _frontSpineAnim;
    [SerializeField]
    private SkeletonAnimation _backSpineAnim;

    private Rigidbody2D _rigid;

    private ePlayerState _nowState = ePlayerState.STANDBY;
    private eDirection _nowDirection = eDirection.LEFT;
    private string _horizontal;
    private string _vertical;
    private string _runKey;
    private string _jumpKey;
    private Vector2 _moveVec = Vector2.zero;

    private Dictionary<string, float> _inputMap = new Dictionary<string, float>();
    private bool _isClimbable = false;
    private bool _isGrounded = false;
    private Vector3 _colPos = Vector3.zero;

    private void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
        CheckPlayerNum();

        _inputMap.Add(_horizontal, 0.0f);
        _inputMap.Add(_vertical, 0.0f);
        _inputMap.Add(_runKey, 0.0f);
        _inputMap.Add(_jumpKey, 0.0f);
    }

    /// <summary>
    /// Input �� ����� FixedUpdate ���� ó���Ѵ�.
    /// LifeCycle �����δ� Fixed�� �տ������� ���������� �����̹Ƿ�..?
    /// </summary>
    private void FixedUpdate()
    {
        _isGrounded = IsGround();

        Flush();
        CheckInput();
        SetAction();

        ///

        if (CheckIdle())
        {
            ChangeState(ePlayerState.STANDBY);
        }

        if (!_isGrounded && !_nowState.Equals(ePlayerState.CLIMBING))
        {
            ChangeState(ePlayerState.JUMP);
        }

        if (IsClimbing())
        {
            _rigid.velocity = Vector2.zero;
            transform.position = new Vector3(_colPos.x, transform.position.y, transform.position.z);
            _rigid.isKinematic = true;

            _backSpineAnim.timeScale -= 1.0f * Time.deltaTime;
            if (_backSpineAnim.timeScale <= 0.0f)
            {
                _backSpineAnim.timeScale = 0.0f;
            }
        }
        else
        {
            _rigid.isKinematic = false;
        }

        AnimateSpine();

        ///

        _rigid.AddForce(_moveVec);

        float limit = _speedLimit;

        if (_nowState.Equals(ePlayerState.RUN) || _nowState.Equals(ePlayerState.JUMP))
        {
            limit *= _runSpeedRatio;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            _isClimbable = true;
            _colPos = collision.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Ladder"))
        {
            _isClimbable = false;
            _colPos = transform.position;
        }
    }

    /// <summary>
    /// Start �ϸ鼭 Player Enum�� ���� ������ ��� �ٸ��� �ο�.
    /// </summary>
    void CheckPlayerNum()
    {
        switch (_playerNumber)
        {
            case ePlayerNumber.PLAYER1:
                {
                    _horizontal = "Horizontal1";
                    _vertical = "Vertical1";
                    _runKey = "Run1";
                    _jumpKey = "Jump1";
                }
                break;
            case ePlayerNumber.PLAYER2:
                {
                    _horizontal = "Horizontal2";
                    _vertical = "Vertical2";
                    _runKey = "Run2";
                    _jumpKey = "Jump2";
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
            _inputMap[item.Key] = 0.0f;
        }
    }

    /// <summary>
    /// Input�� �޾Ƽ� ó���ϴ� �Լ�
    /// �����̰��� �ϴ� ���͸� �Է¿� ���� ������, ���¸� ������Ʈ ���ش�.
    /// </summary>
    void CheckInput()
    {
        _inputMap[_horizontal] = Input.GetAxis(_horizontal);
        _inputMap[_vertical] = Input.GetAxis(_vertical);
        _inputMap[_runKey] = Input.GetAxis(_runKey);
        _inputMap[_jumpKey] = Input.GetAxis(_jumpKey);
    }

    void SetAction()
    {
        if(_inputMap[_horizontal] < -0.1f)
        {
            ChangeDirection(eDirection.LEFT);
        }
        else if (_inputMap[_horizontal] > 0.1f)
        {
            ChangeDirection(eDirection.RIGHT);
        }

        if (_inputMap[_vertical] > 0.1f)
        {
            if(CanTranstition(ePlayerState.CLIMBING))
            {
                ChangeState(ePlayerState.CLIMBING);
                _backSpineAnim.timeScale = 1;
                transform.Translate(transform.up * _climbSpeed * Time.deltaTime);
                _rigid.isKinematic = true;
                return;
            }
            else if(_isGrounded || !_isClimbable)
            {
                ChangeState(ePlayerState.STANDBY);
            }
        }
        else if(_inputMap[_vertical] < -0.1f)
        {
            if (CanTranstition(ePlayerState.CLIMBING))
            {
                ChangeState(ePlayerState.CLIMBING);
                _backSpineAnim.timeScale = 1;
                transform.Translate(-transform.up * _climbSpeed * Time.deltaTime);
                _rigid.isKinematic = true;
                return;
            }
            else if (_isGrounded)
            {
                ChangeState(ePlayerState.STANDBY);
            }
        }

        if (_isGrounded || _nowState.Equals(ePlayerState.CLIMBING))
        {
            if (_inputMap[_jumpKey] > 0.5f && CanTranstition(ePlayerState.JUMP))
            {
                _rigid.AddForce(transform.up * _jumpPower, ForceMode2D.Impulse);
                ChangeState(ePlayerState.JUMP);
            }

            if (CanTranstition(ePlayerState.STOP)
                && ((_inputMap[_horizontal] == 0.0f)
                    || (_nowDirection == eDirection.RIGHT && _inputMap[_horizontal] < -0.1f)
                    || (_nowDirection == eDirection.LEFT && _inputMap[_horizontal] > 0.1f)))
            {
                // �ƹ� �Է��� ���ų�
                // �ݴ� �������� ���ڱ� �ٷ��� �� ��?
                _moveVec = Vector2.zero;
                ChangeState(ePlayerState.STOP);
            }
            else if (_inputMap[_runKey] > 0.5f && CanTranstition(ePlayerState.RUN))
            {
                if (_inputMap[_horizontal] < -0.1f)
                {
                    _moveVec = -transform.right * _moveSpeed * _runSpeedRatio;
                    ChangeState(ePlayerState.RUN);
                }
                else if (_inputMap[_horizontal] > 0.1f)
                {
                    _moveVec = transform.right * _moveSpeed * _runSpeedRatio;
                    ChangeState(ePlayerState.RUN);
                }
            }
            else if (CanTranstition(ePlayerState.WALK))
            {
                if (_inputMap[_horizontal] < -0.1f)
                {
                    _moveVec = -(transform.right * _moveSpeed);
                    ChangeState(ePlayerState.WALK);
                }
                else if (_inputMap[_horizontal] > 0.1f)
                {
                    _moveVec = transform.right * _moveSpeed;
                    ChangeState(ePlayerState.WALK);
                }
            }
        }
        else
        {
            if (_inputMap[_horizontal] < -0.1f)
            {
                _moveVec = -(transform.right * _moveSpeed / 3);
            }
            else if (_inputMap[_horizontal] > 0.1f)
            {
                _moveVec = transform.right * _moveSpeed / 3;
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
                        case ePlayerState.RUN:
                        case ePlayerState.WALK:
                        case ePlayerState.STANDBY:
                            if (_isGrounded)
                            {
                                return true;
                            }
                            return false;

                        case ePlayerState.CLIMBING:
                            if(_isClimbable)
                            {
                                return true;
                            }
                            return false;

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
                            if (_isGrounded)
                            {
                                return true;
                            }
                            return false;

                        case ePlayerState.CLIMBING:
                            if (_isClimbable)
                            {
                                return true;
                            }
                            return false;

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
                            if (_isGrounded)
                            {
                                return true;
                            }
                            return false;

                        case ePlayerState.CLIMBING:
                            if (_isClimbable)
                            {
                                return true;
                            }
                            return false;

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
                            if (_isGrounded)
                            {
                                return true;
                            }
                            return false;

                        case ePlayerState.CLIMBING:
                            if (_isClimbable)
                            {
                                return true;
                            }
                            return false;

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
                            if (_isGrounded)
                            {
                                return true;
                            }
                            return false;

                        case ePlayerState.CLIMBING:
                            if (_isClimbable)
                            {
                                return true;
                            }
                            return false;

                        default:
                            return false;
                    }
                }
            case ePlayerState.CLIMBING:
                {
                    switch (state)
                    {
                        case ePlayerState.JUMP:
                        case ePlayerState.RUN:
                        case ePlayerState.WALK:
                            return true;

                        case ePlayerState.CLIMBING:
                            if (_isClimbable)
                            {
                                return true;
                            }
                            return false;

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
    /// ���� ���¿� �´� ������ �ִϸ��̼��� ����ϴ� �Լ�.
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

            case ePlayerState.CLIMBING:
                {
                    _frontSpineAnim.gameObject.SetActive(false);
                    _backSpineAnim.gameObject.SetActive(true);
                    _backSpineAnim.loop = true;
                    _backSpineAnim.AnimationName = "climbing";
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
                    if(_nowState.Equals(ePlayerState.CLIMBING))
                    {
                        _backSpineAnim.skeleton.ScaleX = Mathf.Abs(_backSpineAnim.skeleton.ScaleX);
                    }
                    else
                    {
                        _frontSpineAnim.skeleton.ScaleX = Mathf.Abs(_frontSpineAnim.skeleton.ScaleX);
                    }
                }
                break;
            case eDirection.RIGHT:
                {
                    _nowDirection = eDirection.RIGHT;
                    if (_nowState.Equals(ePlayerState.CLIMBING))
                    {
                        _backSpineAnim.skeleton.ScaleX = -Mathf.Abs(_backSpineAnim.skeleton.ScaleX);
                    }
                    else
                    {
                        _frontSpineAnim.skeleton.ScaleX = -Mathf.Abs(_frontSpineAnim.skeleton.ScaleX);
                    }
                }
                break;
            default:
                break;
        }
    }

    bool CheckIdle()
    {
        return IsGround() && Mathf.Abs(_rigid.velocity.x) <= 2.0f && CanTranstition(ePlayerState.STANDBY);
    }

    bool IsGround()
    {
        return Physics2D.Raycast(transform.position, -transform.up, 0.01f);
    }

    bool IsClimbing()
    {
        return _nowState.Equals(ePlayerState.CLIMBING);
    }
}