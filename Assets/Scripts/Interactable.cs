using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    // WARNING: Interact() can only be called on an object that has a collider attached to it

    [SerializeField]
    UnityEvent OnHoverOver;
    [SerializeField]
    UnityEvent OnHoverCancel;
    [SerializeField]
    UnityEvent OnInteract;

    public void HoverOver()
    {
        OnHoverOver.Invoke();
    }

    public void StopHover()
    {
        OnHoverCancel.Invoke();
    }

    public void Interact()
    {
        OnInteract.Invoke();
    }
}
