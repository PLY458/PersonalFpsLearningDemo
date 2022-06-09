using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FPS_Weapon_Control;

public class WeaponBarPlane : UIBasePanel
{

    Image img_WeaponIcon;
    TMP_Text txt_MagNum;
    TMP_Text txt_AmmoNum;

    WeaponHandler currentWeapon;

    protected override void InitPanel()
    {
        base.InitPanel();

        img_WeaponIcon = GetControl<Image>("img_WeaponIcon");
        txt_MagNum = GetControl<TMP_Text>("txt_MagNum");
        txt_AmmoNum = GetControl<TMP_Text>("txt_AmmoNum");

        EventCenter.GetInstance().AddEventListener<WeaponHandler>("SetWeapon_weaponBar", SetWeaponInfo);
    }

    public override void RefreshPlane()
    {
        base.RefreshPlane();

        UpdateAmmoInfo();
    }

    private void UpdateAmmoInfo()
    {
        if (currentWeapon != null)
        {
            txt_MagNum.text = currentWeapon.ammoInClip.ToString();
            txt_AmmoNum.text = currentWeapon.totalAmmo.ToString();
        }
    }

    void SetWeaponInfo( WeaponHandler handler )
    {
        currentWeapon = handler;
        if (currentWeapon.weapon.weaponIcon != null)
            img_WeaponIcon.sprite = currentWeapon.weapon.weaponIcon;
    }

}
