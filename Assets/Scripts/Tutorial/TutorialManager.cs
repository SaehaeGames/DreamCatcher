using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private PlayerDataContainer curPlayerData;   //�÷��̾� ������ ����
    private int curScene;

    private void Awake()
    {
        for(int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // json Ȯ�� ����� �� Ȯ��
        //�ؽ�Ʈ���� ������Ʈ �ϴ� �Լ�

        curPlayerData = GameManager.instance.loadPlayerData;    //�÷��̾��� ��ܹ� ������ ������ ������

        curScene = (int)curPlayerData.dataList[7].dataNumber;
        this.transform.GetChild(curScene).gameObject.SetActive(true);
        
        Debug.Log(curScene);

    }

    public void ChangeScene()
    {
        // ���� �� ������Ʈ ��Ȱ��ȭ
        this.transform.GetChild(curScene).gameObject.SetActive(false);

        // �� ������Ʈ Ȱ��ȭ
        curScene++;
        this.transform.GetChild(curScene).gameObject.SetActive(true);

        // �� ������ ������Ʈ
        curPlayerData.dataList[7].dataNumber = curScene;
        GameManager.instance.GetComponent<PlayerDataJSON>().DataSaveText(curPlayerData);
    }
}
