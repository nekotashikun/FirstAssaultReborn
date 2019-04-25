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

        [SerializeField]
        private string _fireAnimParameter = string.Empty;

        [SerializeField]
        private string _reloadAnimParameter = string.Empty;
        
        public string FireAnimParameter => _fireAnimParameter;

        public string ReloadAnimParameter => _reloadAnimParameter;

        protected float timeSinceLastWeaponUsage = 0;

        public float WeaponDistance => _weaponDistance;
        public float WeaponActivationCooldown => _weaponActivationCooldown;
        public float BaseDamage => _baseDamage;
        public bool ShouldAim { get; set; }

        public abstract bool Fire();

        public abstract bool Reload();

        protected virtual void Update()
        {
            timeSinceLastWeaponUsage -= Time.deltaTime;
        }
    }
}