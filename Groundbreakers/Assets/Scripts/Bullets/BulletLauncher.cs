﻿namespace Assets.Scripts
{
    using System;
    using System.Collections.Generic;

    using Sirenix.OdinInspector;

    using UnityEngine;

    /// <inheritdoc />
    /// <summary>
    /// Ideally, equip this launcher to character objects when in Ranged attack mode.
    /// Disable this component when switched to Melee Mode.
    /// </summary>
    [RequireComponent(typeof(BulletMovement))]
    [RequireComponent(typeof(DamageHandler))]
    public class BulletLauncher : MonoBehaviour
    {
        #region Inspector
        
        [SerializeField]
        private GameObject bulletPrefab;

        [SerializeField]
        private Type type;

        [SerializeField]
        private Vector3 offSet;

        #endregion

        #region Internal Fields

        private DamageHandler damageHandler;

        #endregion

        #region Public Properties

        private enum Type
        {
            SingleShot,

            MultiShot,
        }

        #endregion

        #region Public Functions    

        [Button("Test Launch All")]
        public void LaunchAll()
        {
            // temp solution
            if (this.type == Type.SingleShot)
            {
                this.SingleShot();
            }
            else
            {
                this.MultiShot();
            }
        }

        public void AimAtTarget(Transform target)
        {
            this.transform.LookAt(target);
        }

        // Warning, this is temporary solution, should use a proper external damage handler
        public void Melee(Transform target)
        {
            this.damageHandler.DeliverDamageTo(target.gameObject, true);
        }

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            this.damageHandler = this.GetComponent<DamageHandler>();

            var attributes = this.transform.parent.GetComponent<characterAttributes>();
            this.damageHandler.SetCharacterAttribute(attributes);
        }

        private void Update()
        {
            // DEBUG
            if (Input.GetKeyDown("1"))
            {
                this.type = Type.SingleShot;
            }

            if (Input.GetKeyDown("2"))
            {
                this.type = Type.MultiShot;
            }
        }

        #endregion

        #region Internal Functions

        /// <summary>
        /// Create Instance from bullet prefab.
        /// </summary>
        /// <returns>
        /// The <see cref="BulletMovement"/>.
        /// </returns>
        private BulletMovement InstantiateBullet()
        {
            // Currently using native Instantiation method. Will switch to Object pool.
            // Should also trigger event
            var pos = this.transform.position;
            var go = Instantiate(this.bulletPrefab, pos, Quaternion.identity);

            var bullet = go.GetComponent<BulletMovement>();

            // this.buffer.Add(bullet);
            return bullet;
        }

        // Subject to change
        private void SingleShot()
        {
            var bullet = this.InstantiateBullet();
            var direction = this.transform.forward;
            bullet.Launch(direction, this.damageHandler);
        }

        // Subject to change
        private void MultiShot()
        {
            var bulletA = this.InstantiateBullet();
            var bulletB = this.InstantiateBullet();
            var bulletC = this.InstantiateBullet();

            // Subject to change
            var directionA = this.transform.forward;
            var directionB = Quaternion.AngleAxis(-45, Vector3.up) * directionA;
            var directionC = Quaternion.AngleAxis(45, Vector3.up) * directionA;

            bulletA.Launch(directionA, this.damageHandler);
            bulletB.Launch(directionB, this.damageHandler);
            bulletC.Launch(directionC, this.damageHandler);
        }

        #endregion
    }
}