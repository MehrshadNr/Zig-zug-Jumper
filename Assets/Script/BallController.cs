using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Wall Positions")]
    public float leftWallX = -3f;
    public float rightWallX = 3f;

    [Header("Audio")]
    public AudioClip jumpSound;

    private AudioSource audioSource;
    private bool isOnLeftWall = true;
    private bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float moveProgress = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // غیرفعال کردن فیزیک جاذبه
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.isKinematic = true; // غیرفعال کردن فیزیک
        }

        // قرار دادن کاراکتر روی دیوار چپ در ابتدا
        transform.position = new Vector3(leftWallX, transform.position.y, transform.position.z);
    }

    void Update()
    {
        HandleInput();
        HandleMovement();
    }

    void HandleInput()
    {
        // فقط وقتی در حال حرکت نیست
        if (isMoving) return;

        // برای موبایل - تشخیص تاچ
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Jump();
        }

        // برای تست در ادیتور - کلیک موس
        if (Input.GetMouseButtonDown(0))
        {
            Jump();
        }

        // کلید اسپیس برای تست
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void Jump()
    {
        if (isMoving) return;

        // پخش صدای جهش
        if (audioSource && jumpSound)
        {
            audioSource.PlayOneShot(jumpSound);
        }

        // تنظیم مقصد
        startPosition = transform.position;

        if (isOnLeftWall)
        {
            // پرش از دیوار چپ به راست
            targetPosition = new Vector3(rightWallX, transform.position.y, transform.position.z);
            isOnLeftWall = false;
        }
        else
        {
            // پرش از دیوار راست به چپ
            targetPosition = new Vector3(leftWallX, transform.position.y, transform.position.z);
            isOnLeftWall = true;
        }

        // شروع حرکت
        isMoving = true;
        moveProgress = 0f;
    }

    void HandleMovement()
    {
        if (!isMoving) return;

        // پیشرفت حرکت
        moveProgress += Time.deltaTime * moveSpeed;

        if (moveProgress >= 1f)
        {
            // پایان حرکت
            moveProgress = 1f;
            isMoving = false;
        }

        // محاسبه موقعیت با انیمیشن نرم
        float curveValue = moveCurve.Evaluate(moveProgress);
        transform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);
    }

    // برای نمایش خطوط راهنما در Scene View
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // خط دیوار چپ
        Gizmos.DrawLine(new Vector3(leftWallX, -10, 0), new Vector3(leftWallX, 10, 0));

        // خط دیوار راست
        Gizmos.DrawLine(new Vector3(rightWallX, -10, 0), new Vector3(rightWallX, 10, 0));

        // نمایش مسیر حرکت اگر در حال حرکت است
        if (isMoving)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startPosition, targetPosition);
        }
    }
}