using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCollider : MonoBehaviour
{
    private GameObject Line;
    private DCCheckManager DCManager;
    private Vector3 mousePos;


    // Start is called before the first frame update
    void Start()
    {
        Line = gameObject.transform.parent.gameObject;
        DCManager = GameObject.FindWithTag("CreateManager").gameObject.GetComponent<DCCheckManager>();
    }

    private void OnMouseDown()
    {
        Line.GetComponent<LineRenderer>().positionCount = 3;
        Line.GetComponent<LineRenderer>().SetPosition(2, Line.GetComponent<LineRenderer>().GetPosition(1));
    }

    private void OnMouseDrag()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 90;
        Line.GetComponent<LineRenderer>().SetPosition(1, mousePos);
    }

    private void OnMouseUp()
    {
        GameObject targetObj = GetMouseUpObject();
        // 마우스가 올라간 부분(GetMouseUpObject의 반환값)이 HangPoint인 경우
        if(targetObj != null )
        {
            if (targetObj.CompareTag("HangPoint"))
            {
                // HangPoint의 번호와 라인의 시작과 끝 숫자를 저장하는 함수 가동
                DCManager.UpdateHangPnt(targetObj.GetComponent<HangPoint>().HangPointNum,
                    this.transform.parent.GetComponent<Line>().startNum,
                    this.transform.parent.GetComponent<Line>().endNum);
            }
        }
    }

    private GameObject GetMouseUpObject()
    {
        GameObject target = null;

        Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(MousePosition, transform.forward);

        if (hit)
        {
            target = hit.collider.gameObject;
            if (target.name == "LineCollider")
            {
                // 자신을 클릭 했을 때
                Line.GetComponent<LineRenderer>().SetPosition(1, Line.GetComponent<LineRenderer>().GetPosition(2));
                Line.GetComponent<LineRenderer>().positionCount = 2;
                return null;
            }
            Line.GetComponent<LineRenderer>().SetPosition(1, target.transform.position);
            Debug.Log(hit.transform.position);
        }
        else
        {
            Line.GetComponent<LineRenderer>().SetPosition(1, Line.GetComponent<LineRenderer>().GetPosition(2));
            Line.GetComponent<LineRenderer>().positionCount = 2;
        }

        return target;
    }


}
