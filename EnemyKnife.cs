using System.Collections;
using UnityEngine;

public class EnemyKnife : MonoBehaviour
{
    [Header("Setări atac")]
    public int damage = 25;
    public float attackDuration = 0.3f;

    [Header("Referințe")]
    public Collider knifeCollider;
    public Animator animator;
    public ParticleSystem slashEffect;
    public AudioSource audioSource;
    public AudioClip attackClip;

    private void Start()
    {
        if (knifeCollider != null)
            knifeCollider.enabled = false;
    }

    public IEnumerator DoAttack()
    {
        // animație
        animator?.SetTrigger("Attack");

        // efect slash
        if (slashEffect != null)
        {
            slashEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            slashEffect.Play();
        }

        // sunet
        if (audioSource != null && attackClip != null)
            audioSource.PlayOneShot(attackClip);

        // damage prin collider activ
        if (knifeCollider != null)
            knifeCollider.enabled = true;

        yield return new WaitForSeconds(attackDuration);

        if (knifeCollider != null)
            knifeCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }
}