using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
public class ThirdPersonShooterController : MonoBehaviour
{
    public enum WeaponSlot
    {
        Primary = 0,
        Secondary = 1
    }

    public Rig headRig;
    public Rig aimRig;
    public Rig handRig;

    [Header("Remove later --- FIX")]
    public MultiAimConstraint aimConstraintChest1;
    public MultiAimConstraint aimConstraintChest2;
    public MultiAimConstraint aimConstraintGun;
    public MultiParentConstraint aimParentConstraint;
    public MultiPositionConstraint aimPosConstraint;
    public MultiRotationConstraint aimRotConstraint;

    [Space(10)]
    public TwoBoneIKConstraint leftHandMover;
    public TwoBoneIKConstraint rightHandMover;


    public CinemachineVirtualCamera aimVirtualCamera;
    public float normalSens;
    public float aimSens;
    public LayerMask aimColliderLayerMask = new LayerMask();
    public Transform endTransform;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs inputs;
    public Animator animator;
    public Animator rigController;

    [Header("Weapons")]
    public RaycastWeapon[] currentWeapons = new RaycastWeapon[2];
    public Transform[] weaponSlots;
    public bool isHolstered;
    public PlayerUI playerUI;


    [Header("Reloading")]
    public RigAnimationEvents animationEvents;
    public Transform leftHand;


    int currentActiveWeapon;
    public bool isChangingWeapon;
    bool isReloading;
    bool isAiming;

    public void ChangeAmmo(int newAmmo)
    {
        playerUI.currentAmmoText.text = newAmmo.ToString();
    }

    void OnAnimationEvent(string eventName)
    {
        switch (eventName)
        {
            case "DetachMag":
                DetachMagazine();
                break;
            case "DropMag":
                DropMagazine();
                break;
            case "RefillMag":
                RefillMagazine();
                break;
            case "AttachMag":
                AttachMagazine();
                break;
            case "ReloadDone":
                ReloadFinish();
                break;
        }
    }

    #region Reloading

    GameObject magazineHand;

    void Reload()
    {
        rigController.SetTrigger("reloadTrigger");
    }

    #region Animation Events

    public RaycastWeapon GetActiveWeapon()
    {
        return GetWeapon(currentActiveWeapon);
    }
    void DetachMagazine()
    {
        RaycastWeapon weapon = GetActiveWeapon();

        magazineHand = Instantiate(weapon.weaponMag, leftHand, true);

        weapon.weaponMag.SetActive(false);
    }

    void DropMagazine()
    {
        GameObject droppedMag = Instantiate(magazineHand, magazineHand.transform.position, magazineHand.transform.rotation);
        droppedMag.transform.localScale = new Vector3(1, 1, 1);
        droppedMag.AddComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
        droppedMag.AddComponent<BoxCollider>();
        magazineHand.SetActive(false);
    }

    void RefillMagazine()
    {
        magazineHand.SetActive(true);
    }

    void AttachMagazine()
    {
        RaycastWeapon weapon = GetActiveWeapon();
        weapon.weaponMag.SetActive(true);
        Destroy(magazineHand);

        weapon.currentAmmo = weapon.maxAmmo;
        playerUI.currentAmmoText.text = weapon.currentAmmo.ToString();
        rigController.ResetTrigger("reloadTrigger");
    }

    void ReloadFinish()
    {
        isReloading = false;
    }

    #endregion

    #endregion

    #region Equipping Weapons

    bool isOnlyWeapon;

    public bool samePressed;
    public void EquipWeapon(RaycastWeapon newWeapon)
    {
        isOnlyWeapon = !GetWeapon(currentActiveWeapon);
        int weaponSlotIndex = (int)newWeapon.weaponSlot;
        var currentWeapon = GetWeapon(weaponSlotIndex);
        if (currentWeapon)
        {
            Destroy(currentWeapon.gameObject);
            //Drop Weapon
        }
        currentWeapon = newWeapon;
        currentWeapon.raycastDestination = endTransform;
        currentWeapon.recoil.rigController = rigController;
        currentWeapon.transform.SetParent(weaponSlots[weaponSlotIndex], false);
        currentWeapon.playerController = this;
        currentWeapons[weaponSlotIndex] = currentWeapon;

        SetActiveWeapon(newWeapon.weaponSlot);
    }

