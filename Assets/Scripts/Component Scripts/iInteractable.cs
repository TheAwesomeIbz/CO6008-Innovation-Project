using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interactable interface used for interactable objects within the game.
/// </summary>
public interface iInteractable
{
    /// <summary>
    /// Interactable property that determines whether a property is interactable or not
    /// </summary>
    public bool Interactable { get; }

    /// <summary>
    /// Blueprint method called on interactable objects, that take an object playerObject as a parameter.
    /// </summary>
    /// <param name="playerObject">Generic player object that must be casted upon implementation</param>
    public void Interact(object playerObject);

}
