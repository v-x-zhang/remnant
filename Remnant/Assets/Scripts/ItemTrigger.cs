using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    public WeaponPickup weaponScript;

    public Outline outlineScript;

    public bool inRange;

    public Color inRangeColor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inRange = true;
            weaponScript.playerShoot = other.GetComponent<ThirdPersonShooterController>();
            weaponScript.isInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inRange = false;
            weaponScript.playerShoot = null;
            weaponScript.isInRange = false;
        }
    }

    private void Update()
    {
        if (inRange)
        {
            outlineScript.OutlineColor = Color.Lerp(outlineScript.OutlineColor, inRangeColor, 0.1f);
        }
        else
        {
            outlineScript.OutlineColor = Color.Lerp(outlineScript.OutlineColor, Color.white, 0.1f);
        }
    }
}
