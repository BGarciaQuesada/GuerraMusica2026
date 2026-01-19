using UnityEngine;
using UnityEngine.InputSystem; 

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")] 
    public float moveSpeed = 5f; 

    [Header("Salto / Gravedad")] 
    public float jumpHeight = 2f;

    
    public float gravity = -9.81f;                          // Valor de la gravedad, negativo para empujar hacia abajo.

    private CharacterController characterController; 

    private Vector2 moveInput;                              // Almacena el input de movimiento (X = izquierda/derecha, Y = adelante/atrás).
    private float verticalVelocity;                         // Velocidad vertical actual (para salto y caída).
    private bool jumpRequested = false;                     // Indica si el jugador ha pedido un salto (pulsando el botón de salto).


    [SerializeField] private AudioSource audioSourceSalto;
    [SerializeField] private AudioSource audioSourcePasos;
    [SerializeField] private int minSpeed = 1;              // velocidad mínima sonido pasos

    private Animaciones animacion;


    /// vertical velocity
    public float VerticalSpeed => verticalVelocity;

    private void Awake() 
    {
        characterController =
            GetComponent<CharacterController>();
        // animacion = GameObject.FindObjectOfType<Animaciones>();
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        if (value.isPressed)
            jumpRequested = true;
    }

    private void Update()
    {
        if (characterController == null)                    // Si por alguna razón no hay CharacterController...
            return;                                         // ...no hacemos nada para evitar errores.

        HandleMovement();
        SonidoPasos();


    }


    private void HandleMovement()
    {
        bool isGrounded = characterController.isGrounded;   // Comprueba si el CharacterController está tocando el suelo.

        // Reset vertical al tocar suelo
        if (isGrounded && verticalVelocity < 0f)            // Si estamos en el suelo y la velocidad vertical es hacia abajo...
            verticalVelocity = -2f;                         // ...ponemos un pequeño valor negativo para mantenerlo pegado al suelo.

        // Movimiento local XZ
        Vector3
            localMove = new Vector3(moveInput.x, 0f,
                moveInput.y);                               // Crea un vector de movimiento en el espacio local (X y Z, sin Y).

        // Convertir de local a mundo según la orientación del player (yaw la controla PlayerLook)
        Vector3 worldMove = transform
                .TransformDirection(
                    localMove);                             // Convierte el vector local a espacio global usando la rotación del jugador.

        // Normalizar para que la diagonal no sea más rápida
        if (worldMove.sqrMagnitude > 1f)                    // Si la magnitud al cuadrado es mayor que 1 (movimiento diagonal fuerte)...
            worldMove.Normalize();                          // ...normaliza el vector para que la velocidad en diagonal sea uniforme.

        Vector3
            horizontalVelocity =
                worldMove * moveSpeed;

        // Salto
        if (isGrounded && jumpRequested)                    // Si está en el suelo y se ha pedido un salto...
        {
            if (audioSourceSalto != null)
                audioSourceSalto.Play();

            /// animacion
            ///

            animacion.TriggerSalto();

            // Calcula la velocidad vertical inicial necesaria para alcanzar la altura de salto deseada usando la fórmula de física.
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

            // Resetea la petición de salto.
            jumpRequested = false;                          
        }

        // Gravedad
        verticalVelocity += gravity * Time.deltaTime;       // Aplica la gravedad acumulándola en la velocidad vertical cada frame.

        Vector3 velocity = horizontalVelocity;
        velocity.y = verticalVelocity;

        // Mueve el CharacterController según la velocidad total (horizontal + vertical) multiplicada por deltaTime.
        characterController.Move(velocity * Time.deltaTime);        
    }

    private void SonidoPasos()
    {
        if (audioSourcePasos == null) return;

        // Velocidad horizontal (ignora saltos/caídas)
        Vector3 v = characterController.velocity;
        v.y = 0f;
        // Debug.Log(v.magnitude);

        bool andando = characterController.isGrounded && v.magnitude > minSpeed;

        if (andando)
        {
            if (!audioSourcePasos.isPlaying)
                audioSourcePasos.Play();
        }
        else
        {
            if (audioSourcePasos.isPlaying)
                audioSourcePasos.Stop();
        }
    }
}