using UnityEngine;
using Player;
using Weapons.ResponsiveObjects;

namespace Weapons.Prototypes
{
    public class BasicHitscanWeapon : BaseWeaponBehaviour
    {
        public override void Fire()
        {
            if (timeSinceLastWeaponUsage > 0) return;

            timeSinceLastWeaponUsage = WeaponActivationCooldown;
            
            RaycastHit hit; //NO C# 7 REEEEEEEEEEEEEEEEEEE
            if (!Physics.Raycast(transform.position, transform.forward, out hit, WeaponDistance)) return;

            hit.transform.GetComponent<IWeaponDamageResponsive>()?.HandleDamageCalculations(BaseDamage);
        }
    }
}

