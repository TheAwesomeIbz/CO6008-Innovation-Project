using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_FloatEntity : MonoBehaviour
{
    [Header("FLOATING PROPERTIES")]
    [SerializeField] float amplitude = 0.5f;
    [SerializeField] float frequency = 1f;
    private float counter = 0;
    private Vector3 originalPosition;


    [Header("ADDITIONAL PROPERTIES")]
    [SerializeField] bool useCurrentPosition;
    [SerializeField] Transform parentTransform;
    [SerializeField] Vector3 offsetPosition;
    void Start()
    {
        originalPosition = transform.position;
    }
    
    void Update()
    {
        counter += Time.deltaTime;
        if (counter >= Mathf.PI * 2) { counter = 0; }
        
        if (useCurrentPosition)
        {
            transform.position = parentTransform.position + offsetPosition + new Vector3(0, Mathf.Sin(counter * frequency), 0) * amplitude;
            return;
        }
        transform.position = originalPosition + new Vector3(0, Mathf.Sin(counter * frequency), 0) * amplitude;
    }
}
