using UnityEngine;

[CreateAssetMenu]
public class WeaponFactory : GameObjectFactory
{

    [SerializeField]
    FireEffect firePrefab = default;

    public FireEffect FireVfx => Get(firePrefab);

    public FireEffect FirePrefab { set => firePrefab = value; }

    T Get<T>(T prefab) where T : WeaponEntity
    {
        GameObject temp = PoolMgr.GetInstance().GetPoolobj(prefab.weaponName);

        T instance;

        if (temp != null)
        {
            instance = temp.GetComponent<T>();
        }
        else
        {
            instance = CreateGameObjectInstance(prefab);
            instance.OriginFactory = this;
        }
              
        return instance;
    }

    public void Reclaim(WeaponEntity entity)
    {
        Debug.Assert(entity.OriginFactory == this, "Wrong factory reclaimed!");

        var tempObject = entity.gameObject;

        //Destroy(entity.gameObject);
        PoolMgr.GetInstance().PushPoolobj(entity.weaponName, tempObject);

    }
}
