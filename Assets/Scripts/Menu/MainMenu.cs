using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Level_01";
    [SerializeField] private int startingLives = 3;
    [SerializeField] private int startingCollectibles = 0;
    [SerializeField] private GameObject continueBtn;

    private void Start()
    {
        // Trigger audio manager here

        continueBtn.SetActive(SaveManager.HasKey(SaveManager.CurrentLevelKey));
    }
    private void Update()
    {
#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.C))
        {
            SaveManager.DeleteAll();
            continueBtn.SetActive(false);
        }
#endif
    }

    public void StartGame()
    {
        SaveManager.SaveString(SaveManager.CurrentLevelKey, gameSceneName);
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit requested");
    }

    public void Continue()
    {
        string savedLevel = SaveManager.LoadString(SaveManager.CurrentLevelKey, string.Empty);
        if (!string.IsNullOrEmpty(savedLevel))
        {
            SceneManager.LoadScene(savedLevel);
        }
        else
        {
            Debug.LogWarning("No saved continue level found.");
        }
    }
}
