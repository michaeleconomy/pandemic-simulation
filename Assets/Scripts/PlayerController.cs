using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {
    // Movement speeds
    [Header("Speed Variables")]
    [SerializeField]
    private float m_MoveSpeed = 5.0f;
    [SerializeField]
    private float m_JumpForce = 5.0f;
    [SerializeField]
    private float m_GravityForce = 9.807f;

    private Camera m_Camera;
    private CharacterController m_CharacterController;

    public GameObject heldObject;

    // Look sensitivity variable
    [Range(0.0f, 5.0f)]
    public float m_LookSensitivity = 1.0f;

    public float throwForce = 100;

    private float m_MouseX;
    private float m_MouseY;

    [SerializeField]
    private Vector3 m_MoveDirection;

    public float stuckAfter = 1, stuckDistance;
    public float stuckDuration = 0;
    private Vector3 startPos, lastPos;

    private void Awake() {
        m_Camera = GetComponentInChildren<Camera>();
        m_CharacterController = GetComponent<CharacterController>();
        startPos = transform.position;
    }

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        Rotate();
        Movement();
    }

    private void Rotate() {
        // Receive mouse input and modifies
        m_MouseX += Input.GetAxisRaw("Mouse X") * m_LookSensitivity;
        m_MouseY += Input.GetAxisRaw("Mouse Y") * m_LookSensitivity;

        // Keep mouseY between -90 and +90
        m_MouseY = Mathf.Clamp(m_MouseY, -90.0f, 90.0f);

        // Rotate the player object around the y-axis
        transform.localRotation = Quaternion.Euler(Vector3.up * m_MouseX);
        // Rotate the camera around the x-axis
        m_Camera.transform.localRotation = Quaternion.Euler(Vector3.left * m_MouseY);
    }

    private void Movement() {
        // If the player is touching the ground
        if (m_CharacterController.isGrounded) {
            // Receive user input for movement
            Vector3 forwardMovement = transform.forward * Input.GetAxisRaw("Vertical");
            Vector3 strafeMovement = transform.right * Input.GetAxisRaw("Horizontal");
            // Convert Input into a Vector3
            m_MoveDirection = (forwardMovement + strafeMovement).normalized * m_MoveSpeed;
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1)) {
                m_MoveDirection.y = m_JumpForce; // Jump
            }
            if (Input.GetAxisRaw("Vertical") > 0) {
                if (Vector3.Distance(lastPos, transform.position) < stuckDistance) {
                    stuckDuration += Time.deltaTime;
                    if (stuckDuration >= stuckAfter) {
                        transform.position = startPos;
                        stuckDuration = 0;
                        return;
                    }
                }
                lastPos = transform.position;
            }
            else {
                stuckDuration = 0;
            }

            if (Input.GetKeyDown(KeyCode.LeftControl) ||
                Input.GetKeyDown(KeyCode.LeftCommand) ||
                Input.GetKeyDown(KeyCode.E) ||
                Input.GetMouseButtonDown(0)) {
                if (heldObject == null) {
                    PickUp();
                }
                else {
                    ThrowItem();
                }
            }
        }

        // Calculate gravity and modify movement vector as such
        m_MoveDirection.y -= m_GravityForce * Time.deltaTime;

        // Move the player using the movement vector
        m_CharacterController.Move(m_MoveDirection * Time.deltaTime);
    }

    private void PickUp() {
        if (!Physics.Raycast(m_Camera.transform.position,
            m_Camera.transform.TransformDirection(Vector3.forward),
            out var hit, 3)) {
            return;
        }
        var hitItem = hit.collider;
        var rigidBody = hit.collider.GetComponent<Rigidbody>();
        if (rigidBody == null) {
            return;
        }
        Destroy(rigidBody);
        hitItem.transform.SetParent(m_Camera.transform);
        heldObject = hitItem.gameObject;
    }


    private void ThrowItem() {
        heldObject.transform.SetParent(null);
        var rigidBody = heldObject.AddComponent<Rigidbody>();

        var cameraDirection = m_Camera.transform.TransformDirection(Vector3.forward);
        rigidBody.AddForce(cameraDirection * throwForce);
        heldObject = null;
    }
}
