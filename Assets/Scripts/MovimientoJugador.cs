using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoJugador : MonoBehaviour
{
    [Header("Movimiento")]

    [Tooltip("Velocidad máxima horizontal del personaje.")]
    [SerializeField]
    private float velocidadMaxima = 5f;

    [Tooltip("Qué tan rápido alcanza la velocidad máxima.")]
    [SerializeField]
    private float aceleracion = 25f;

    [Tooltip("Qué tan rápido se detiene.")]
    [SerializeField]
    private float desaceleracion = 30f;


    [Header("Salto")]

    [Tooltip("Altura aproximada del salto.")]
    [SerializeField]
    private float alturaSalto = 2f;
    private int saltosRestantes;


    [Header("Suelo")]

    [Tooltip("Objeto ubicado bajo los pies del personaje.")]
    [SerializeField]
    private Transform groundCheck;

    [Tooltip("Radio utilizado para detectar el suelo.")]
    [SerializeField]
    private float radioGroundCheck = 0.15f;

    [Tooltip("Layers consideradas como suelo.")]
    [SerializeField]
    private LayerMask capaSuelo;

    private Rigidbody2D rb;
    private PlayerInput playerInput;

    private InputAction moverAccion;
    private InputAction saltarAccion;

    private float inputMovimiento;

    private bool saltoSolicitado;

    private bool enSuelo;

    public GameObject renderJugador;
    public Transform puntoDisparo;
    public float direccionMirada = 1;
    private float posicionDisparo;

    public Animator anim;

    public AudioSource pasos;


    [Header("Dash")]
    public float velocidadDash = 20f;
    public float duracionDash = 0.15f;
    public float tiempoEsperaDash = 1f;

    private InputAction dashAccion;
    private bool haciendoDash = false;
    private float tiempoDash;
    private float siguienteDash;


    [Header("Salto de pared")]

    [SerializeField] private LayerMask capaPared;
    [SerializeField] private float distanciaPared = 0.8f;
    [SerializeField] private float fuerzaSaltoPared = 7f;
    [SerializeField] private float impulsoHorizontalPared = 7f;
    [SerializeField] private float velocidadDeslizamiento = 2f;
    private bool tocandoPared;
    private int direccionPared;

    private void Start()
    {

        if (PlayerPrefs.HasKey("posX") && PlayerPrefs.HasKey("posY"))
        {
            Vector3 posInicio = new Vector3(PlayerPrefs.GetFloat("posX"), PlayerPrefs.GetFloat("posY"), 0);
            this.transform.position = posInicio;
        }




        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        moverAccion = playerInput.actions.FindAction("Humano/mover", true);

        saltarAccion = playerInput.actions.FindAction("Humano/saltar", true);

        dashAccion = playerInput.actions.FindAction("Humano/dash", true);

    }

    private void Update()
    {
        inputMovimiento = moverAccion.ReadValue<float>();

        if (saltarAccion.WasPressedThisFrame())
        {
            saltoSolicitado = true;
        }

        if (inputMovimiento > 0)
        {
            //derecha
            cambiarDireccion(1);
        }
        else if (inputMovimiento < 0)
        {
            //izquierda
            cambiarDireccion(-1);
        }

        if (dashAccion.WasPressedThisFrame() && !haciendoDash && Time.time >= siguienteDash)
        {
            haciendoDash = true;
            tiempoDash = duracionDash;
            siguienteDash = Time.time + tiempoEsperaDash;
        }
    }

    private void cambiarDireccion(float direccion)
    {
        direccionMirada = direccion;
        if (direccionMirada > 0)
        {
            renderJugador.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direccionMirada < 0)
        {
            renderJugador.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        Vector3 posicion = puntoDisparo.localPosition;
        posicion.x = posicionDisparo * direccion;
        puntoDisparo.localPosition = posicion;

    }



    private void FixedUpdate()
    {
        comprobarSuelo();
        comprobarPared();

        if (haciendoDash)
        {
            rb.linearVelocity = new Vector2(
                direccionMirada * velocidadDash,
                0f
            );

            tiempoDash -= Time.fixedDeltaTime;

            if (tiempoDash <= 0f)
            {
                haciendoDash = false;
            }

            return;
        }

        aplicarMovimiento();

        deslizarPared();

        procesarSalto();

        actualizarAnimacionCaminar();
        actualizarSonidoPasos();
    }


    private void aplicarMovimiento()
    {
        // Si estamos en el aire y tocamos una pared,
        // evitamos empujar al personaje contra ella.
        if (tocandoPared && !enSuelo)
        {
            if ((direccionPared == 1 && inputMovimiento > 0) ||
                (direccionPared == -1 && inputMovimiento < 0))
            {
                rb.linearVelocity = new Vector2(
                    0f,
                    rb.linearVelocity.y
                );

                return;
            }
        }

        float velocidadDeseada = inputMovimiento * velocidadMaxima;

        float velocidadActual = rb.linearVelocity.x;
        float diferenciaVelocidad = velocidadDeseada - velocidadActual;

        float aceleracionDisponible;

        if (Mathf.Abs(inputMovimiento) > 0.01f)
            aceleracionDisponible = aceleracion;
        else
            aceleracionDisponible = desaceleracion;

        float aceleracionNecesaria =
            diferenciaVelocidad / Time.fixedDeltaTime;

        aceleracionNecesaria = Mathf.Clamp(
            aceleracionNecesaria,
            -aceleracionDisponible,
            aceleracionDisponible
        );

        rb.AddForce(
            Vector2.right * aceleracionNecesaria,
            ForceMode2D.Force
        );
    }


    private void comprobarSuelo()
    {
        Collider2D suelo = Physics2D.OverlapCircle(groundCheck.position, radioGroundCheck, capaSuelo);

        enSuelo = suelo != null;

        Debug.Log("En suelo: " + enSuelo);
    }

    private void comprobarPared()
    {
        RaycastHit2D derecha = Physics2D.Raycast(
            transform.position,
            Vector2.right,
            distanciaPared,
            capaPared
        );

        RaycastHit2D izquierda = Physics2D.Raycast(
            transform.position,
            Vector2.left,
            distanciaPared,
            capaPared
        );

        tocandoPared = derecha.collider != null ||
                       izquierda.collider != null;

        if (derecha.collider != null)
        {
            direccionPared = 1;
        }
        else if (izquierda.collider != null)
        {
            direccionPared = -1;
        }
    }



    private void deslizarPared()
    {
        if (tocandoPared && !enSuelo && !haciendoDash)
        {
            if (rb.linearVelocity.y < -velocidadDeslizamiento)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    -velocidadDeslizamiento
                );
            }
        }
    }


    private void procesarSalto()
    {
        if (enSuelo)
        {
            saltosRestantes = 2;
        }

        if (saltoSolicitado)
        {
            if (enSuelo)
            {
                saltar();
                saltosRestantes = 1;
            }
            else if (tocandoPared)
            {
                rb.linearVelocity = new Vector2(
                    -direccionPared * impulsoHorizontalPared,
                    fuerzaSaltoPared
                );

                saltosRestantes = 1;
            }
            else if (saltosRestantes > 0)
            {
                saltar();
                saltosRestantes--;
            }
        }

        saltoSolicitado = false;
    }


    private void saltar()
    {
        if (rb.linearVelocity.y < 0f)
        {
            Vector2 velocidad = rb.linearVelocity;

            velocidad.y = 0f;

            rb.linearVelocity = velocidad;
        }

        float gravedadEfectiva = Physics2D.gravity.y * rb.gravityScale;


        float velocidadSalto = Mathf.Sqrt(alturaSalto * -2f * gravedadEfectiva);

        float impulso = rb.mass * velocidadSalto;

        rb.AddForce(Vector2.up * impulso, ForceMode2D.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }


        Gizmos.DrawWireSphere(groundCheck.position, radioGroundCheck);
    }

    private void actualizarAnimacionCaminar()
    {
        bool saltando = !enSuelo;

        anim.SetBool("saltando", saltando);

        if (enSuelo)
        {
            anim.SetInteger("caminar", Mathf.Abs(rb.linearVelocity.x) > 0.1f ? 1 : 0);
        }

    }


    private void actualizarSonidoPasos()
    {
        bool estaCaminando = Mathf.Abs(rb.linearVelocity.x) > 0;


        if (estaCaminando)
        {
            if (!pasos.isPlaying)
            {
                pasos.Play();
            }
        }
        else
        {
            if (pasos.isPlaying)
            {
                pasos.Stop();
            }
        }

    }
}
