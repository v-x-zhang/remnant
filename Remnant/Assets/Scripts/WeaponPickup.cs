using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public RaycastWeapon weaponPrefab;

    public ThirdPersonShooterController playerShoot;

    public bool isInRange;

    private void Update()
    {
        if (isInRange)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                PickupWeapon();
            }
        }
    }

    void PickupWeapon()
    {
        if (playerShoot)
        {
            RaycastWeapon newWeapon = Instantiate(weaponPrefab);
            playerShoot.EquipWeapon(newWeapon);

            Destroy(transform.root.gameObject);
        }
    }
}
