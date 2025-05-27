using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    // WARNING: Interact() can only be called on an object that has a 2d collider attached to it

    [SerializeField]
    UnityEvent OnInteract;
    public void Interact()
    {
        OnInteract.Invoke();
    }
}
