using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    CharacterController characterController;
    Animator animator;

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float acceleration = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("Ground Check")]
    public Transform groundCheck;           // Un objeto vacío a los pies que sirve para comprobar si el personaje está en el suelo.
    public float groundDistance = 0.25f;
    public LayerMask groundMask;            // Capa de suelo

    bool isGrounded;

    Vector2 moveInput;
    Vector3 velocity;
    bool isWalking;                         // Cómo lo general es que corra... Pues que lo haga automático y que el jugador decida si quiere andar

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // [!] A lo mejor es por estar aquí desde las 5AM, pero no logro hacer que lo de el mantener el botón funcione de otra forma. Voy a comprobar aquí si está pulsado y preguntar luego.........
        isWalking = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;


        HandleMovement();
        HandleGravityAndJump();
        UpdateAnimator();
        CheckGround();

        // Debug.Log(isWalking);
    }

    // ================= MOVEMENT =================

    void HandleMovement()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        float speed = isWalking ? walkSpeed : runSpeed;

        characterController.Move(move * speed * Time.deltaTime);
    }

    // ================= GRAVITY & JUMP =================

    void HandleGravityAndJump()
    {
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        animator.SetFloat("VerticalVelocity", velocity.y);
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        animator.SetBool("IsGrounded", isGrounded);
    }


    // ================= ANIMATOR =================

    void UpdateAnimator()
    {
        float mult = isWalking ? 0.5f : 1f;

        animator.SetFloat("X", moveInput.x * mult, 0.1f, Time.deltaTime);
        animator.SetFloat("Y", moveInput.y * mult, 0.1f, Time.deltaTime);
    }

    // ================= INPUT SYSTEM =================

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnWalk(InputValue value)
    {
        isWalking = value.isPressed;
    }

    public void OnJump(InputValue value)
    {
        if (!value.isPressed) return;

        if (characterController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
