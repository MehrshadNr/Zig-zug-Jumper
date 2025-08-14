using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float destroyHeight = -10f;

    void Update()
    {
        // حرکت مانع به سمت پایین
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        // بررسی برای حذف مانع
        if (transform.position.y <= destroyHeight)
        {
            Debug.Log("Obstacle destroyed at: " + transform.position);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // بررسی برخورد با پلیر
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("GAME OVER! Player hit obstacle!");

            // فراخوانی Game Over از GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // اگر Collider روی Trigger تنظیم باشد
        if (other.CompareTag("Player"))
        {
            Debug.Log("GAME OVER! Player hit obstacle!");

            // فراخوانی Game Over از GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }
}