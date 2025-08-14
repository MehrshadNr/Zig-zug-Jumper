using UnityEngine;

public class ScoreOrbSpawner : MonoBehaviour
{
    [Header("Score Orb Settings")]
    public GameObject scoreOrbPrefab;
    public float orbSize = 0.5f;
    public Color orbColor = Color.yellow;
    private ObstacleSpawner obstacleSpawner;

    [Header("Spawn Settings")]
    public float spawnHeight = 10f;
    public float destroyHeight = -10f;
    public float moveSpeed = 5f;

    [Header("Position Range")]
    public float minX = -2f;
    public float maxX = 2f;

    private int obstacleCount = 0;
    private bool shouldSpawnOrb = false;

    void Start()
    {
        obstacleSpawner = GetComponent<ObstacleSpawner>();
        // اگر Prefab تنظیم نشده، یک گوی پیش‌فرض ایجاد می‌کنیم
        if (scoreOrbPrefab == null)
        {
            CreateDefaultScoreOrbPrefab();
        }
    }

    // این متد از ObstacleSpawner فراخوانی می‌شود
    public void OnObstacleSpawned()
    {
        obstacleCount++;

        // بعد از هر دو مانع، یک گوی امتیاز اسپان کن
        if (obstacleCount % 1 == 0)
        {
            shouldSpawnOrb = true;
            Invoke("SpawnScoreOrb", obstacleSpawner.spawnInterval/2); // با تاخیر 1 ثانیه اسپان کن
        }
    }

    void SpawnScoreOrb()
    {
        if (!shouldSpawnOrb || scoreOrbPrefab == null) return;

        // موقعیت تصادفی روی محور X
        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, 0);

        // ایجاد گوی امتیاز
        GameObject orb = Instantiate(scoreOrbPrefab, spawnPosition, Quaternion.identity);

        // اضافه کردن کامپوننت حرکت
        ScoreOrbMovement movement = orb.GetComponent<ScoreOrbMovement>();
        if (movement == null)
        {
            movement = orb.AddComponent<ScoreOrbMovement>();
        }

        // تنظیم پارامترهای حرکت
        movement.moveSpeed = moveSpeed;
        movement.destroyHeight = destroyHeight;

        // اطمینان از وجود Collider (Trigger)
        Collider2D collider = orb.GetComponent<Collider2D>();
        if (collider == null)
        {
            collider = orb.AddComponent<CircleCollider2D>();
        }
        collider.isTrigger = true; // تنظیم به Trigger

        // تنظیم تگ
        orb.tag = "ScoreOrb";

        shouldSpawnOrb = false;
        //Debug.Log("Score Orb spawned at: " + spawnPosition);
    }

    void CreateDefaultScoreOrbPrefab()
    {
        // ایجاد یک Sphere زرد
        GameObject defaultOrb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        defaultOrb.name = "Default Score Orb";

        // تنظیم اندازه
        defaultOrb.transform.localScale = Vector3.one * orbSize;

        // حذف Collider سه‌بعدی و اضافه کردن Collider دوبعدی
        Destroy(defaultOrb.GetComponent<SphereCollider>());
        CircleCollider2D circleCollider = defaultOrb.AddComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;

        // تنظیم رنگ زرد
        Renderer renderer = defaultOrb.GetComponent<Renderer>();
        renderer.material.color = orbColor;

        // تبدیل به Prefab (در حافظه)
        scoreOrbPrefab = defaultOrb;

        // غیرفعال کردن تا در صحنه نمایش داده نشود
        defaultOrb.SetActive(false);
    }

    // برای نمایش محدوده تولید در Scene View
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        // نمایش خط تولید گوی امتیاز
        Vector3 leftPoint = new Vector3(minX, spawnHeight, 0);
        Vector3 rightPoint = new Vector3(maxX, spawnHeight, 0);
        Gizmos.DrawLine(leftPoint, rightPoint);

        // نمایش نقاط تولید احتمالی
        for (float x = minX; x <= maxX; x += 0.5f)
        {
            Gizmos.DrawWireSphere(new Vector3(x, spawnHeight, 0), orbSize / 2);
        }
    }
}