    void SetActiveWeapon(WeaponSlot weaponSlot)
    {
        samePressed = false;
        int holsterIndex = currentActiveWeapon;
        int activateIndex = (int)weaponSlot;
        currentActiveWeapon = activateIndex;

        if ((holsterIndex == activateIndex) && isOnlyWeapon)
        {
            holsterIndex = -1;
        }else if(holsterIndex == activateIndex)
        {
            isOnlyWeapon = false;
            samePressed = true;
            isChangingWeapon = false;
            return;
        }

        StartCoroutine(SwitchWeapon(holsterIndex, activateIndex));
        isOnlyWeapon = false;
    }

    RaycastWeapon GetWeapon(int index)
    {
        if (index < 0 || index >= currentWeapons.Length) return null;

        return currentWeapons[index];
    }
   
    IEnumerator SwitchWeapon(int holsterIndex, int activateIndex)
    {
        yield return StartCoroutine(HolsterActiveWeapon(holsterIndex));
        yield return StartCoroutine(ActivateNewWeapon(activateIndex));
    }

    IEnumerator HolsterActiveWeapon(int index)
    {
        var weapon = GetWeapon(index);

        if (!weapon) yield return null;
        rigController.SetBool("isHolstered", true);
        //Debug.Log("Started Holster");

        //yield return new WaitForSeconds(rigController.GetCurrentAnimatorStateInfo(0).length);

        //Debug.Log("Finished Holster");
        isHolstered = rigController.GetBool("isHolstered");
    }

    IEnumerator ActivateNewWeapon(int index)
    {
        var weapon = GetWeapon(index);

        //Debug.Log(weapon);

        if (!weapon)
        {
            //Debug.Log("No weapon to switch to.");
            yield break;
        }
        rigController.Play("equip_" + weapon.weaponName, 0,0.0f);
        rigController.SetBool("isHolstered", false);

        yield return new WaitForSeconds(rigController.GetCurrentAnimatorStateInfo(0).length);
        //Debug.Log("Check");
        isHolstered = rigController.GetBool("isHolstered");
        //Debug.Log("equip_" + weapon.weaponName);
        isChangingWeapon = false;
    }
    #endregion

    #region Updates
   

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        inputs = GetComponent<StarterAssetsInputs>();

