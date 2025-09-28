using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private EnemyHealth enemyHealth;

    [Header("Movement")]
    public float stoppingDistance = 2f;

    [Header("Attack")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float attackDuration = 0.3f;
    public Collider knifeCollider;
    public int damage = 25;

    [Header("Rotation")]
    public float lookSpeed = 5f;
    public float rotationOffset = 90f; // ↔ aici reglezi în Inspector (ex. 90, -90 sau 180)

    private float lastAttackTime = -999f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            agent.stoppingDistance = stoppingDistance;
        }
        else
        {
            Debug.LogWarning("Player not found in scene!");
        }

        if (knifeCollider != null)
            knifeCollider.enabled = false; // dezactivăm cuțitul la start
    }

    void Update()
    {
        if (player == null || enemyHealth == null) return;
        if (enemyHealth.IsDead) {
            return; // nu mai face nimic dacă e mort
}

        // 🔹 urmărește playerul
        agent.SetDestination(player.position);

        // 🔹 rotire spre player
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0;

        if (lookPos != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookPos) * Quaternion.Euler(0, rotationOffset, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lookSpeed);
        }

        // 🔹 atac
        float sqrDist = (player.position - transform.position).sqrMagnitude;
        if (sqrDist <= attackRange * attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            StartCoroutine(DoAttack());
        }
    }

    private IEnumerator DoAttack()
    {
        lastAttackTime = Time.time;

        // TODO: poți adăuga aici animație
        // animator?.SetTrigger("Attack");

        // activăm collider-ul cuțitului pentru o fracțiune de secundă
        if (knifeCollider != null)
            knifeCollider.enabled = true;

        yield return new WaitForSeconds(attackDuration);

        if (knifeCollider != null)
            knifeCollider.enabled = false;
    }
}
