using System.Collections;
using UnityEngine;
using Weapons.Prototypes;

namespace Weapons.Prototypes
{
    public class BasicTacReloadHitscanWeapon : BasicHitscanWeapon
    {
        [SerializeField]
        private string _tacReloadAnimParameter = string.Empty;

        [SerializeField]
        private int _maxCapacity = 30;

        private int _currentCapacity = 0;

        public override string ReloadAnimParameter => _currentCapacity > 0 ? _tacReloadAnimParameter : _reloadAnimParameter;

        private void Start()
        {
            _currentCapacity = _maxCapacity;
        }

        public override bool Fire()
        {
            bool didFire = base.Fire();

            if (didFire) --_currentCapacity;

            return didFire;
        }

        public override bool Reload()
        {
            StartCoroutine(ExecuteDelayedReload());
            return true;
        }        
        
        //TODO: fuck this is such a hack reeee but prototype so ye idc.
        private IEnumerator ExecuteDelayedReload()
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            _currentCapacity = _maxCapacity;
        }
    }
}