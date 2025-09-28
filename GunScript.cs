using System.Collections;
using UnityEngine;
using TMPro;

public class GunScript : MonoBehaviour
{
    private PlayerStatus playerStatus;
    public float shootDistance = 100f;
    public LayerMask hitMask;
    public float fireRate = 1f;
    public int damage;
    public int maxAmmo = 12;
    public bool fullAuto;
    public float reloadTime = 2f;
    private bool isAiming;

    public Camera cam;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public AudioSource audioSource;
    public AudioClip shootClip;

    [Header("Animations")]
    public Animator animator;

    private float lastShot = 0f;
    [HideInInspector] public int currentAmmo;
    private bool reloading = false;

    [Header("UI")]
    public TextMeshProUGUI ammoText;
    public TMP_Text crosshair_hit; // dot crosshair

    [Header("Gun Positions")]
    public Transform weaponHolder;
    public Transform hipPosition;
    public Transform aimPosition;

    [Header("Camera Settings")]
    public float normalFOV = 60f;
    public float aimFOV = 40f;

    [Header("Settings")]
    public float aimSpeed = 10f;
    public float zoomSpeed = 10f;

    void Start()
    {
        currentAmmo = maxAmmo;
        if (crosshair_hit != null)
            crosshair_hit.gameObject.SetActive(false);
        playerStatus = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStatus>();
    }

    void Update()
    {
        HandleShootingInput();
        HandleAiming();
        HandleReloading();
        UpdateAmmoUI();
        UpdateWeaponPosition();
        UpdateCameraFOV();
    }

    private void HandleShootingInput()
    {
        if (!fullAuto)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
        }
        else if (Input.GetMouseButton(0))
        {
            Shoot();
        }
    }

    private void HandleAiming()
    {
        bool allowedToAim = playerStatus == null ? true : playerStatus.canAim;
        isAiming = allowedToAim && Input.GetMouseButton(1) && !reloading;
    }
    
    public void SetAmmoImmediate(int ammo)
    {
        currentAmmo = Mathf.Clamp(ammo, 0, maxAmmo);
        UpdateAmmoUI();
    }

    private void HandleReloading()
    {
        if (Input.GetKeyDown(KeyCode.R) && !reloading && currentAmmo != maxAmmo)
        {
            StartCoroutine(Reload());
        }
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = currentAmmo + "/" + maxAmmo;
    }

    private void UpdateWeaponPosition()
    {
        Transform targetPos = isAiming ? aimPosition : hipPosition;
        weaponHolder.position = Vector3.Lerp(weaponHolder.position, targetPos.position, Time.deltaTime * aimSpeed);
        weaponHolder.rotation = Quaternion.Lerp(weaponHolder.rotation, targetPos.rotation, Time.deltaTime * aimSpeed);
    }

    private void UpdateCameraFOV()
    {
        float targetFOV = isAiming ? aimFOV : normalFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }

    private void Shoot()
    {
        if (!(Time.time > lastShot + fireRate) || currentAmmo <= 0 || reloading)
            return;

        lastShot = Time.time;
        currentAmmo--;

        // 🔥 Efecte tragere
        if (muzzleFlash != null)
        {
            muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            muzzleFlash.Play();
        }

        if (audioSource != null && shootClip != null)
            audioSource.PlayOneShot(shootClip);

        animator?.Play("Shoot");

        // 🎯 Raycast pentru hit detection
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, shootDistance, hitMask))
        {

            
            Debug.Log("Lovit " + hit.collider.name);
            // verificăm și părintele collider-ului lovit
            EnemyHealth enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
            {
                if (crosshair_hit != null)
                {
                    crosshair_hit.gameObject.SetActive(true);
                    StartCoroutine(HideCrosshairHit());
                }
                Debug.Log("Lovit " + hit.collider.name + " -> scad " + damage + " HP");
                enemyHealth.TakeDamage(damage);
            }
        }

        else
        {
            if (crosshair_hit != null)
                crosshair_hit.gameObject.SetActive(false);
        }
    }


    private IEnumerator HideCrosshairHit()
    {
        yield return new WaitForSeconds(0.2f); 
        if (crosshair_hit != null)
            crosshair_hit.gameObject.SetActive(false);
    }


    private IEnumerator Reload()
    {
        reloading = true;
        animator?.SetBool("Reloading", true);
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        animator?.SetBool("Reloading", false);
        reloading = false;
    }
}
