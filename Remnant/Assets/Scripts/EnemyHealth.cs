using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth;
    public float dieForce;

    public float maxEmission;
    public float blinkDuration;


    public Gradient outlineColor;

    SkinnedMeshRenderer skinnedMeshRenderer;
    Material blinkMaterial;

    [HideInInspector]
    public float currentHealth;

    Outline outline;
    Ragdoll ragdoll;
    AIMovement movement;

    Color baseEmissionColor;
   
    float blinkTimer;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        movement = GetComponent<AIMovement>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        blinkMaterial = skinnedMeshRenderer.material;
        blinkMaterial.EnableKeyword("_EMISSION");
        baseEmissionColor = blinkMaterial.GetColor("_EmissionColor");
        outline = GetComponentInChildren<Outline>();
        ragdoll = GetComponent<Ragdoll>();
    }

    public void TakeDamage(float amount, Vector3 direction, float _dieForce)
    {
        dieForce = _dieForce;
        currentHealth -= amount;
        outline.OutlineColor = outlineColor.Evaluate( 1 -(currentHealth / maxHealth));
        if (currentHealth <= 0)
        {
            Die(direction);
        }

        blinkTimer = blinkDuration;
    }

    void Die(Vector3 direction)
    {
        outline.enabled = false;
        direction.y = 1f;
        ragdoll.ActivateRagdoll();
        ragdoll.ApplyForce(direction * dieForce);
        movement.isDead = true;
    }

    private void Update()
    {
        blinkTimer -= Time.deltaTime;
        float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
        float emissionIntensity = lerp * maxEmission;
        blinkMaterial.SetColor("_EmissionColor", baseEmissionColor * emissionIntensity);
    }

}
