using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollection : MonoBehaviour
{
    [SerializeField]
    private E_Collection_Type type_Collected;

    [SerializeField]
    private Transform fbx_Coin;

    private Collider collider_Coin;

    [SerializeField]
    [Range(10f,45f)]
    private float speed_CoinRot = 30f;
    [SerializeField]
    [Range(0.2f, 1.0f)]
    private float factor_CoinRot = 1f;

    private void Start()
    {
        collider_Coin = GetComponent<Collider>();
    }

    private void Update()
    {
        CoinRotAnimate();
    }

    private void CoinRotAnimate()
    {
        fbx_Coin.Rotate(factor_CoinRot * new Vector3(0, speed_CoinRot, 0) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CollectMgr.GetInstance().AddPoint(type_Collected);
        }
    }


}
