using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game State")]
    public bool isGameActive = true;
    public bool isGameOver = false;

    [Header("UI References")]
    public GameObject gameOverPanel;
    public GameObject gameUI;

    [Header("Game Over Settings")]
    public float gameOverDelay = 1f;

    // Singleton Pattern
    public static GameManager Instance;

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // مطمئن شویم که بازی فعال است
        isGameActive = true;
        isGameOver = false;
        Time.timeScale = 1f;

        // فعال کردن UI بازی و غیرفعال کردن Game Over panel
        if (gameUI != null)
            gameUI.SetActive(true);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void GameOver()
    {
        if (isGameOver) return; // جلوگیری از فراخوانی مکرر

        Debug.Log("GAME OVER!");

        isGameActive = false;
        isGameOver = true;

        // نمایش Game Over panel با تاخیر
        Invoke("ShowGameOverPanel", gameOverDelay);
    }

    void ShowGameOverPanel()
    {
        // توقف زمان بازی
        Time.timeScale = 0f;

        // نمایش پنل Game Over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // مخفی کردن UI بازی
        if (gameUI != null)
        {
            gameUI.SetActive(false);
        }

        // نمایش امتیاز نهایی در Game Over panel
        UpdateGameOverScore();
    }

    void UpdateGameOverScore()
    {
        if (ScoreManager.Instance != null)
        {
            // اگر Game Over panel امتیاز نهایی داشته باشد
            GameOverUI gameOverUI = gameOverPanel.GetComponent<GameOverUI>();
            if (gameOverUI != null)
            {
                gameOverUI.SetFinalScore(ScoreManager.Instance.GetCurrentScore());
            }
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restarting Game...");

        // بازگرداندن زمان به حالت عادی
        Time.timeScale = 1f;

        // ریست امتیاز
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }

        // بارگذاری مجدد صحنه
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public bool IsGameActive()
    {
        return isGameActive && !isGameOver;
    }

    // متد برای pause/resume بازی
    public void PauseGame()
    {
        Time.timeScale = 0f;
        isGameActive = false;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isGameActive = true;
    }
}