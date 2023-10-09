using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField]
    private Transform player1TR;
    [SerializeField]
    private Transform player2TR;

    [SerializeField]
    private float maxSize = 9.5f;
    [SerializeField]
    private float minSize = 5.0f;
    [SerializeField]
    private float sizeRatio = 0.5f;

    [SerializeField]
    private Transform LT;
    [SerializeField]
    private Transform RB;

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    Vector3 centerPos;
    float length;

    private void LateUpdate()
    {
        // �÷��̾� ��ġ�� ���� ī�޶� ��ġ �� size ����
        if (player1TR != null && player2TR != null)
        {
            centerPos = (player1TR.position + player2TR.position) / 2;
            centerPos.z = -10;
            transform.position = centerPos;
            length = (player1TR.position - player2TR.position).magnitude;
            cam.orthographicSize = length * sizeRatio;
        }

        // size �ּ�, �ִ� ����
        if(cam.orthographicSize < minSize)
        {
            cam.orthographicSize = minSize;
        }
        else if ( maxSize < cam.orthographicSize)
        {
            cam.orthographicSize = maxSize;
        }

        // ī�޶� ��ġ �� size�� ���� �� ��� �׵θ��� ����� �ʵ���
        if (LT.position.y < transform.position.y + cam.orthographicSize)
        {
            transform.Translate(new Vector3( 0.0f,-(transform.position.y + cam.orthographicSize - LT.position.y),0.0f));
        }
        else if (transform.position.y - cam.orthographicSize < RB.position.y)
        {
            transform.Translate(new Vector3(0.0f, -(transform.position.y - cam.orthographicSize - RB.position.y), 0.0f));
        }

        if (transform.position.x - (cam.orthographicSize * 16.0f / 9.0f) < LT.position.x)
        {
            transform.Translate(new Vector3(-(transform.position.x - (cam.orthographicSize * 16.0f / 9.0f) - LT.position.x), 0.0f, 0.0f));
        }
        else if (RB.position.x < transform.position.x + (cam.orthographicSize * 16.0f / 9.0f))
        {
            transform.Translate(new Vector3(-(transform.position.x + (cam.orthographicSize * 16.0f / 9.0f) - RB.position.x), 0.0f, 0.0f));
        }
    }
}
