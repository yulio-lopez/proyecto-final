using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class EnemigoTirador : MonoBehaviour
{
    [Header("Referencias")]
    public VisionEnemigo visionEnemigo;
    public GameObject prefabProyectil;
    public Transform puntoDisparo;

    [Header("Disparo")]
    public float tiempoEntreDisparos = 1f;
    public float distanciaDisparo = 10f;

    [Header("Patrulla")]
    public Transform destinoA;
    public Transform destinoB;
    public float tiempoEspera = 1f;

    [Header("Audio de proximidad")]
    [SerializeField, Min(0f)] private float distanciaAudioCercania = 8f;

    private NavMeshAgent agente;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioCercania;
    private Transform jugador;
    private bool yendoAPuntoB = true;
    private float contadorEspera = 0f;
    private float contadorDisparo = 0f;
    private Transform destinoPatrullaActual;

    private const float DistanciaLlegadaPatrulla = 0.2f;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        audioCercania = GetComponent<AudioSource>();
        audioCercania.loop = true;

        GameObject jugadorObjeto = GameObject.FindGameObjectWithTag("Player");
        if (jugadorObjeto != null)
        {
            jugador = jugadorObjeto.transform;
        }

        agente.updateRotation = false;
        agente.updateUpAxis = false;

        EstablecerDestinoPatrulla(destinoB);
    }

    void Update()
    {
        ActualizarAudioProximidad();

        if (agente.velocity.x > 0.1f)
        {
            spriteRenderer.flipX = true;
        }
        else if (agente.velocity.x < -0.1f)
        {
            spriteRenderer.flipX = false;
        }

        if (visionEnemigo == null ||
            visionEnemigo.posicionJugador == null)
        {
            // No ve al jugador: volver a patrullar.
            agente.isStopped = false;
            contadorDisparo = 0f;

            contadorEspera += Time.deltaTime;

            if (contadorEspera >= tiempoEspera)
            {
                Patrullar();
            }

            return;
        }

        Transform jugador = visionEnemigo.posicionJugador;

        // El enemigo ve al jugador y se detiene.
        contadorEspera = 0f;
        destinoPatrullaActual = null;
        agente.isStopped = true;
        agente.ResetPath();

        // Comprobar la distancia antes de disparar.
        float distancia = Vector2.Distance(
            transform.position,
            jugador.position
        );

        if (distancia <= distanciaDisparo)
        {
            contadorDisparo += Time.deltaTime;

            if (contadorDisparo >= tiempoEntreDisparos)
            {
                Disparar(jugador);
                contadorDisparo = 0f;
            }
        }
        else
        {
            contadorDisparo = 0f;
        }
    }

    private void ActualizarAudioProximidad()
    {
        if (jugador == null)
            return;

        float distancia = Vector2.Distance(
            transform.position,
            jugador.position
        );
        bool jugadorCerca = distancia <= distanciaAudioCercania;

        if (jugadorCerca && !audioCercania.isPlaying)
        {
            audioCercania.Play();
        }
        else if (!jugadorCerca && audioCercania.isPlaying)
        {
            audioCercania.Stop();
        }
    }

    void Disparar(Transform jugador)
    {
        if (prefabProyectil == null || puntoDisparo == null)
            return;

        Vector2 direccion =
            ((Vector2)jugador.position -
             (Vector2)puntoDisparo.position).normalized;

        GameObject proyectil = Instantiate(
            prefabProyectil,
            puntoDisparo.position,
            Quaternion.identity
        );

        ProyectilEnemigo script =
            proyectil.GetComponent<ProyectilEnemigo>();

        if (script != null)
        {
            script.Disparar(direccion);
        }
    }

    private void Patrullar()
    {
        Transform destino = yendoAPuntoB ? destinoB : destinoA;
        if (destino == null)
            return;

        if (destinoPatrullaActual != destino)
        {
            EstablecerDestinoPatrulla(destino);
            return;
        }

        if (agente.pathPending)
            return;

        float toleranciaLlegada = Mathf.Max(
            agente.stoppingDistance,
            DistanciaLlegadaPatrulla
        );
        bool llegoAlDestino =
            Vector2.Distance(transform.position, destino.position) <
                DistanciaLlegadaPatrulla ||
            (agente.hasPath &&
             agente.pathStatus == NavMeshPathStatus.PathComplete &&
             agente.remainingDistance <= toleranciaLlegada);

        if (llegoAlDestino)
            CambiarDestinoPatrulla();
    }

    private void CambiarDestinoPatrulla()
    {
        yendoAPuntoB = !yendoAPuntoB;
        EstablecerDestinoPatrulla(yendoAPuntoB ? destinoB : destinoA);
    }

    private void EstablecerDestinoPatrulla(Transform destino)
    {
        if (destino == null || !agente.isOnNavMesh)
            return;

        agente.isStopped = false;
        agente.SetDestination(destino.position);
        destinoPatrullaActual = destino;
    }
}
