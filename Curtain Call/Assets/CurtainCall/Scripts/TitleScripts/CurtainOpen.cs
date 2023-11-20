using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CurtainOpen : MonoBehaviour
{
    [SerializeField]
    private Ease curtainEase;

    [SerializeField]
    private Ease titleEase;

    [SerializeField]
    private float targetX;

    [SerializeField]
    private float targetX2;

    [SerializeField]
    private float duration;

    public int tick = 0;

    public GameObject title;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X) && tick == 0)
        {
            this.transform.DOMoveX(targetX , duration).SetEase(curtainEase);
            tick = 1;   
        }
        else if (Input.GetKeyDown(KeyCode.X) && tick == 1)
        {
            this.transform.DOMoveX(targetX - targetX2, duration).SetEase(curtainEase);
            tick = 2;
        }
        else if(tick == 2)
        {
            title.transform.DOMoveY(7, 1).SetEase(titleEase);
        }
    }
}
