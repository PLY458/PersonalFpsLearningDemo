using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace FPS_Weapon_Control
{

    public enum E_FireModel {
        Semi, Auto, Burst
    };

    public enum E_ShotModel {
        Raycast, Physics
    };

    public class WeaponObject : InteractObject
    {

        public TransformData sightAimTrans;
        public float fireRate;
        public FireEffect fireVfx;

        public float aimDownSpeed = 8f;
        public float bulletRange = 100f;
        [Range(0.01f, 0.25f)]
        public float bulletSpread = 0.1f;

        public float bulletDamage = 10f;
        [Range(1f, 2f)]
        public float headshotMult = 1.25f;

        [Range(40f, 80f)]
        public float aimFOV = 60f;
        [Range(0.0f, 0.5f)]
        public float aimDis = 0.0f;
        [Range(0.0f, 1.0f)]
        public float aimSpreadMultiplier = 0.2f;
        [Range(0f, 1f)]
        public float aimDownMultiplier = 0.5f;

        public E_FireModel fireModel; // 射击模式
        public E_ShotModel shotModel;

        public int bulletsPerShot = 1; // 单次发射子弹数
        public bool canFireWhileActing = true; // 特殊动作下是否允许武器击发
        public int ammoClip;

        public Sprite weaponIcon;
        public string fireSounds;
        public string reloadSounds;

    }

}

