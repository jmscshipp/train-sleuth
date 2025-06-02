using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SittingSpot : MonoBehaviour
{
    private Transform player;

    private bool lerpingDown = false;
    private bool lerpingUp = false;
    private float lerpCounter = 0f;
    private Vector3 startPos;
    private Quaternion startRot;
    private Quaternion camStartRot;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (lerpingDown)
        {
            lerpCounter += Time.deltaTime * 2f;
            player.position = Vector3.Slerp(startPos, transform.position, lerpCounter);
            player.rotation = Quaternion.Slerp(startRot, transform.rotation, lerpCounter);
            Camera.main.transform.localRotation = Quaternion.Slerp(camStartRot, Quaternion.Euler(15f, 0f, 0f), lerpCounter);
            if (lerpCounter >= 1f)
            {
                lerpingDown = false;
            }    
        }

        if (lerpingUp)
        {
            lerpCounter += Time.deltaTime * 2f;
            player.position = Vector3.Slerp(transform.position, startPos, lerpCounter);
            Camera.main.transform.localRotation = Quaternion.Slerp(camStartRot, Quaternion.Euler(0f, 0f, 0f), lerpCounter);
            if (lerpCounter >= 1f)
            {
                player.GetComponent<FirstPersonController>().enabled = true;
                player.GetComponent<FirstPersonController>().ResetLook();
                lerpingUp = false;
            }
        }
    }

    public void Sit()
    {
        DialogueDisplay.Instance().ClosePopups();
        player.GetComponent<FirstPersonController>().enabled = false;
        player.GetComponent<FirstPersonController>().EnableAction();
        player.GetComponent<PlayerInteraction>().AssignSittingSpot(this);
        startPos = player.position;
        startRot = player.rotation;
        camStartRot = Camera.main.transform.localRotation;
        lerpCounter = 0f;
        lerpingDown = true;
    }

    public void Stand()
    {
        startPos = new Vector3(transform.position.x, 5.36f, transform.position.z) + transform.forward;
        camStartRot = Camera.main.transform.localRotation;
        lerpCounter = 0f;
        lerpingUp = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.up, Vector3.one);
    }
}
