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

    [Header("Audio Settings (Activity 6)")] //
    public AudioSource bgmSource;           // Drag your BGM GameObject's AudioSource here (Req 1)
    public AudioSource sfxSource;           // Drag an AudioSource for UI/SFX here
    public AudioClip coinPickupSound;       // Collectible SFX (Req 6)
    public AudioClip victorySound;          // Win SFX (Req 7)
    public AudioClip gameOverSound;         // Game Over SFX (Req 7)

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

        // Auto-assign SFX source if on the same GameObject
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();
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

        // --- Requirement 6: Collectible Sound Effect ---
        if (sfxSource != null && coinPickupSound != null) //[cite: 4]
        {
            sfxSource.PlayOneShot(coinPickupSound); //[cite: 4]
        }

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

    // --- Win Condition (Requirement 7) ---
    public void Victory() //[cite: 4]
    {
        isGameOver = true;
        Time.timeScale = 0f;

        // Stop background music and play Victory theme
        if (bgmSource != null) bgmSource.Stop(); //[cite: 4]
        if (sfxSource != null && victorySound != null) //[cite: 4]
        {
            sfxSource.ignoreListenerPause = true; // Ensures audio plays even if game is paused
            sfxSource.PlayOneShot(victorySound); //[cite: 4]
        }

        if (victoryMenu != null)
            victoryMenu.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // --- Game Over Condition (Requirement 7) ---
    public void GameOver() //[cite: 4]
    {
        isGameOver = true;
        Time.timeScale = 0f;

        // Stop background music and play Game Over sound
        if (bgmSource != null) bgmSource.Stop(); //[cite: 4]
        if (sfxSource != null && gameOverSound != null) //[cite: 4]
        {
            sfxSource.ignoreListenerPause = true;
            sfxSource.PlayOneShot(gameOverSound); //[cite: 4]
        }

        if (gameOverMenu != null)
            gameOverMenu.SetActive(true);

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

        if (bgmSource != null) bgmSource.Pause();
    }

    public void Resume()
    {
        isPaused = false;
        if (pauseMenu != null) pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (bgmSource != null) bgmSource.UnPause();
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
}