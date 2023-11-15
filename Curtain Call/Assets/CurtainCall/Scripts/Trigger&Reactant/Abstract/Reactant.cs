using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Reactant : MonoBehaviour
{
    public virtual void OnProgressUpdate(float value) //Trigger의 progress가 바뀔 때 호출  
    {
        
    }
    public virtual void OnConnect()
    {

    }
    public virtual void OnDisconnect()
    {

    }
}
