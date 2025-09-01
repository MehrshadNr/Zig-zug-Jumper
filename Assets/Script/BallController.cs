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

    [Header("Mid-Air Control")]
    public bool allowMidAirDirectionChange = true;
    [Range(0.0f, 0.9f)]
    public float minProgressForDirectionChange = 0.1f; // حداقل پیشرفت برای تغییر مسیر
    [Range(0.1f, 1f)]
    public float directionChangeDelay = 0.2f; // تاخیر بین تغییرات مسیر

    private AudioSource audioSource;
    private bool isOnLeftWall = true;
    private bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float moveProgress = 0f;

    // متغیرهای جدید برای تغییر مسیر
    private float lastDirectionChangeTime = 0f;

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
        bool inputDetected = false;

        // برای موبایل - تشخیص تاچ
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            inputDetected = true;
        }

        // برای تست در ادیتور - کلیک موس
        if (Input.GetMouseButtonDown(0))
        {
            inputDetected = true;
        }

        // کلید اسپیس برای تست
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inputDetected = true;
        }

        if (inputDetected)
        {
            if (!isMoving)
            {
                // حرکت معمولی
                Jump();
            }
            else if (allowMidAirDirectionChange && CanChangeDirectionNow())
            {
                // تغییر مسیر در حین حرکت
                ChangeDirectionMidAir();
            }
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
        lastDirectionChangeTime = 0f; // ریست تایمر تغییر مسیر

        Debug.Log("Jump started from " + (isOnLeftWall ? "Right to Left" : "Left to Right"));
    }

    bool CanChangeDirectionNow()
    {
        // بررسی حداقل پیشرفت
        if (moveProgress < minProgressForDirectionChange) return false;

        // بررسی تاخیر بین تغییرات
        if (Time.time - lastDirectionChangeTime < directionChangeDelay) return false;

        return true;
    }

    void ChangeDirectionMidAir()
    {
        // پخش صدای جهش برای تغییر مسیر
        if (audioSource && jumpSound)
        {
            audioSource.PlayOneShot(jumpSound);
        }

        // موقعیت فعلی پلیر
        Vector3 currentPosition = transform.position;

        // تعیین مقصد جدید (دیوار مقابل)
        Vector3 newTarget;
        if (isOnLeftWall)
        {
            // اگر در حال حرکت به سمت چپ است، برو به راست
            newTarget = new Vector3(rightWallX, transform.position.y, transform.position.z);
            isOnLeftWall = false;
        }
        else
        {
            // اگر در حال حرکت به سمت راست است، برو به چپ
            newTarget = new Vector3(leftWallX, transform.position.y, transform.position.z);
            isOnLeftWall = true;
        }

        // تنظیم موقعیت شروع جدید به موقعیت فعلی
        startPosition = currentPosition;
        targetPosition = newTarget;

        // ریست پیشرفت برای حرکت جدید
        moveProgress = 0f;

        // ثبت زمان تغییر مسیر
        lastDirectionChangeTime = Time.time;

        Debug.Log("Direction changed mid-air! New target: " + (isOnLeftWall ? "Left" : "Right") + " wall");
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

            // اطمینان از قرار گیری دقیق روی دیوار
            if (isOnLeftWall)
            {
                transform.position = new Vector3(leftWallX, transform.position.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(rightWallX, transform.position.y, transform.position.z);
            }

            Debug.Log("Movement completed. Now on " + (isOnLeftWall ? "LEFT" : "RIGHT") + " wall");
            return;
        }

        // محاسبه موقعیت با انیمیشن نرم
        float curveValue = moveCurve.Evaluate(moveProgress);
        transform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);
    }

    // متدهای کمکی برای debugging و UI
    public bool IsMoving()
    {
        return isMoving;
    }

    public bool CanChangeDirection()
    {
        return isMoving && allowMidAirDirectionChange && CanChangeDirectionNow();
    }

    public float GetMoveProgress()
    {
        return moveProgress;
    }

    public bool IsOnLeftWall()
    {
        return isOnLeftWall;
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

            // نمایش موقعیت فعلی
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.3f);

            // نمایش نقطه‌ای که می‌تواند مسیر را تغییر دهد
            if (allowMidAirDirectionChange && CanChangeDirectionNow())
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, 0.5f);
            }
        }

        // نمایش محدوده تغییر مسیر
        if (allowMidAirDirectionChange && Application.isPlaying && isMoving)
        {
            float minDistance = Vector3.Distance(startPosition, targetPosition) * minProgressForDirectionChange;
            Vector3 minPoint = Vector3.Lerp(startPosition, targetPosition, minProgressForDirectionChange);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(minPoint, 0.2f);
        }
    }
}