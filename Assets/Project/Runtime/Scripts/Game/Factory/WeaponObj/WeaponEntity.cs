using UnityEngine;

public class WeaponEntity : MonoBehaviour
{
    public string weaponName;

    WeaponFactory originFactory;

    public WeaponFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Redefined origin factory!");
            originFactory = value;
        }
    }

    public void Recycle()
    {
        originFactory.Reclaim(this);
    }
}
