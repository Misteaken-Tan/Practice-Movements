using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Collectibles")]
    public int coins = 0;
    public int targetCoins = 20; // Win condition target
    public TMP_Text coinText;

    [Header("UI Panels")]
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public GameObject victoryMenu; // Victory panel reference

    private bool isPaused = false;
    private bool isGameOver = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateCoinUI();
        Time.timeScale = 1f;

        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (gameOverMenu != null) gameOverMenu.SetActive(false);
        if (victoryMenu != null) victoryMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGameOver) return;

            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoinUI();

        // Check win condition
        if (coins >= targetCoins && !isGameOver)
        {
            Victory();
        }
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = "Coins: " + coins.ToString() + " / " + targetCoins.ToString();
    }

    // --- Win Condition ---
    public void Victory()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        if (victoryMenu != null)
            victoryMenu.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // --- Pause Menu ---
    public void Pause()
    {
        isPaused = true;
        if (pauseMenu != null) pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        isPaused = false;
        if (pauseMenu != null) pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        if (gameOverMenu != null) gameOverMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}