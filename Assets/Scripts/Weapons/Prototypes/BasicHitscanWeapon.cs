using UnityEngine;
using Player;
using Weapons.ResponsiveObjects;

namespace Weapons.Prototypes
{
    public class BasicHitscanWeapon : BaseWeaponBehaviour
    {
        public override bool Fire()
        {
            if (timeSinceLastWeaponUsage > 0) return false;
            //Debug.Log("[Matt] Gun Fired!");

            timeSinceLastWeaponUsage = WeaponActivationCooldown;

            RaycastHit hit; //NO C# 7 REEEEEEEEEEEEEEEEEEE
            if (Physics.Raycast(transform.position, transform.forward, out hit, WeaponDistance)) hit.transform.GetComponent<IWeaponDamageResponsive>()?.HandleDamageCalculations(BaseDamage);

            return true;
        }

        public override bool Reload()
        {
            //Debug.Log("[Matt] Gun Reloaded!");
            return true;
        }
    }
}