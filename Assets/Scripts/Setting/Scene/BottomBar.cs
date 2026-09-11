using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BottomBar : MonoBehaviour
{
    public GameObject[] Selects;
    public GameObject[] UnSelects;

    private GameSceneManager _gameSceneManager;
    private UnityAction[] navigationListeners;
    private bool[] navigationEnabled;

    private readonly SceneState[] sceneStates =
    {
        SceneState.Main,
        SceneState.Making,
        SceneState.CollectionDream,
        SceneState.Store
    };

    public enum SceneName
    {
        Main,
        Making,
        CollectionDream,
        Store
    }

    private void Awake()
    {
        int menuCount = Mathf.Min(UnSelects.Length, sceneStates.Length);
        navigationListeners = new UnityAction[menuCount];
        navigationEnabled = new bool[menuCount];

        for (int i = 0; i < menuCount; i++)
        {
            int menu = i;
            navigationListeners[i] = () => ChangeScene(menu);
            navigationEnabled[i] = true;
        }
    }

    public void Start()
    {
        _gameSceneManager = GameSceneManager.Instance;
        OnClickSetting();
    }

    private void ChangeScene(int menu)
    {
        if (!IsValidMenu(menu) || !navigationEnabled[menu]) return;

        if (_gameSceneManager == null)
        {
            _gameSceneManager = GameSceneManager.Instance;
        }

        if (_gameSceneManager != null)
        {
            _gameSceneManager.ChangeSceneState(sceneStates[menu]);
        }
    }

    public void ResetCategory()
    {
        for (int i = 0; i < Selects.Length; i++)
        {
            UnSelects[i].gameObject.SetActive(true);
            Selects[i].gameObject.SetActive(false);
        }
    }

    public void SetActiveCategory()
    {
        ResetCategory();

        string sceneName = SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            case "Main":
                Selects[(int)SceneName.Main].SetActive(true);
                UnSelects[(int)SceneName.Main].SetActive(false);
                break;
            case "Making":
                Selects[(int)SceneName.Making].SetActive(true);
                UnSelects[(int)SceneName.Making].SetActive(false);
                break;
            case "Store":
                Selects[(int)SceneName.Store].SetActive(true);
                UnSelects[(int)SceneName.Store].SetActive(false);
                break;
            default:
                Selects[(int)SceneName.CollectionDream].SetActive(true);
                UnSelects[(int)SceneName.CollectionDream].SetActive(false);
                break;
        }
    }

    public void OnClickSetting()
    {
        if (navigationListeners == null) return;

        for (int i = 0; i < navigationListeners.Length; i++)
        {
            if (navigationEnabled[i])
            {
                AddNavigationListener(i);
            }
        }
    }

    public void onClickRemove(int menu)
    {
        if (!IsValidMenu(menu)) return;

        navigationEnabled[menu] = false;
    }

    public void OnClickAdd(int menu)
    {
        if (!IsValidMenu(menu)) return;

        navigationEnabled[menu] = true;
    }

    public bool TryGetMenuIndex(GameObject target, out int menu)
    {
        menu = -1;
        if (target == null || navigationListeners == null) return false;

        for (int i = 0; i < navigationListeners.Length; i++)
        {
            if (UnSelects[i] == null) continue;

            Transform menuTransform = UnSelects[i].transform;
            if (target == UnSelects[i] || target.transform.IsChildOf(menuTransform))
            {
                menu = i;
                return true;
            }
        }

        return false;
    }

    private void AddNavigationListener(int menu)
    {
        Button button = GetMenuButton(menu);
        if (button == null) return;

        button.onClick.RemoveListener(navigationListeners[menu]);
        button.onClick.AddListener(navigationListeners[menu]);
    }

    private Button GetMenuButton(int menu)
    {
        if (!IsValidMenu(menu) || UnSelects[menu] == null) return null;
        return UnSelects[menu].GetComponent<Button>();
    }

    private bool IsValidMenu(int menu)
    {
        return navigationListeners != null && menu >= 0 && menu < navigationListeners.Length;
    }

    private void OnDestroy()
    {
        if (navigationListeners == null) return;

        for (int i = 0; i < navigationListeners.Length; i++)
        {
            Button button = GetMenuButton(i);
            if (button != null)
            {
                button.onClick.RemoveListener(navigationListeners[i]);
            }
        }
    }
}
