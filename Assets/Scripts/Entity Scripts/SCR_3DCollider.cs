using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class SCR_3DCollider : MonoBehaviour
{
    Collider2D objectCollider;
    float counter;

    [Header("OSCILLATION PROPERTIES")]
    [SerializeField] private float defaultZOffset;
    [SerializeField] private float periodMultiplier;
    [SerializeField] private float amplitudeMultiplier;


    void Start()
    {
        objectCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
        if (counter > Mathf.PI * 2){
            counter = 0;
        }

        float zOscillationValue = Mathf.Sin(counter * periodMultiplier) * amplitudeMultiplier;
        transform.position = new Vector3(transform.position.x, transform.position.y, defaultZOffset + zOscillationValue);

        objectCollider.enabled = transform.position.z - Camera.main.transform.position.z > 1;
    }
}
