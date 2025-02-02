using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class SCR_DescriptiveObject : MonoBehaviour, UI_DescriptionUI.IDescriptive
{
    [SerializeField] Collider2D currentCollider;

    [Header("DESCRIPTIVE OBJECT PROPERTIES")]
    [SerializeField] protected string header;
    [SerializeField] [TextArea(2,2)] protected string description;
    [SerializeField] protected string usageDescription;

    public string Header => header;

    public string Description => description;

    public string UsageDesctiption => usageDescription;

    public GameObject GameObject => gameObject;

    public virtual void OnOptionUsed()
    {

    }
}
