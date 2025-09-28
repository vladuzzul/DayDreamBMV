using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Wave")]
    public int totalEnemiesThisWave = 5;
    private int enemiesAlive;

    [Header("UI")]
    public GameObject waveCompletePanel; // panel with text "Wave 1 completed"
    public TextMeshProUGUI waveCompleteText; // "Wave 1 completed"
    public TextMeshProUGUI hintText; // "Press Space to continue or E for sacrifice"
    public GameObject sacrificeMenuPanel; // panel with sacrifice options

    [Header("Weapons")]
    public GameObject arPrefab;       // Prefab-ul AR (cu scriptul propriu de gun)
    public Transform weaponSlot;      // WeaponHolder (slot-ul din player pentru arme)
    private GameObject currentWeapon;
    
    [Header("Settings")]
    public KeyCode continueKey = KeyCode.Space;
    public KeyCode openMenuKey = KeyCode.E;

    private bool waveFinished = false;
    
    

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        enemiesAlive = totalEnemiesThisWave;
        if (waveCompletePanel != null) waveCompletePanel.SetActive(false);
        if (sacrificeMenuPanel != null) sacrificeMenuPanel.SetActive(false);
        ResetSacrifices();
    }

    public void RegisterEnemy() // call this when spawning enemy if you want dynamic count
    {
        enemiesAlive++;
    }

    public void OnEnemyKilled()
    {
        enemiesAlive = Mathf.Max(0, enemiesAlive - 1);
        Debug.Log("Enemy killed - remaining: " + enemiesAlive);

        if (enemiesAlive <= 0 && !waveFinished)
        {
            WaveCompleted();
        }
    }

    private void WaveCompleted()
    {
        waveFinished = true;
        if (waveCompletePanel != null)
        {
            waveCompletePanel.SetActive(true);
            if (waveCompleteText != null) waveCompleteText.text = "Wave 1 completed";
            if (hintText != null) hintText.text = $"Press {continueKey} to continue or {openMenuKey} to open Sacrifice Menu";
        }
    }

    void Update()
    {
        if (!waveFinished) return;

        if (Input.GetKeyDown(openMenuKey))
        {
            OpenSacrificeMenu();
        }
        else if (Input.GetKeyDown(continueKey))
        {
            ContinuePressed();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1)) // apăsăm 1
        {
            SacrificeArmGiveAR(); 
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // apăsăm 2
        {
            SacrificeLegGiveMedkit(50); 
        }
    }

    private void OpenSacrificeMenu()
    {
        if (sacrificeMenuPanel != null)
            sacrificeMenuPanel.SetActive(true);
    }

    public void ContinuePressed()
    {
        // Hide UI and prepare next wave or resume game
        if (waveCompletePanel != null) waveCompletePanel.SetActive(false);
        if (sacrificeMenuPanel != null) sacrificeMenuPanel.SetActive(false);

        waveFinished = false;

        // TODO: spawn next wave or signal another system. For now just log.
        Debug.Log("Continue pressed - start next wave (implement spawn logic).");
    }

    // Called by UI buttons
    public void SacrificeLegGiveMedkit(int medkitHealAmount)
    {
        // apply sacrifice to player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerStatus status = player.GetComponent<PlayerStatus>();
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (status != null)
            {
                status.canSprint = false;
            }
            if (ph != null)
            {
                ph.Heal(medkitHealAmount);
            }
        }

        // close UI and mark continue
        if (sacrificeMenuPanel != null) sacrificeMenuPanel.SetActive(false);
        if (waveCompletePanel != null) waveCompletePanel.SetActive(false);
        waveFinished = false;

        Debug.Log("Leg sacrificed: gave medkit and disabled sprint.");
    }

    public void SacrificeArmGiveAR()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerStatus status = player.GetComponent<PlayerStatus>();
            if (status != null)
            {
                // sacrificiu braț = pierzi ADS
                status.canAim = false;
            }

            if (weaponSlot != null)
            {
                GunScript[] guns = weaponSlot.GetComponentsInChildren<GunScript>(true);

                foreach (GunScript gun in guns)
                {
                    if (gun.gameObject.name.Contains("Pistol"))
                    {
                        gun.enabled = false; // dezactivează pistolul
                        gun.gameObject.SetActive(false); // opțional ascunzi și mesh-ul
                    }
                    else if (gun.gameObject.name.Contains("AR"))
                    {
                        gun.enabled = true; // activează AR-ul
                        gun.gameObject.SetActive(true);
                    }
                }
            }
        }

        if (sacrificeMenuPanel != null) sacrificeMenuPanel.SetActive(false);
        if (waveCompletePanel != null) waveCompletePanel.SetActive(false);
        waveFinished = false;

        Debug.Log("Arm sacrificed: Pistol disabled, AR enabled, aiming disabled.");
    }

    public void ResetSacrifices()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerStatus status = player.GetComponent<PlayerStatus>();
            if (status != null)
            {
                status.canSprint = true; // Permite sprintul din nou
            }
        }
    }
    
}
