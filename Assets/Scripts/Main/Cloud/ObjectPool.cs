using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    //Object Pooling을 구현하는 클래스
    //객체 생성 -> 비활성화 후 스택에 푸쉬 -> 활성화 후 팝

    [Header ("[Object Setting]")]
    [SerializeField] private Poolable[] poolObj;     //생성할 오브젝트 배열(여러 오브젝트 랜덤 생성)
    [SerializeField] private int allocateCount = 5;  //생성할 오브젝트 개수
    [SerializeField] private int createNum = 0;      //랜덤 생성할 오브젝트 번호
    private int createNum_start, createNum_end; //랜덤 생성할 오브젝트 번호 범위

    private Stack<Poolable> poolStack = new Stack<Poolable>();

    void Start()
    {
        //랜덤 생성할 오브젝트 번호값 초기화(시작은 0, 끝은 배열의 요소 개수)

        createNum_start = 0;
        createNum_end = poolObj.Length;

        Allocate();
    }

    public void Allocate()
    {
        //오브젝트 풀 스택에 오브젝트를 추가하는 함수

        for (int i = 0; i < allocateCount; i++)
        {
            createNum = RandomNumber(createNum_start, createNum_end); //생성할 오브젝트 번호 랜덤 생성
            Poolable allocateObj = Instantiate(poolObj[createNum], this.gameObject.transform);     //오브젝트 풀 할 객체 생성
            allocateObj.Create(this);       //오브젝트 생성
            poolStack.Push(allocateObj);    //오브젝트 풀 스택에 추가(비활성화된 채로)
        }
    }

    public GameObject Pop()
    {
        //오브젝트 풀 할 객체 팝 하는 함수

        Poolable obj = poolStack.Pop();     //오브젝트 풀 스택에서 가져옴(스택의 오브젝트 삭제)
        obj.gameObject.SetActive(true);     //오브젝트 활성화
        return obj.gameObject;      //오브젝트 반환
    }

    public void Push(Poolable obj)
    {
        //오브젝트 풀 할 객체 푸쉬하는 함수

        obj.gameObject.SetActive(false);    //오브젝트 비활성화
        poolStack.Push(obj);            //오브젝트 풀 스택에 추가(재사용)
    }

    public int RandomNumber(int range_start, int range_end)
    {
        //오브젝트 생성 번호 랜덤으로 재설정하는 함수

        int number = Random.Range(range_start, range_end); 
        return number;      //랜덤 숫자 반환
    }


    public bool IsInactiveObject()
    {
        //비활성화된 오브젝트 여부를 반환하는 함수

        bool isInactive = false;     //비활성화된 오브젝트 여부
        if (poolStack.Count != 0)  //하이어라키 창에서 오브젝트가 비활성화 되어있다면
        {
            isInactive = true;
        }
        return isInactive;
    }
}