        if (GetWeapon(currentActiveWeapon) != null)
        {
            EquipWeapon(GetWeapon(currentActiveWeapon));
        }
    }

    private void Start()
    {
        animationEvents.WeaponAnimationEvent.AddListener(OnAnimationEvent);
    }


    private void Update()
    {
        var currentWeapon = GetWeapon(currentActiveWeapon);

        Vector3 mouseWorldPos = Vector3.zero;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            endTransform.position = raycastHit.point;
            mouseWorldPos = raycastHit.point;
        }


        if (inputs.reload)
        {
            if (isAiming || isReloading || isChangingWeapon)
            {
                inputs.reload = false;
                return;
            }

            if (!GetWeapon(currentActiveWeapon)) return;

            inputs.reload = false;
            isReloading = true;
            Reload();
        }

        if (inputs.primary)
        {
            if (isAiming || isReloading)
            {
                inputs.primary = false;
                return;
            }

            if (isChangingWeapon)
            {
                if (GetWeapon(currentActiveWeapon)) return;
            }

            inputs.primary = false;
            isChangingWeapon = true;
            SetActiveWeapon(WeaponSlot.Primary);
        }

        if (inputs.secondary)
        {
            if (isAiming || isReloading)
            {
                inputs.secondary = false;
                return;
            }

            if (isChangingWeapon)
            {
                if (GetWeapon(currentActiveWeapon)) return;
            }

            inputs.secondary = false;
            isChangingWeapon = true;
            SetActiveWeapon(WeaponSlot.Secondary);
        }

        if (inputs.holster)
        {
            if(currentWeapon == null || isAiming || isReloading || isChangingWeapon)
            {
                inputs.holster = false;
                return;
            }
            inputs.holster = false;
            isChangingWeapon = true;
            ToggleActiveWeapon();
        }
        


        if (inputs.aim && currentWeapon != null && !isReloading && !isChangingWeapon)
        {
            isAiming = true;

            thirdPersonController.MoveSpeed = 1.5f;
            thirdPersonController.SprintSpeed = 2.5f;


            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSens);
            thirdPersonController.SetRotateOnMove(false);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), .5f, Time.deltaTime * 10f));

            Vector3 worldAimTarget = mouseWorldPos;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);

            SetAimWeights(isAiming);

            leftHandMover.data.hintWeight = Mathf.Lerp(leftHandMover.data.hintWeight, 0, Time.deltaTime * 10f);
            rightHandMover.data.hintWeight = Mathf.Lerp(rightHandMover.data.hintWeight, 0, Time.deltaTime * 10f);



            rigController.SetBool("isAiming", isAiming);

        }
        else
        {
            isAiming = false;
            thirdPersonController.MoveSpeed = 2f;
            thirdPersonController.SprintSpeed = 5.335f;

            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSens);
            thirdPersonController.SetRotateOnMove(true); //Set to true for aiming

            //Vector3 worldAimTarget = mouseWorldPos;
            //worldAimTarget.y = transform.position.y;
            //Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            //transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);

            rigController.SetBool("isAiming", isAiming);

            SetAimWeights(isAiming);

            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 0f, Time.deltaTime * 10f));
            leftHandMover.data.hintWeight = Mathf.Lerp(leftHandMover.data.hintWeight, 1, Time.deltaTime * 10f);
            rightHandMover.data.hintWeight = Mathf.Lerp(rightHandMover.data.hintWeight, 1, Time.deltaTime * 10f);
        }

        

        if (currentWeapon == null) return;

        currentWeapon.UpdateBullets(Time.deltaTime);

        if (isReloading) return;
        if (isChangingWeapon) return;



        if (inputs.shoot)
        {
            if (!isAiming)
            {
                inputs.shoot = false;
                return;
            }
            currentWeapon.TryFire();

            if (currentWeapon.isFullAuto) return;

            inputs.shoot = false;

        }


    }

    #endregion
    void ToggleActiveWeapon()
    {
        bool isHolstered = rigController.GetBool("isHolstered");
        if (isHolstered)
        {
            StartCoroutine(ActivateNewWeapon(currentActiveWeapon));
        }
        else
        {
            StartCoroutine(HolsterActiveWeapon(currentActiveWeapon));
        }
    }
    void SetAimWeights(bool isAiming)
    {
        if (isAiming)
        {
            //aimRig.weight = Mathf.Lerp(aimRig.weight, 1f, Time.deltaTime * 20f);

            aimConstraintChest1.weight = Mathf.Lerp(aimConstraintChest1.weight, 0.35f, Time.deltaTime * 20f);
            aimConstraintChest2.weight = Mathf.Lerp(aimConstraintChest2.weight, .52f, Time.deltaTime * 20f);
            aimConstraintGun.weight = Mathf.Lerp(aimConstraintGun.weight, 0.66f, Time.deltaTime * 20f);

            aimParentConstraint.weight = Mathf.Lerp(aimParentConstraint.weight, 1f, Time.deltaTime * 20f);
            aimPosConstraint.weight = Mathf.Lerp(aimPosConstraint.weight, 1f, Time.deltaTime * 20f);
            aimRotConstraint.weight = Mathf.Lerp(aimRotConstraint.weight, 1f, Time.deltaTime * 20f);

        }
        else
        {
            //aimRig.weight = Mathf.Lerp(aimRig.weight, 0f, Time.deltaTime * 20f);


            aimConstraintChest1.weight = Mathf.Lerp(aimConstraintChest1.weight, 0f, Time.deltaTime * 20f);
            aimConstraintChest2.weight = Mathf.Lerp(aimConstraintChest2.weight, 0f, Time.deltaTime * 20f);
            aimConstraintGun.weight = Mathf.Lerp(aimConstraintGun.weight, 0f, Time.deltaTime * 20f);

            aimParentConstraint.weight = Mathf.Lerp(aimParentConstraint.weight, 0f, Time.deltaTime * 20f);
            aimPosConstraint.weight = Mathf.Lerp(aimPosConstraint.weight, 0f, Time.deltaTime * 20f);
            aimRotConstraint.weight = Mathf.Lerp(aimRotConstraint.weight, 0f, Time.deltaTime * 20f);
        }
    }
}
