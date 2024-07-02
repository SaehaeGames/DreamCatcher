using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUIManager : MonoBehaviour
{
    private GameSceneManager _gameSceneManager;
    [SerializeField] Button startBtn;

    private void OnEnable()
    {
        _gameSceneManager = GameSceneManager.Instance;
    }
    // Start is called before the first frame update
    void Start()
    {
        startBtn.onClick.AddListener(() => _gameSceneManager.ChangeSceneState(SceneState.Main));
    }
}
