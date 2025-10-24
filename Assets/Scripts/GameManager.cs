using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Paneles UI (Asignar en Inspector)")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    private bool isGameOver = false;

    private void Start()
    {
        Time.timeScale = 1f;
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        isGameOver = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void HandleWin()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("¡HAS GANADO!");
        winPanel.SetActive(true);
        PauseGameAndShowCursor();
    }

    public void HandleLose()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("¡HAS PERDIDO! La luz se extinguió.");
        losePanel.SetActive(true);
        PauseGameAndShowCursor();
    }

    private void PauseGameAndShowCursor()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void RestartLevel()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}