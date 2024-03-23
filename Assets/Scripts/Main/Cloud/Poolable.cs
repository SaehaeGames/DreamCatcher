using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poolable : MonoBehaviour
{
    //Pooling을 할 오브젝트가 상속받는 클래스

    protected ObjectPool pool;

    public virtual void Create(ObjectPool pool)
    {
        this.pool = pool;
        gameObject.SetActive(false);    //처음 생성될 때는 비활성화(풀 안에 있는 상태)
    }

    public virtual void Push()
    {
        pool.Push(this);
    }
}
