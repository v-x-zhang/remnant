using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHitbox : MonoBehaviour
{
    public EnemyHealth health;

    public GameObject damageCritHitPrefab;
    public GameObject damageHitPrefab;
    public Transform textAnchor;
    public Transform uiCanvas;
    public void OnRaycastHit(RaycastWeapon weapon, Vector3 direction, bool isHeadshot, float dieForce)
    {
        float totalDamage = weapon.damage;

        if (isHeadshot)
        {
            totalDamage *= 2;
        }
        health.TakeDamage(totalDamage, direction, dieForce);


        if (isHeadshot)
        {
            GameObject critHit = Instantiate(damageCritHitPrefab, uiCanvas);
            Text critText = critHit.GetComponentInChildren<Text>();
            critText.text = totalDamage.ToString();

            critHit.GetComponent<FollowObject>().transformToFollow = textAnchor;
            Destroy(critHit.gameObject, 1.5f);
        }
        else
        {
            GameObject critHit = Instantiate(damageHitPrefab, uiCanvas);
            Text critText = critHit.GetComponentInChildren<Text>();
            critText.text = totalDamage.ToString();

            critHit.GetComponent<FollowObject>().transformToFollow = textAnchor;
            Destroy(critHit.gameObject, 1.5f);
        }        
    }

}
