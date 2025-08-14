using UnityEngine;

public class ScoreOrbMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float destroyHeight = -10f;

    [Header("Visual Effects")]
    public float rotationSpeed = 90f; // سرعت چرخش
    public bool enablePulse = true;
    public float pulseSpeed = 2f;
    public float pulseScale = 0.2f;

    private Vector3 originalScale;
    private float pulseTimer = 0f;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        // حرکت گوی امتیاز به سمت پایین
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        // چرخش گوی امتیاز
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        // افکت نبض (پульس)
        if (enablePulse)
        {
            pulseTimer += Time.deltaTime * pulseSpeed;
            float scaleMultiplier = 1f + Mathf.Sin(pulseTimer) * pulseScale;
            transform.localScale = originalScale * scaleMultiplier;
        }

        // بررسی برای حذف گوی امتیاز
        if (transform.position.y <= destroyHeight)
        {
            Debug.Log("Score Orb destroyed at: " + transform.position);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // بررسی برخورد با پلیر
        if (other.CompareTag("Player"))
        {
            Debug.Log("SCORE! Player collected orb!");

            // اضافه کردن امتیاز از ScoreManager
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScoreForOrb();
            }

            // حذف گوی امتیاز بعد از جمع‌آوری
            Destroy(gameObject);
        }
    }

    // افکت ویژوال برای جمع‌آوری (اختیاری)
    void OnDestroy()
    {
        // اینجا می‌تونید افکت انفجار یا درخشش اضافه کنید
        // مثلاً پارتیکل سیستم
    }
}