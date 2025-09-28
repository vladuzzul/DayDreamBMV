using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    public bool IsDead { get; private set; } = false;

    [Header("UI")]
    public Slider healthBar; // slider UI deasupra inamicului

    private Animator animator;
    private NavMeshAgent agent;
    private Collider mainCollider;

    void Start()
    {
        currentHealth = maxHealth;

        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        mainCollider = GetComponent<Collider>();

        if (healthBar != null)
        {
            healthBar.minValue = 0;
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"{gameObject.name} HP = {currentHealth}");

        UpdateHealthUI();

        if (currentHealth <= 0)
            Die();
        else
            animator?.SetTrigger("Hit");
    }


    private void UpdateHealthUI()
    {
        if (healthBar != null)
            healthBar.value = currentHealth;
    }

    private void Die()
    {
        IsDead = true;

        if (agent != null) agent.enabled = false;
        if (mainCollider != null) mainCollider.enabled = false;

        animator?.SetTrigger("Die");

        // notify wave manager (if present)
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnEnemyKilled();
        }

        // destroy after 5 seconds
        Destroy(gameObject, 5f);
    }
}