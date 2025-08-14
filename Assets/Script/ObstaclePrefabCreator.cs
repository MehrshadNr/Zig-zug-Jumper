using UnityEngine;

public class ObstaclePrefabCreator : MonoBehaviour
{
    [Header("Obstacle Types")]
    public ObstacleType obstacleType = ObstacleType.Cube;

    [Header("Obstacle Settings")]
    public Vector3 obstacleSize = new Vector3(1f, 1f, 1f);
    public Color obstacleColor = Color.red;
    public bool isTrigger = false;

    public enum ObstacleType
    {
        Cube,
        Sphere,
        Cylinder,
        Custom
    }

    void Start()
    {
        if (Application.isPlaying)
        {
            // فقط برای نمایش - در زمان اجرا کاری انجام نمی‌دهد
            return;
        }
    }

    [ContextMenu("Create Obstacle Prefab")]
    public GameObject CreateObstaclePrefab()
    {
        GameObject obstacle = null;

        switch (obstacleType)
        {
            case ObstacleType.Cube:
                obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
                break;
            case ObstacleType.Sphere:
                obstacle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;
            case ObstacleType.Cylinder:
                obstacle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                break;
        }

        if (obstacle != null)
        {
            SetupObstacle(obstacle);
        }

        return obstacle;
    }

    void SetupObstacle(GameObject obstacle)
    {
        // نام گذاری
        obstacle.name = obstacleType.ToString() + " Obstacle";

        // تنظیم اندازه
        obstacle.transform.localScale = obstacleSize;

        // حذف Collider سه‌بعدی
        Collider oldCollider = obstacle.GetComponent<Collider>();
        if (oldCollider != null)
        {
            DestroyImmediate(oldCollider);
        }

        // اضافه کردن Collider2D
        BoxCollider2D collider2D = obstacle.AddComponent<BoxCollider2D>();
        collider2D.isTrigger = isTrigger;

        // تنظیم رنگ
        Renderer renderer = obstacle.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = obstacleColor;
        }

        // اضافه کردن اسکریپت حرکت
        obstacle.AddComponent<ObstacleMovement>();

        // تنظیم تگ
        obstacle.tag = "Obstacle";

        Debug.Log("Obstacle prefab created: " + obstacle.name);
    }

    // متدهای کمکی برای ایجاد انواع مختلف موانع
    public GameObject CreateCubeObstacle()
    {
        obstacleType = ObstacleType.Cube;
        return CreateObstaclePrefab();
    }

    public GameObject CreateSphereObstacle()
    {
        obstacleType = ObstacleType.Sphere;
        return CreateObstaclePrefab();
    }

    public GameObject CreateCylinderObstacle()
    {
        obstacleType = ObstacleType.Cylinder;
        return CreateObstaclePrefab();
    }
}