using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorManager : MonoBehaviour
{
    public GameObject interiorPanel;    // 인테리어 패널
    public InteriorCategory category;   //
    
    public void OpenInteriorPanel()     // 인테리어 패널을 여는 함수
    {
        interiorPanel.SetActive(true);
        //category.RefreshCategory(currentTabIndex);
    }

    public void CloseInteriorPanel()     // 인테리어 패널을 여는 함수
    {
        interiorPanel.SetActive(false);
    }
}
