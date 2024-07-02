using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    public BoxCollider2D col;
    public int startNum, endNum;

    public void addColliderToLine()
    {
        Vector2 StartPoint = gameObject.GetComponent<LineRenderer>().GetPosition(0);
        Vector2 EndPoint = gameObject.GetComponent<LineRenderer>().GetPosition(1);

        col.transform.position = (EndPoint + StartPoint) / 2;
        float lineLength = Vector2.Distance(StartPoint, EndPoint);
        col.size = new Vector2(lineLength - 0.5f, 0.1f);

        Vector2 btw = EndPoint - StartPoint;
        float angle = Mathf.Atan2(btw.y, btw.x) * Mathf.Rad2Deg;
        col.transform.Rotate(0, 0, angle);
    }
}
