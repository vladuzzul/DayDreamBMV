using System.Collections;
using UnityEngine;
using TMPro;

public class Knife : MonoBehaviour
{
    public float attackRange = 2f;       // Distanta la care poti ataca
    public LayerMask hitMask;            // Ce poate fi lovit
    public int damage = 25;              // Damage-ul cuțitului
    public float attackRate = 1f;        // Cât de rapid se poate ataca
    private float lastAttackTime = 0f;

    public Camera cam;

    [Header("Effects")]
    public ParticleSystem slashEffect;
    public AudioSource audioSource;
    public AudioClip attackClip;

    [Header("Animations")]
    public Animator animator;

    [Header("UI")]
    public TMP_Text crosshair_hit;

    [Header("Knife Positions")]
    public Transform weaponHolder;
    public Transform idlePosition;
    public Transform attackPosition;

    [Header("Settings")]
    public float attackSpeed = 10f;

    private bool isAttacking = false;

    void Start()
    {
        if (crosshair_hit != null)
            crosshair_hit.gameObject.SetActive(false);
    }

    void Update()
    {
        HandleAttackInput();
        UpdateWeaponPosition();
    }

    private void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    private void UpdateWeaponPosition()
    {
        Transform targetPos = isAttacking ? attackPosition : idlePosition;
        weaponHolder.position = Vector3.Lerp(weaponHolder.position, targetPos.position, Time.deltaTime * attackSpeed);
        weaponHolder.rotation = Quaternion.Lerp(weaponHolder.rotation, targetPos.rotation, Time.deltaTime * attackSpeed);
    }

    private void Attack()
    {
        if (Time.time < lastAttackTime + attackRate)
            return;

        lastAttackTime = Time.time;
        isAttacking = true;

        animator?.Play("Attack");

        if (slashEffect != null)
        {
            slashEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            slashEffect.Play();
        }

        if (audioSource != null && attackClip != null)
            audioSource.PlayOneShot(attackClip);

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, attackRange, hitMask))
        {
            // Atingere detectata
            if (crosshair_hit != null)
            {
                crosshair_hit.gameObject.SetActive(true);
                StartCoroutine(HideCrosshairHit());
            }

            // Aplica damage
            /*var health = hit.collider.GetComponent<Health>(); // Presupunem ca exista un script Health
            if (health != null)
                health.TakeDamage(damage);*/
        }

        StartCoroutine(ResetAttack());
    }

    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }

    private IEnumerator HideCrosshairHit()
    {
        yield return new WaitForSeconds(0.2f); 
        if (crosshair_hit != null)
            crosshair_hit.gameObject.SetActive(false);
    }
}
