using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 100000000f; // Пуля живет 3 секунды

    private void Start()
    {
        // Уничтожаем пулю через 3 секунды
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Уничтожить пулю при столкновении с чем-то
        // Можно добавить логику урона или эффекта
        if (collision.gameObject.CompareTag("Player"))
        {
            // Тут можно добавить код для нанесения урона игроку
            Debug.Log("Player hit!");
        }

        // Если пуля столкнулась с чем-то, уничтожаем ее
        Destroy(gameObject);
    }
}
