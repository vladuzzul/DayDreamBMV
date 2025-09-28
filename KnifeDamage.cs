using UnityEngine;

public class KnifeDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damage = 25;
    public float damageCooldown = 1f; // timp minim între două lovituri
    private float lastDamageTime = -999f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // verificăm dacă a trecut cooldown-ul
            if (Time.time - lastDamageTime >= damageCooldown)
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    lastDamageTime = Time.time;
                }
            }
        }
    }
}