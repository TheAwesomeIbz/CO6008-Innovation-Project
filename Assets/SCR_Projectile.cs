using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Projectile : MonoBehaviour
{
    public SO_WeaponProperties weaponProperties { get; private set; }
    public Rigidbody2D GetRigidbody2D => GetComponent<Rigidbody2D>();

    
    IEnumerator Start()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    public void InitializeProjectile(SO_WeaponProperties weaponProperties, Transform shootingObject)
    {
        this.weaponProperties = weaponProperties;
        transform.localScale = new Vector3(transform.localScale.x * Mathf.Sign(shootingObject.transform.localScale.x), transform.localScale.y);
    }
    public void OnCollide()
    {
        Destroy(gameObject);
    }
}
