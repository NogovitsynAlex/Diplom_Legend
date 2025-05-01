using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 30;
    private int currentHP;

    [Header("Patrol & Detection")]
    public Transform[] patrolPoints;
    public float detectionRadius = 50f;
    private int currentPatrolIndex = 0;
    private Transform player;
    private NavMeshAgent agent;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;
    public float fireRate = 2f;    // выстрелов в секунду
    public float reloadTime = 2f;  // Время на перезарядку после стрельбы

    private bool isShooting = false;

    private void Start()
    {
        currentHP = maxHP;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            agent.Warp(hit.position);

        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[0].position);
    }

    private void Update()
    {
        if (player == null || !agent.isOnNavMesh) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // Если игрок в радиусе обнаружения
        if (dist <= detectionRadius)
        {
            agent.SetDestination(player.position); // Преследуем игрока

            // Если враг не стреляет, запускаем стрельбу
            if (!isShooting)
            {
                StartCoroutine(AttackRoutine());
            }

            // Плавно поворачиваемся к игроку
            RotateTowards(player.position);
        }
        else
        {
            Patrol();  // Возвращаемся к патрулированию, если игрок не в радиусе
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    private IEnumerator AttackRoutine()
    {
        isShooting = true;
        agent.isStopped = true; // Останавливаем движение, чтобы начать стрелять

        // Периодическая стрельба
        while (Vector3.Distance(transform.position, player.position) <= detectionRadius)
        {
            Shoot();
            yield return new WaitForSeconds(1f / fireRate); // Ждем между выстрелами
        }

        // После того, как игрок выходит из зоны, продолжаем движение
        isShooting = false;
        agent.isStopped = false;
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Спавним пулю чуть вперёд, чтобы избежать пересечений
        Vector3 spawnPos = firePoint.position + firePoint.forward * 0.5f;
        var bullet = Instantiate(bulletPrefab, spawnPos, firePoint.rotation);

        // Игнорируем столкновения пули с самим врагом
        var bulletCol = bullet.GetComponent<Collider>();
        foreach (var col in GetComponentsInChildren<Collider>())
            if (bulletCol != null) Physics.IgnoreCollision(bulletCol, col);

        // Задаём скорость
        var rb = bullet.GetComponent<Rigidbody>();
        if (rb != null) rb.velocity = firePoint.forward * bulletForce;

        // Уничтожаем пулю через 5 секунд
        Destroy(bullet, 5f);
    }

    private void RotateTowards(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        Quaternion look = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * agent.angularSpeed);
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP <= 0) Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
