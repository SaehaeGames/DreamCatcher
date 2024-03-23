using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    //Object Pool을 관리하는 Manager (싱글톤 패턴)

    private static ObjectPoolManager instance;
    public static ObjectPoolManager Instance
    {
        get
        {
            return instance;
        }
    }

    public ObjectPool pool;

    private void Awake()
    {
        if (instance)        //만약 다른 ObjectPoolManager가 있다면 삭제
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

}
