using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS_Weapon_Control;

public class TestBoxInteract : InteractObject
{
    public override void Interact()
    {
        base.Interact();
        WeaponControl.GetInstance().RefillAmmo();
    }
}
