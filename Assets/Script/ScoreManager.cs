using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    public int scorePerOrb = 10;
    public int currentScore = 0;
    public int highScore = 0;

    [Header("UI References")]
    public Text scoreText;
    public Text highScoreText;

    [Header("Score Effects")]
    public bool enableScoreAnimation = true;
    public float animationDuration = 0.3f;

    // Singleton Pattern
    public static ScoreManager Instance;

    // Animation variables
    private Vector3 originalScaleScore;
    private bool isAnimating = false;

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
        // بارگذاری High Score از PlayerPrefs
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        // ذخیره اندازه اصلی برای انیمیشن
        if (scoreText != null)
        {
            originalScaleScore = scoreText.transform.localScale;
        }

        // بروزرسانی UI
        UpdateScoreUI();
        UpdateHighScoreUI();

        // ریست امتیاز فعلی
        ResetScore();
    }

    public void AddScore(int points)
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameActive())
            return;

        currentScore += points;
        Debug.Log("Score Added: +" + points + " | Total: " + currentScore);

        // بروزرسانی UI
        UpdateScoreUI();

        // انیمیشن امتیاز
        if (enableScoreAnimation && !isAnimating)
        {
            StartCoroutine(AnimateScoreText());
        }

        // بررسی و بروزرسانی High Score
        CheckHighScore();
    }

    public void AddScoreForOrb()
    {
        AddScore(scorePerOrb);
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString();
        }
    }

    void UpdateHighScoreUI()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "Best: " + highScore.ToString();
        }
    }

    void CheckHighScore()
    {
        if (currentScore > highScore)
        {
            highScore = currentScore;

            // ذخیره High Score
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();

            // بروزرسانی UI
            UpdateHighScoreUI();

            Debug.Log("NEW HIGH SCORE: " + highScore);
        }
    }

    System.Collections.IEnumerator AnimateScoreText()
    {
        if (scoreText == null) yield break;

        isAnimating = true;

        // بزرگ کردن
        float elapsedTime = 0f;
        Vector3 targetScale = originalScaleScore * 1.2f;

        while (elapsedTime < animationDuration / 2)
        {
            float progress = elapsedTime / (animationDuration / 2);
            scoreText.transform.localScale = Vector3.Lerp(originalScaleScore, targetScale, progress);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // برگرداندن به اندازه اصلی
        elapsedTime = 0f;
        while (elapsedTime < animationDuration / 2)
        {
            float progress = elapsedTime / (animationDuration / 2);
            scoreText.transform.localScale = Vector3.Lerp(targetScale, originalScaleScore, progress);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        scoreText.transform.localScale = originalScaleScore;
        isAnimating = false;
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
        Debug.Log("Score Reset");
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public int GetHighScore()
    {
        return highScore;
    }

    // متد برای تنظیم مقدار امتیاز هر گوی
    public void SetScorePerOrb(int newScoreValue)
    {
        scorePerOrb = newScoreValue;
    }
}