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
    void Start()
    {
        originalPosition = transform.position;
    }
    
    void Update()
    {
        counter += Time.deltaTime;
        if (counter >= Mathf.PI * 2) { counter = 0; }
        
        transform.position = originalPosition + new Vector3(0, Mathf.Sin(counter * frequency), 0) * amplitude;
    }
}
