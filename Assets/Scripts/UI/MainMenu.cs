using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Canvas References")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject optionsMenuCanvas;

    [Header("Audio")]
    [SerializeField] private AudioListener audioListener;

    private bool isMuted = false;

    private void Start()
    {
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(true);
        if (optionsMenuCanvas != null) optionsMenuCanvas.SetActive(false);
    }
    public void OnStartGame()
    {
        SceneManager.LoadScene("Level1");
    }
    public void OnOptions()
    {
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
        if (optionsMenuCanvas != null) optionsMenuCanvas.SetActive(true);
    }
    public void OnQuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    public void OnBackToMainMenu()
    {
        if (optionsMenuCanvas != null) optionsMenuCanvas.SetActive(false);
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(true);
    }
    public void OnToggleWindowedMode()
    {
        if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow || Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
    }
    public void OnToggleMute()
    {
        isMuted = !isMuted;
        if (audioListener != null)
        {
            audioListener.enabled = !isMuted;
        }
    }
}