using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T s;

    public static T S
    {
        get
        {
            if(s == null)
            {
                s = FindObjectOfType<T>();
                  if(s== null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    s = obj.AddComponent<T>();
                }
            }
            return s;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected virtual void Awake()
    {
        if(S == null)
        {
            s = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(s != this)
        {
            Destroy(gameObject);
        }
    }
}
