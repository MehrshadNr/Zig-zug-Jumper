using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject obstaclePrefab;
    public float spawnInterval = 2f; // فاصله زمانی بین تولید موانع
    public float spawnHeight = 10f; // ارتفاع تولید موانع

    [Header("Obstacle Movement")]
    public float moveSpeed = 5f; // سرعت حرکت موانع
    public float destroyHeight = -10f; // ارتفاع حذف موانع

    [Header("Spawn Position Range")]
    public float minX = -2f; // حداقل موقعیت X
    public float maxX = 2f; // حداکثر موقعیت X

    [Header("Score Orb Integration")]
    public ScoreOrbSpawner scoreOrbSpawner; // ارجاع به ScoreOrbSpawner

    private float nextSpawnTime;

    void Start()
    {
        // تنظیم زمان اولین تولید
        nextSpawnTime = Time.time + spawnInterval;

        // اگر Prefab تنظیم نشده، یک Cube ایجاد می‌کنیم
        if (obstaclePrefab == null)
        {
            CreateDefaultObstaclePrefab();
        }

        // اگر ScoreOrbSpawner تنظیم نشده، سعی کن پیداش کنی
        if (scoreOrbSpawner == null)
        {
            scoreOrbSpawner = FindObjectOfType<ScoreOrbSpawner>();
        }
    }

    void Update()
    {
        // بررسی زمان تولید مانع جدید
        if (Time.time >= nextSpawnTime)
        {
            SpawnObstacle();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnObstacle()
    {
        if (obstaclePrefab == null) return;

        // موقعیت تصادفی روی محور X
        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, 0);

        // ایجاد مانع
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);

        // اضافه کردن کامپوننت حرکت
        ObstacleMovement movement = obstacle.GetComponent<ObstacleMovement>();
        if (movement == null)
        {
            movement = obstacle.AddComponent<ObstacleMovement>();
        }

        // تنظیم پارامترهای حرکت
        movement.moveSpeed = moveSpeed;
        movement.destroyHeight = destroyHeight;

        // اطمینان از وجود Collider
        if (obstacle.GetComponent<Collider2D>() == null)
        {
            obstacle.AddComponent<BoxCollider2D>();
        }

        // تنظیم تگ
        obstacle.tag = "Obstacle";

        // اطلاع دادن به ScoreOrbSpawner که یک مانع اسپان شده
        if (scoreOrbSpawner != null)
        {
            scoreOrbSpawner.OnObstacleSpawned();
        }

        //Debug.Log("Obstacle spawned at: " + spawnPosition);
    }

    void CreateDefaultObstaclePrefab()
    {
        // ایجاد یک Cube پیش‌فرض
        GameObject defaultObstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        defaultObstacle.name = "Default Obstacle";

        // حذف Collider سه‌بعدی و اضافه کردن Collider دوبعدی
        Destroy(defaultObstacle.GetComponent<BoxCollider>());
        defaultObstacle.AddComponent<BoxCollider2D>();

        // تنظیم رنگ قرمز
        Renderer renderer = defaultObstacle.GetComponent<Renderer>();
        renderer.material.color = Color.red;

        // تبدیل به Prefab (در حافظه)
        obstaclePrefab = defaultObstacle;

        // غیرفعال کردن تا در صحنه نمایش داده نشود
        defaultObstacle.SetActive(false);
    }

    // برای نمایش محدوده تولید در Scene View
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // نمایش خط تولید
        Vector3 leftPoint = new Vector3(minX, spawnHeight, 0);
        Vector3 rightPoint = new Vector3(maxX, spawnHeight, 0);
        Gizmos.DrawLine(leftPoint, rightPoint);

        // نمایش خط حذف
        Gizmos.color = Color.red;
        Vector3 leftDestroy = new Vector3(minX, destroyHeight, 0);
        Vector3 rightDestroy = new Vector3(maxX, destroyHeight, 0);
        Gizmos.DrawLine(leftDestroy, rightDestroy);

        // نمایش محدوده
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3((minX + maxX) / 2, (spawnHeight + destroyHeight) / 2, 0),
                           new Vector3(maxX - minX, spawnHeight - destroyHeight, 1));
    }
}