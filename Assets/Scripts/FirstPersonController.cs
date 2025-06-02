using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;

    [Header("Look Sensitivity")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float upDownRange = 80.0f;

    [Header("Input Actions")]
    [SerializeField] private InputActionAsset PlayerControls;

    private Vector3 currentMovement = Vector3.zero;
    private CharacterController characterController;
    private float verticalRotation;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction interactAction;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private PlayerInteraction interact;

    private bool movementLocked = false;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        interact = GetComponent<PlayerInteraction>();

        moveAction = PlayerControls.FindActionMap("Player").FindAction("Movement");
        lookAction = PlayerControls.FindActionMap("Player").FindAction("Look");
        interactAction = PlayerControls.FindActionMap("Player").FindAction("Interact");

        moveAction.performed += context => moveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => moveInput = Vector2.zero;

        lookAction.performed += context => lookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => lookInput = Vector2.zero;

        interactAction.performed += HandleInteractInput;

        Cursor.visible = false;
    }

    private void OnEnable()
    {
        GetComponent<CapsuleCollider>().enabled = true;
        moveAction.Enable();
        lookAction.Enable();
        interactAction.Enable();
    }

    public void ResetLook()
    {
        verticalRotation = 0f;
    }

    private void OnDisable()
    {
        GetComponent<CapsuleCollider>().enabled = false;
        moveAction.Disable();
        lookAction.Disable();
        interactAction.Disable();
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {

        float verticalSpeed = moveInput.y * walkSpeed;
        float horizontalSpeed = moveInput.x * walkSpeed;

        Vector3 horizontalMovement = new Vector3(horizontalSpeed, 0, verticalSpeed);
        horizontalMovement = transform.rotation * horizontalMovement;

        currentMovement.x = horizontalMovement.x;
        currentMovement.z = horizontalMovement.z;

        characterController.Move(currentMovement * Time.deltaTime);
    }

    void HandleRotation()
    {
        float mouseXRotation = lookInput.x * mouseSensitivity;
        transform.Rotate(0, mouseXRotation, 0);

        verticalRotation -= lookInput.y * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        Debug.Log(lookInput.y);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void HandleInteractInput(InputAction.CallbackContext context)
    {
        interact.Interact();
    }
}
