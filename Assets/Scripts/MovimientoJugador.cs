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

        //Debug.Log("Mover: " + moverAccion);
        //Debug.Log("Saltar: " + saltarAccion);

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

        aplicarMovimiento();

        procesarSalto();

        actualizarAnimacionCaminar();

        actualizarSonidoPasos();
    }

    private void aplicarMovimiento()
    {
        float velocidadDeseada = inputMovimiento * velocidadMaxima;

        float velocidadActual = rb.linearVelocity.x;

        float diferenciaVelocidad = velocidadDeseada - velocidadActual;

        float aceleracionDisponible;


        if (Mathf.Abs(inputMovimiento) > 0.01f)
        {
            aceleracionDisponible = aceleracion;
        }
        else
        {
            aceleracionDisponible = desaceleracion;
        }

        float aceleracionNecesaria = diferenciaVelocidad / Time.fixedDeltaTime;

        aceleracionNecesaria = Mathf.Clamp(aceleracionNecesaria, -aceleracionDisponible, aceleracionDisponible);

        rb.AddForce(Vector2.right * aceleracionNecesaria, ForceMode2D.Force);
    }

    private void comprobarSuelo()
    {
        Collider2D suelo = Physics2D.OverlapCircle(groundCheck.position, radioGroundCheck, capaSuelo);

        enSuelo = suelo != null;

        Debug.Log("En suelo: " + enSuelo);
    }

    private void procesarSalto()
    {

        if (saltoSolicitado && enSuelo)
        {
            saltar();
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


        // v = sqrt(-2gh)

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


        // Nos permite ver en Scene
        // dónde está el detector del suelo.

        Gizmos.DrawWireSphere(groundCheck.position, radioGroundCheck);
    }

    private void actualizarAnimacionCaminar()
    {
        // Comprobar si el personaje está en el aire
        bool saltando = !enSuelo;

        // Actualizar la animación de salto
        anim.SetBool("saltando", saltando);

        // Actualizar la animación de caminar
        if (enSuelo)
        {
            anim.SetInteger("caminar", Mathf.Abs(rb.linearVelocity.x) > 0.1f ? 1 : 0);
        }

        //anim.SetInteger("caminar", (int)rb.linearVelocity.x);


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
