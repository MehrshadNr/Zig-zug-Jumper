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

    [Header("Wall Positions (Side Spawn)")]
    public float leftWallX = -3f; // موقعیت دیوار چپ
    public float rightWallX = 3f; // موقعیت دیوار راست
    public bool spawnOnSides = true; // فعال/غیرفعال کردن اسپان روی دیوارها

    [Header("Random Center Spawn (Optional)")]
    public bool allowCenterSpawn = false; // اجازه اسپان در وسط
    public float minX = -2f; // حداقل موقعیت X برای اسپان وسط
    public float maxX = 2f; // حداکثر موقعیت X برای اسپان وسط
    [Range(0f, 1f)]
    public float centerSpawnChance = 0.2f; // احتمال اسپان در وسط

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

        Vector3 spawnPosition;

        if (spawnOnSides)
        {
            // تصمیم‌گیری برای اسپان در وسط یا کنار
            bool spawnInCenter = allowCenterSpawn && Random.Range(0f, 1f) < centerSpawnChance;

            if (spawnInCenter)
            {
                // اسپان در موقعیت تصادفی وسط
                float randomX = Random.Range(minX, maxX);
                spawnPosition = new Vector3(randomX, spawnHeight, 0);
            }
            else
            {
                // اسپان روی یکی از دیوارها به صورت رندوم
                bool spawnOnLeft = Random.Range(0, 2) == 0; // 50% احتمال چپ، 50% راست

                float xPosition = spawnOnLeft ? leftWallX : rightWallX;
                spawnPosition = new Vector3(xPosition, spawnHeight, 0);

                Debug.Log("Spawning obstacle on " + (spawnOnLeft ? "LEFT" : "RIGHT") + " wall at X: " + xPosition);
            }
        }
        else
        {
            // حالت قدیمی - اسپان در موقعیت تصادفی
            float randomX = Random.Range(minX, maxX);
            spawnPosition = new Vector3(randomX, spawnHeight, 0);
        }

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

        Debug.Log("Obstacle spawned at: " + spawnPosition);
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
        if (spawnOnSides)
        {
            // نمایش نقاط اسپان روی دیوارها
            Gizmos.color = Color.green;

            // دیوار چپ
            Vector3 leftSpawn = new Vector3(leftWallX, spawnHeight, 0);
            Gizmos.DrawWireCube(leftSpawn, Vector3.one);
            Gizmos.DrawLine(new Vector3(leftWallX, spawnHeight - 0.5f, 0),
                           new Vector3(leftWallX, spawnHeight + 0.5f, 0));

            // دیوار راست
            Vector3 rightSpawn = new Vector3(rightWallX, spawnHeight, 0);
            Gizmos.DrawWireCube(rightSpawn, Vector3.one);
            Gizmos.DrawLine(new Vector3(rightWallX, spawnHeight - 0.5f, 0),
                           new Vector3(rightWallX, spawnHeight + 0.5f, 0));

            // نمایش محدوده اسپان وسط (اگر فعال باشد)
            if (allowCenterSpawn)
            {
                Gizmos.color = Color.blue;
                Vector3 centerLeft = new Vector3(minX, spawnHeight, 0);
                Vector3 centerRight = new Vector3(maxX, spawnHeight, 0);
                Gizmos.DrawLine(centerLeft, centerRight);

                // نمایش درصد احتمال
                Gizmos.color = Color.cyan;
                for (float x = minX; x <= maxX; x += 0.5f)
                {
                    Gizmos.DrawWireSphere(new Vector3(x, spawnHeight, 0), 0.2f);
                }
            }
        }
        else
        {
            // حالت قدیمی
            Gizmos.color = Color.green;
            Vector3 leftPoint = new Vector3(minX, spawnHeight, 0);
            Vector3 rightPoint = new Vector3(maxX, spawnHeight, 0);
            Gizmos.DrawLine(leftPoint, rightPoint);
        }

        // نمایش خط حذف
        Gizmos.color = Color.red;
        Vector3 leftDestroy = new Vector3(leftWallX - 1, destroyHeight, 0);
        Vector3 rightDestroy = new Vector3(rightWallX + 1, destroyHeight, 0);
        Gizmos.DrawLine(leftDestroy, rightDestroy);

        // نمایش محدوده کلی
        Gizmos.color = Color.yellow;
        float totalWidth = rightWallX - leftWallX + 2;
        Gizmos.DrawWireCube(new Vector3((leftWallX + rightWallX) / 2, (spawnHeight + destroyHeight) / 2, 0),
                           new Vector3(totalWidth, spawnHeight - destroyHeight, 1));
    }
}