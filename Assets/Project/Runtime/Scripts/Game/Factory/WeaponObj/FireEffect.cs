using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEffect : WeaponEntity
{

    public void SpawnOn(Transform location)
    {
        if (!transform.parent == location)
        {
            transform.SetParent(location);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;
        }
        if (!isActiveAndEnabled)
            gameObject.SetActive(true);
        StartCoroutine(RecycleAsync());
    }

    IEnumerator RecycleAsync()
    {
        yield return new WaitForSeconds(0.3f);
        Recycle();
    }
}
