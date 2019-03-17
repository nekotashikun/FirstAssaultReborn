using UnityEngine;

namespace Weapons
{
    public abstract class BaseWeaponBehaviour : MonoBehaviour
    {
        [SerializeField, Header("Basic Weapon Settings")]
        private float _weaponDistance = 10f;

        [SerializeField]
        private float _weaponActivationCooldown = 0.5f;

        [SerializeField]
        private float _baseDamage = 1;

        protected float timeSinceLastWeaponUsage = 0;

        public float WeaponDistance => _weaponDistance;
        public float WeaponActivationCooldown => _weaponActivationCooldown;
        public float BaseDamage => _baseDamage;
        public bool ShouldAim { get; set; }

        public abstract void Fire();

        protected virtual void Update()
        {
            timeSinceLastWeaponUsage -= Time.deltaTime;
        }
    }
}