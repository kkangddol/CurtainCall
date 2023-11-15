using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioManager : MonoBehaviour
{
    private static ScenarioManager instance = null;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static ScenarioManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }


}
