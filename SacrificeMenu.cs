using UnityEngine;
using UnityEngine.UI;

public class SacrificeMenu : MonoBehaviour
{
    public int medkitHealAmount = 50;
    public int arDamage = 15;
    public int arMaxAmmo = 120;
    public bool arFullAuto = true;

    public void OnSacrificeLegClicked()
    {
        if (WaveManager.Instance != null)
            WaveManager.Instance.SacrificeLegGiveMedkit(medkitHealAmount);
    }

    public void OnSacrificeArmClicked()
    {
        if (WaveManager.Instance != null)
            WaveManager.Instance.SacrificeArmGiveAR(arDamage, arMaxAmmo, arFullAuto);
    }

    public void OnCancelClicked()
    {
        if (gameObject != null) gameObject.SetActive(false);
    }
}