using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    public ThirdPersonShooterController thirdPersonShooterController;
    public PlayerUI playerUI;

    public void StartSwitch(float timeToWait)
    {
        playerUI.StartSwitch();
        //Debug.Log("Start Switch");
        StartCoroutine(EndSwitchTimer(timeToWait));
    }

    IEnumerator EndSwitchTimer(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        EndSwitch();
    }
    public void EndSwitch()
    {
        if (thirdPersonShooterController.samePressed) return;

        playerUI.EndSwitch(thirdPersonShooterController.GetActiveWeapon());

        //Debug.Log("End Switch");

    }
}
