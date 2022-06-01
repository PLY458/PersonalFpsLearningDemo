using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractObject : MonoBehaviour
{
    [Range(1f, 10f)]
    public float interactRange = 3f;

    [HideInInspector]
    public bool isEnableInteract;

    public string description;

    public virtual void Start()
    {
        isEnableInteract = true;
    }

    public virtual void Interact()
    {
       
    }

}
