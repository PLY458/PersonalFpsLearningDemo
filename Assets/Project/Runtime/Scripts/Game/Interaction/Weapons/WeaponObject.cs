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

        
        [Header("��������")]
        public E_ShotModel shotModel;
        public FireEffect fireVfx;

        [Header("ǹе����")]
        public E_FireModel fireModel; // ���ģʽ
        public float fireRate;
        public int bulletsPerShot = 1; // ���η����ӵ���
        public bool canFireWhileActing = true; // ���⶯�����Ƿ�������������
        public float fireDelay = 0f; //0 to shoot instantly
        [Range(0f, 2f)]
        public float fireCooldownSpeed = 1f;

        [Header("��ҩ����")]
        public int ammoClip;
        public int startingClips;
        public float bulletRange = 100f;
        [Range(0.01f, 0.25f)]
        public float bulletSpread = 0.1f;
        [Range(0.0f, 1.0f)]
        public float aimSpreadMultiplier = 0.2f;

        [Header("��׼����")]
        public TransformData sightAimTrans;
        [Range(40f, 80f)]
        public float aimFOV = 60f;
        public float aimDownSpeed = 8f;

        [Header("�˺�����")]
        public float bulletDamage = 10f;
        [Range(1f, 2f)]
        public float headshotMult = 1.25f;

        [Header("����������")]
        [Range(0f, 1f)]
        public float aimDownMultiplier = 0.5f;
        public int cyclesInClip = 1;
        public GunRecoil recoilInfo;

        [Header("UI����")]
        public Sprite weaponIcon;

        [Header("��Ч����")]
        public string fireSounds = "Fire";
        public string reloadSounds = "Reload";



    }

    [System.Serializable]
    public class GunRecoil
    {
        public RecoilValue xRecoil;
        public RecoilValue yRecoil;
        public RecoilValue zRecoil;

        [Range(0f, 1f)]
        public float randomizeRecoil = 0;

        public Vector3 GetRecoil(int shot, int max, int cyclesInClip)
        {
            int shotsPerCycle = (max / cyclesInClip);
            int cycle = (shot / shotsPerCycle);
            int numberInCycle = shot - (cycle * shotsPerCycle);
            float percent = (float)numberInCycle / (float)shotsPerCycle;

            float random = Random.Range(-randomizeRecoil, randomizeRecoil) / (float)cyclesInClip;
            percent = Mathf.Clamp(percent + random, 0f, 1f);
            return RecoilValue(percent);
        }

        public Vector3 GetRecoil(int shot, int max)
        {
            float percent = (float)shot / (float)max;
            float random = Random.Range(-randomizeRecoil, randomizeRecoil);
            percent = Mathf.Clamp(percent + random, 0f, 1f);
            return RecoilValue(percent);
        }

        Vector3 RecoilValue(float percent)
        {
            return new Vector3(xRecoil.EvaluteValue(percent),
                                yRecoil.EvaluteValue(percent),
                                zRecoil.EvaluteValue(percent));
        }
    }

    [System.Serializable]
    public class RecoilValue
    {
        public AnimationCurve graph;
        public float multiplier = 1;

        public float EvaluteValue(float time)
        {
            return graph.Evaluate(time) * multiplier;
        }
    }

}

