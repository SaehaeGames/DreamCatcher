using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private PlayerDataContainer curPlayerData;   //�÷��̾� ������ ����
    private int curScene;
    private int curTutorial;
    [SerializeField] private GameObject tutorialFadePanal;

    private void Awake()
    {
        for(int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    void Start()
    {
        curPlayerData = GameManager.instance.loadPlayerData;    //�÷��̾��� ��ܹ� ������ ������ ������

        curScene = (int)curPlayerData.dataList[7].dataNumber;
        this.transform.GetChild(curScene).gameObject.SetActive(true);
        if(tutorialFadePanal!=null)
        {
            if ((curScene == 0))
            {
                tutorialFadePanal.SetActive(true);
            }
            else
            {
                tutorialFadePanal.SetActive(false);
            }
        }
        Debug.Log(curScene);

    }

    // �� �ѹ� ���� �� �� ������Ʈ ������Ʈ �Լ�
    // : �� �ѹ��� ����� �� ����ȴ�.
    public void ChangeScene()
    {
        // ���� �� ������Ʈ ��Ȱ��ȭ
        this.transform.GetChild(curScene).gameObject.SetActive(false);
        Debug.Log("ChangeScene : ���� �� ������Ʈ ��Ȱ��ȭ");
        // �� ������Ʈ Ȱ��ȭ
        curScene++;
        this.transform.GetChild(curScene).gameObject.SetActive(true);
        Debug.Log("ChangeScene : �� ������Ʈ Ȱ��ȭ");
        // �� ������ ������Ʈ
        curPlayerData.dataList[7].dataNumber = curScene;
        GameManager.instance.GetComponent<PlayerDataJSON>().DataSaveText(curPlayerData);
        Debug.Log("ChangeScene : �� ������ ������Ʈ");
    }
}
