using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("UI References")]
    public Text finalScoreText;
    public Text highScoreText;
    public Text gameOverTitle;
    public Button restartButton;
    public Button quitButton;

    [Header("UI Animation")]
    public bool enableAnimation = true;
    public float animationDuration = 0.5f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    void Awake()
    {
        // دریافت یا اضافه کردن CanvasGroup برای انیمیشن
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        rectTransform = GetComponent<RectTransform>();

        // تنظیم دکمه‌ها
        SetupButtons();
    }

    void OnEnable()
    {
        // انیمیشن ورود
        if (enableAnimation)
        {
            StartCoroutine(AnimateIn());
        }

        // بروزرسانی امتیازات
        UpdateScores();
    }

    void SetupButtons()
    {
        // تنظیم دکمه Restart
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        // تنظیم دکمه Quit
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(OnQuitClicked);
        }
    }

    void UpdateScores()
    {
        if (ScoreManager.Instance != null)
        {
            SetFinalScore(ScoreManager.Instance.GetCurrentScore());

            if (highScoreText != null)
            {
                highScoreText.text = "Best: " + ScoreManager.Instance.GetHighScore().ToString();
            }
        }
    }

    public void SetFinalScore(int score)
    {
        if (finalScoreText != null)
        {
            finalScoreText.text = "Score: " + score.ToString();

            // اگر رکورد جدید است
            if (ScoreManager.Instance != null && score >= ScoreManager.Instance.GetHighScore())
            {
                finalScoreText.color = Color.yellow;

                if (gameOverTitle != null)
                {
                    gameOverTitle.text = "NEW RECORD!";
                    gameOverTitle.color = Color.yellow;
                }
            }
            else
            {
                finalScoreText.color = Color.white;

                if (gameOverTitle != null)
                {
                    gameOverTitle.text = "GAME OVER";
                    gameOverTitle.color = Color.red;
                }
            }
        }
    }

    System.Collections.IEnumerator AnimateIn()
    {
        // تنظیمات اولیه انیمیشن
        canvasGroup.alpha = 0f;
        if (rectTransform != null)
        {
            rectTransform.localScale = Vector3.zero;
        }

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            float progress = elapsedTime / animationDuration;
            float curveValue = animationCurve.Evaluate(progress);

            // انیمیشن شفافیت
            canvasGroup.alpha = curveValue;

            // انیمیشن اسکیل
            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.one * curveValue;
            }

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // اطمینان از مقادیر نهایی
        canvasGroup.alpha = 1f;
        if (rectTransform != null)
        {
            rectTransform.localScale = Vector3.one;
        }
    }

    void OnRestartClicked()
    {
        Debug.Log("Restart button clicked");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }

    void OnQuitClicked()
    {
        Debug.Log("Quit button clicked");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
    }

    // متد برای بستن پنل (اختیاری)
    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    // متد برای نمایش پنل
    public void ShowPanel()
    {
        gameObject.SetActive(true);
    }
}