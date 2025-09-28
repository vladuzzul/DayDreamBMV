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
            if (waveCompleteText != null) waveCompleteText.text = "Wave completed";
            if (hintText != null) hintText.text =
                $"Press {continueKey} to continue, {openMenuKey} for Sacrifice Menu \n1 to Sacrifice Arm\t 2 to Sacrifice Leg";
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
        else if (Input.GetKeyDown(KeyCode.Alpha1)) // apăsăm 1 pentru braț
        {
            SacrificeArmGiveAR(25, 60, true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // apăsăm 2 pentru picior
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

    // Called by UI buttons OR hotkeys
    public void SacrificeLegGiveMedkit(int medkitHealAmount)
    {
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

        if (sacrificeMenuPanel != null) sacrificeMenuPanel.SetActive(false);
        if (waveCompletePanel != null) waveCompletePanel.SetActive(false);
        waveFinished = false;

        Debug.Log("Leg sacrificed: gave medkit and disabled sprint.");
    }

    public void SacrificeArmGiveAR(int arDamage, int arMaxAmmo, bool arFullAuto)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerStatus status = player.GetComponent<PlayerStatus>();
            GunScript pistol = player.GetComponentInChildren<GunScript>(); // assumes pistol is child of player
            if (status != null)
            {
                status.canAim = false;
            }
            if (pistol != null)
            {
                pistol.fullAuto = arFullAuto;
                pistol.damage = arDamage;
                pistol.maxAmmo = arMaxAmmo;
                pistol.SetAmmoImmediate(arMaxAmmo); // helper from your Pistol script
            }
        }

        if (sacrificeMenuPanel != null) sacrificeMenuPanel.SetActive(false);
        if (waveCompletePanel != null) waveCompletePanel.SetActive(false);
        waveFinished = false;

        Debug.Log("Arm sacrificed: gave AR and disabled aiming.");
    }
}
