using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBoxInteract : InteractObject
{
    public override void Interact()
    {
        base.Interact();
        Destroy(gameObject);
    }
}
