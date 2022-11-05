using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
public class WeaponRecoil : MonoBehaviour
{

    //[HideInInspector]public CinemachineCameraOffset followOffset;
    //[HideInInspector] public CinemachineCameraOffset followOffset2;

    //[HideInInspector]public Transform playerRoot;
    //public ThirdPersonController playerInput;

    [HideInInspector] public Animator rigController;

    public CinemachineImpulseSource recoilCam;
    public CinemachineImpulseSource recoilShake;

    public float verticalRecoilMultiplier;
    public float shakeMultiplier;

    private void Start()
    {
        recoilCam.m_ImpulseDefinition.m_AmplitudeGain = verticalRecoilMultiplier;
        recoilShake.m_ImpulseDefinition.m_AmplitudeGain = shakeMultiplier/4;
    }

    public void GenerateRecoil(string weaponName)
    {
        recoilCam.GenerateImpulse(Camera.main.transform.forward);
        recoilShake.GenerateImpulse(Camera.main.transform.forward);

        rigController.Play("recoil_" + weaponName, 1, 0.0f);
    }
}
