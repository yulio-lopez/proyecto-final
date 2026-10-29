
using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoJugador : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidadMaxima = 5f;
    [SerializeField] private float aceleracion = 25f;
    [SerializeField] private float desaceleracion = 30f;

    [Header("Salto")]
    [SerializeField] private float alturaSalto = 2f;
    [SerializeField] private float tiempoCoyote = 0.15f;
    [SerializeField] private float tiempoBufferSalto = 0.15f;

    private int saltosRestantes = 2;
    private float tiempoDesdeSuelo;
    private float tiempoDesdePulsacionSalto;
    private bool saltoInicialRealizado;

    [Header("Suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float radioGroundCheck = 0.15f;
    [SerializeField] private LayerMask capaSuelo;

    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private InputAction moverAccion;
    private InputAction saltarAccion;

    private float inputMovimiento;
    private bool enSuelo;
    private bool estabaEnSuelo;

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
    private bool haciendoDash;
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
        if (AdministradorCheckPoint.instancia != null &&
            AdministradorCheckPoint.instancia.ExisteCheckpoint())
        {
            string escenaGuardada =
                PlayerPrefs.GetString("escenaCheckpoint", "");

            if (escenaGuardada == UnityEngine.SceneManagement.SceneManager
                .GetActiveScene().name)
            {
                transform.position =
                    AdministradorCheckPoint.instancia.ObtenerCheckpoint();
            }
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
            tiempoDesdePulsacionSalto = tiempoBufferSalto;
        }

        if (inputMovimiento > 0)
        {
            cambiarDireccion(1);
        }
        else if (inputMovimiento < 0)
        {
            cambiarDireccion(-1);
        }

        if (dashAccion.WasPressedThisFrame()
            && !haciendoDash
            && Time.time >= siguienteDash)
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
        else
        {
            renderJugador.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        Vector3 posicion = puntoDisparo.localPosition;
        posicion.x = posicionDisparo * direccion;
        puntoDisparo.localPosition = posicion;
    }

    private void FixedUpdate()
    {
        estabaEnSuelo = enSuelo;

        comprobarSuelo();
        comprobarPared();

        // Reiniciar los saltos únicamente al aterrizar.
        if (enSuelo && !estabaEnSuelo)
        {
            saltosRestantes = 2;
            saltoInicialRealizado = false;
        }

        // Coyote time.
        if (enSuelo)
        {
            tiempoDesdeSuelo = tiempoCoyote;
        }
        else
        {
            tiempoDesdeSuelo -= Time.fixedDeltaTime;
        }

        // Si termina el coyote time sin haber saltado,
        // se consume el salto inicial.
        if (!enSuelo
            && tiempoDesdeSuelo <= 0f
            && !saltoInicialRealizado)
        {
            saltoInicialRealizado = true;
            saltosRestantes = 1;
        }

        // Jump buffering.
        tiempoDesdePulsacionSalto -= Time.fixedDeltaTime;

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
        if (tocandoPared && !enSuelo)
        {
            if ((direccionPared == 1 && inputMovimiento > 0)
                || (direccionPared == -1 && inputMovimiento < 0))
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

        float aceleracionDisponible =
            Mathf.Abs(inputMovimiento) > 0.01f
            ? aceleracion
            : desaceleracion;

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
        Collider2D suelo = Physics2D.OverlapCircle(
            groundCheck.position,
            radioGroundCheck,
            capaSuelo
        );

        enSuelo = suelo != null;
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

        tocandoPared = derecha.collider != null
            || izquierda.collider != null;

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
        if (tiempoDesdePulsacionSalto <= 0f)
            return;

        // Salto normal o coyote time.
        if (!saltoInicialRealizado
            && (enSuelo || tiempoDesdeSuelo > 0f))
        {
            saltar();

            saltoInicialRealizado = true;
            saltosRestantes = 1;

            tiempoDesdeSuelo = 0f;
            tiempoDesdePulsacionSalto = 0f;
        }
        // Salto de pared.
        else if (!enSuelo && tocandoPared)
        {
            rb.linearVelocity = new Vector2(
                -direccionPared * impulsoHorizontalPared,
                fuerzaSaltoPared
            );

            saltoInicialRealizado = true;
            saltosRestantes = 1;

            tiempoDesdeSuelo = 0f;
            tiempoDesdePulsacionSalto = 0f;
        }
        // Doble salto.
        else if (!enSuelo && saltosRestantes > 0)
        {
            saltar();

            saltosRestantes--;
            tiempoDesdePulsacionSalto = 0f;
        }
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

        float velocidadSalto = Mathf.Sqrt(
            alturaSalto * -2f * gravedadEfectiva
        );

        float impulso = rb.mass * velocidadSalto;

        rb.AddForce(
            Vector2.up * impulso,
            ForceMode2D.Impulse
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            radioGroundCheck
        );
    }

    private void actualizarAnimacionCaminar()
    {
        bool saltando = !enSuelo;
        anim.SetBool("saltando", saltando);

        if (enSuelo)
        {
            anim.SetInteger(
                "caminar",
                Mathf.Abs(rb.linearVelocity.x) > 0.1f ? 1 : 0
            );
        }
    }

    private void actualizarSonidoPasos()
    {
        bool estaCaminando = enSuelo
            && Mathf.Abs(rb.linearVelocity.x) > 0.1f;

        if (estaCaminando)
        {
            if (!pasos.isPlaying)
                pasos.Play();
        }
        else
        {
            if (pasos.isPlaying)
                pasos.Stop();
        }
    }
}
