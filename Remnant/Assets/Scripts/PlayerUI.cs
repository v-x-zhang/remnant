using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Weapon")]
    public Text currentAmmoText;
    public Text maxAmmoText;
    public Text firingModeText;
    public Animator uiAnimator;

    public void StartSwitch()
    {
        uiAnimator.SetBool("isOn", false);
    }
    public void EndSwitch(RaycastWeapon newWeapon)
    {
        SetWeapon(newWeapon);

        uiAnimator.SetBool("isOn", true);
    }

    public void SetWeapon(RaycastWeapon newWeapon)
    {
        if (!newWeapon) return;
        currentAmmoText.text = newWeapon.currentAmmo.ToString();
        maxAmmoText.text = newWeapon.maxAmmo.ToString();


        if (newWeapon.isFullAuto)
        {
            firingModeText.text = "AUTO";
        }
        else
        {
            firingModeText.text = "SEMI";
        }
    }
}
