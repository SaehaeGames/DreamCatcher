using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private PlayerDataContainer curPlayerData;   //�÷��̾� ������ ����
    private int curScene;
    private int curTutorial;
    [SerializeField] private GameObject tutorialFadePanal;

    private ScriptBox scriptBox;

    private void Awake()
    {
        for(int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(false);
        }
        scriptBox = GameObject.FindObjectOfType<ScriptBox>();
    }

    void Start()
    {
        // �÷��̾� ������(PlayerDataFile) �ε�
        curPlayerData = GameManager.instance.loadPlayerData;

        // ���� Ʃ�丮�� �� ����
        curScene = (int)curPlayerData.dataList[7].dataNumber; // ���� Ʃ�丮�� �� �ҷ�����

        if(curScene > 11) // Ʃ�丮���� �ƴ� �����ʹ� Ȱ��ȭ���� ����
        {
            scriptBox.ScriptBoxOnOff(false);
            if (tutorialFadePanal != null)  tutorialFadePanal.SetActive(false); // ���̴� �г�(���� �г�) ��Ȱ��ȭ
            return;
        }

        this.transform.GetChild(curScene).gameObject.SetActive(true); // Ʃ�丮�� �� Ȱ��ȭ
        // ù Ʃ�丮�� ���� ��� ���̵� �г� ����
        if (tutorialFadePanal!=null)
        {
            if ((curScene == 0)) // ù Ʃ�丮�� ���� ������ ���
            {
                tutorialFadePanal.SetActive(true); // ���̴� �г�(���� �г�) Ȱ��ȭ
            }
            else // �� ���� Ʃ�丮�� ������ �����ϴ� ���
            {
                tutorialFadePanal.SetActive(false); // ���̴� �г�(���� �г�) ��Ȱ��ȭ
            }
        }
    }

    // �� �ѹ� ���� �� �� ������Ʈ ������Ʈ �Լ�
    // : �� �ѹ��� ����� �� ����ȴ�.
    public void ChangeScene()
    {
        Debug.Log(curScene);       

        // ������ �ڽ����� Ȯ��
        if (this.transform.GetChild(curScene) == this.transform.GetChild(this.transform.childCount - 1))
        {
            // ���� �� ������Ʈ ��Ȱ��ȭ
            this.transform.GetChild(curScene).gameObject.SetActive(false);

            // ���� �� ������ ����
            curScene++;
        }
        else
        {

            // ���� �� ������Ʈ ��Ȱ��ȭ
            this.transform.GetChild(curScene).gameObject.SetActive(false);

            // ���� �� ������Ʈ Ȱ��ȭ
            curScene++;
            if (this.transform.GetChild(curScene).gameObject != null)
            {
                this.transform.GetChild(curScene).gameObject.SetActive(true);
            }
        }
        
        
        // �� ������ ������Ʈ
        curPlayerData.dataList[7].dataNumber = curScene;
        GameManager.instance.GetComponent<PlayerDataJSON>().DataSaveText(curPlayerData);
    }
}
