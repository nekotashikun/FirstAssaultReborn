using UnityEngine;

namespace Weapons
{
    public abstract class BaseWeaponBehaviour : MonoBehaviour
    {
        [SerializeField, Header("Basic Weapon Settings")]
        protected float _weaponDistance = 10f;

        [SerializeField]
        protected float _weaponActivationCooldown = 0.5f;

        [SerializeField]
        protected float _baseDamage = 1;

        [SerializeField]
        protected string _fireAnimParameter = string.Empty;

        [SerializeField]
        protected string _reloadAnimParameter = string.Empty;
        
        public string FireAnimParameter => _fireAnimParameter;

        public virtual string ReloadAnimParameter => _reloadAnimParameter;

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