using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField]
    private LayerMask selectionLayerMask;
    private Transform selectedObject;
    [SerializeField]
    private float maxSelectionDistance = 5f;

    private void Update()
    {
        // reset selected object
        if (selectedObject != null)
        {
            selectedObject.gameObject.GetComponent<Outline>().enabled = false;
            selectedObject = null;
        }

        // check if we are targeting an object and select if so
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out hit, maxSelectionDistance, selectionLayerMask))
        {
            selectedObject = hit.transform;
        }
        else
        {
            selectedObject = null;
        }

        UpdateSelectionGraphics();
    }

    private void UpdateSelectionGraphics()
    {
        if (selectedObject == null)
        {
            return;
        }

        if (selectedObject.GetComponent<Outline>() != null)
        {
            selectedObject.gameObject.GetComponent<Outline>().enabled = true;
        }
        else
        {
            Outline outline = selectedObject.gameObject.AddComponent<Outline>();
            outline.enabled = true;
        }
    }

    public void Interact()
    {
        if (selectedObject != null)
        {
            selectedObject.GetComponent<Interactable>().Interact();
        }
    }
}