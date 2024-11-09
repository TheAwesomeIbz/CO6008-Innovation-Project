using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class SCR_Projectile : MonoBehaviour
    {
        public SO_WeaponProperties weaponProperties { get; private set; }
        private Transform shootingObject;
        public Rigidbody2D GetRigidbody2D => GetComponent<Rigidbody2D>();


        IEnumerator Start()
        {
            yield return new WaitForSeconds(3);
            Destroy(gameObject);
        }

        public void  InitializeProjectile(SO_WeaponProperties weaponProperties, Transform shootingObject, iAttackable iAttackable)
        {
            this.weaponProperties = weaponProperties;
            this.shootingObject = shootingObject;
            transform.localScale = new Vector3(transform.localScale.x * Mathf.Sign(shootingObject.transform.localScale.x), transform.localScale.y);
            GetComponent<SCR_DamageCollider>().InitializeCollider(shootingObject, iAttackable.DamageableTo);

           
        }
        public void OnCollide()
        {
            Destroy(gameObject);
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (GetComponent<Collider2D>().IsTouchingLayers(GlobalMasks.GroundLayerMask))
            {
                
                Destroy(gameObject);
            }
        }
    }
}
