
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float jumpForce = 3f;
    public float fireRange = 5f;
    public GameObject explosion;

    private PlayerInputAction mInputAction;
    private InputAction mMovementAction;
    private Rigidbody mRigidbody;
    private Transform mFirePoint;
    private bool jumpPressed = false;
    private bool onGround = true;

    private void Awake()
    {
        mInputAction = new PlayerInputAction();
        mRigidbody = GetComponent<Rigidbody>();
        mFirePoint = transform.Find("FirePoint");
    }

    private void OnEnable()
    {
        // Codigo que se ejecutara al habilitar un GO
        mInputAction.Player.Jump.performed += DoJump;
        mInputAction.Player.Jump.Enable();

        mInputAction.Player.Fire.performed += DoFire;
        mInputAction.Player.Fire.Enable();

        mMovementAction = mInputAction.Player.Movement;
        mMovementAction.Enable();

    }

    private void DoFire(InputAction.CallbackContext obj)
    {
        // Lanzar un raycast
        RaycastHit hit;

        if (Physics.Raycast(
            mFirePoint.position,
            transform.forward,
            out hit,
            fireRange
        ))
        {
            // Hubo una colision
            Debug.Log(hit.collider.name);
            GameObject nuevaExplosion = 
                Instantiate(explosion, hit.point, Quaternion.identity);
            Destroy(nuevaExplosion, 1f);
        }

        Debug.DrawRay(mFirePoint.position,
            transform.forward * fireRange,
            Color.red,
            .25f
        );
    }

    private void OnDisable()
    {
        // Codigo que se ejecutara al deshabilitar un GO
        mInputAction.Player.Jump.Disable();
        mMovementAction.Disable();
        mInputAction.Disable();
    }

    private void Update()
    {
        Vector2 movement = Vector2.ClampMagnitude(
            mMovementAction.ReadValue<Vector2>(),
            1f
        );
        mRigidbody.velocity = new Vector3(
            movement.x * moveSpeed,
            mRigidbody.velocity.y,
            movement.y * moveSpeed
        );

        if (jumpPressed && onGround)
        {
            mRigidbody.velocity += Vector3.up * jumpForce;
            jumpPressed = false;
            onGround = false;
        }
    }

    private void DoJump(InputAction.CallbackContext obj)
    {
        jumpPressed = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision");
        onGround = true;
        jumpPressed = false;
    }

}
