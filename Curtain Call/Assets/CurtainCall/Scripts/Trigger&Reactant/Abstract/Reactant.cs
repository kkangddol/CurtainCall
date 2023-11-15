using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Reactant : MonoBehaviour
{
    public virtual void OnProgressUpdate(float value) //Trigger�� progress�� �ٲ� �� ȣ��  
    {
        
    }
    public virtual void OnConnect()
    {

    }
    public virtual void OnDisconnect()
    {

    }
}
