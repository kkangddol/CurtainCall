using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> where T : MonoBehaviour 
{
    private T instance_;

    public T instance
    {
        get
        {
            if (instance_ == null)
            {
                try
                {
                    instance_ = (T)Object.FindObjectOfType<T>();
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.StackTrace);
                    return null;
                }
            }
            return instance_;
        }
    }
}
