using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerSetup : MonoBehaviour
    {
        public Cinemachine.CinemachineVirtualCamera VirtualCamera;
        public Cinemachine3rdPersonAim cinemachine3RdPersonAim;

        public AimStateManager aimStateManager;
        public MovementStateManager movementStateManager;
        public ActionStateManager actionStateManager;
        public WallBuilder wallBuilder;
        public AdsAnimationManager adsAnimationManager;

        public WeaponManager weaponManager;
        public WeaponAmmo weaponAmmo;
        public WeaponRecoil weaponRecoil;
        public WeaponBloom weaponBloom;
        public GameObject gun;

        public Transform TPWeaponHolder;

        public Rig gunRig;

        public Animator anim;
        

        public void IsLocalPlayer()
            {
                //TPWeaponHolder.gameObject.SetActive(false);

                gun.SetActive(true);

                movementStateManager.enabled = true;
                aimStateManager.enabled = true;
                actionStateManager.enabled = true;
                wallBuilder.enabled = true;
                adsAnimationManager.enabled = true;

                VirtualCamera.enabled = true;
                cinemachine3RdPersonAim.enabled = true;

                gunRig.weight = 0;

                weaponManager.enabled = true;
                weaponAmmo.enabled = true;
                weaponRecoil.enabled = true;
                weaponBloom.enabled = true;
            }
        
        public void SetTPWeapon(int _weaponIndex)
            {
                // Manage third-person weapon models based on _weaponIndex
                if (TPWeaponHolder == null)
                    {
                        Debug.LogError("TPWeaponHolder is not assigned.");
                        return;
                    }
                
                if (_weaponIndex == 1)
                    {
                        // Example: Handle first weapon model
                        TPWeaponHolder.GetChild(0).gameObject.SetActive(false);
                        TPWeaponHolder.GetChild(1).gameObject.SetActive(false);
                        gunRig.weight = 0;
                    }
                else if (_weaponIndex == 2)
                    {
                        // Example: Handle second weapon model
                        TPWeaponHolder.GetChild(0).gameObject.SetActive(true);
                        TPWeaponHolder.GetChild(1).gameObject.SetActive(false);
                        gunRig.weight = 1;
                    }
            }
    }