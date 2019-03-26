using UnityEngine;

namespace Weapons.ResponsiveObjects.Prototypes
{
    public class PrototypeDestroyResponse : MonoBehaviour, IWeaponDamageResponsive
    {
        public void HandleDamageCalculations(float weaponDamage)
        {
            Destroy(gameObject);
        }
    }
}
