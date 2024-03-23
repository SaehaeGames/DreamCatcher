using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BottomBar : MonoBehaviour
{
    public GameObject[] Selects;
    public GameObject[] UnSelects;

    public void ResetCategory()
    {
        for (int i = 0; i < 4; i++)
        {
            UnSelects[i].gameObject.SetActive(true);
            Selects[i].gameObject.SetActive(false);
        }
    }

    public void SetActiveCategory()
    {
        ResetCategory();
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "Main") 
        {
            Selects[0].gameObject.SetActive(true);
            UnSelects[0].gameObject.SetActive(false);
        }
        else if (scene.name == "Making")
        {
            Selects[1].gameObject.SetActive(true);
            UnSelects[1].gameObject.SetActive(false);
        }
        else if (scene.name == "Store")
        {
            Selects[3].gameObject.SetActive(true);
            UnSelects[3].gameObject.SetActive(false);
        }
        else
        {
            Selects[2].gameObject.SetActive(true);
            UnSelects[2].gameObject.SetActive(false);
        }
    }
}
