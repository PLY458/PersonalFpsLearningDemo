using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollection : CollectObject
{

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

    protected override void CallToGatherPoints()
    {
        base.CallToGatherPoints();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("触发记分");
        if (other.gameObject.CompareTag("Player"))
        {
            CallToGatherPoints();
            Destroy(gameObject, 0.02f); // 改良为对象池收纳
        }
    }


}
