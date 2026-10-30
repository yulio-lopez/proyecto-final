using UnityEngine;
using UnityEngine.AI;

public class EnemigoNavMesh : MonoBehaviour
{
    public Transform destinoA;
    public Transform destinoB;
    private NavMeshAgent agente;
    private SpriteRenderer spriteRenderer;
    public VisionEnemigo visionEnemigo;
    private bool yendoAPuntoB = true;
    private bool persiguiendoJugador;
    private float alturaPersecucion;
    public float tiempoEspera = 2f;
    private float contador = 0f;
    private Transform destinoPatrullaActual;
    private Vector3 ultimaPosicionPatrulla;
    private float tiempoSinAvance;

    private const float DistanciaLlegadaPatrulla = 0.2f;
    private const float TiempoMaximoAtascado = 1.5f;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        agente.updateRotation = false;
        agente.updateUpAxis = false;
        ultimaPosicionPatrulla = transform.position;
        EstablecerDestinoPatrulla(destinoB);
    }

    void Update()
    {
        if (agente.velocity.x > 0.1f)
        {
            spriteRenderer.flipX = true;
        }
        else if (agente.velocity.x < -0.1f)
        {
            spriteRenderer.flipX = false;
        }

        if (visionEnemigo.posicionJugador != null)
        {
            contador = 0f;
            if (!persiguiendoJugador)
            {
                alturaPersecucion = transform.position.y;
                persiguiendoJugador = true;
                destinoPatrullaActual = null;
                tiempoSinAvance = 0f;
            }

            Vector3 destinoJugador = visionEnemigo.posicionJugador.position;
            destinoJugador.y = alturaPersecucion;
            agente.SetDestination(destinoJugador);
        }
        else
        {
            if (persiguiendoJugador)
            {
                persiguiendoJugador = false;
                destinoPatrullaActual = null;
                tiempoSinAvance = 0f;
            }

            contador += Time.deltaTime;
            if (contador >= tiempoEspera)
            {
                Patrullar();
            }
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
        {
            ultimaPosicionPatrulla = transform.position;
            tiempoSinAvance = 0f;
            return;
        }

        float toleranciaLlegada = Mathf.Max(
            agente.stoppingDistance,
            DistanciaLlegadaPatrulla
        );
        bool llegoAlDestino =
            Vector2.Distance(transform.position, destino.position) <
                DistanciaLlegadaPatrulla ||
            (agente.hasPath &&
             agente.remainingDistance <= toleranciaLlegada);

        if (llegoAlDestino)
        {
            CambiarDestinoPatrulla();
            return;
        }

        float avance = Vector2.Distance(
            transform.position,
            ultimaPosicionPatrulla
        );
        ultimaPosicionPatrulla = transform.position;

        if (avance > 0.01f)
        {
            tiempoSinAvance = 0f;
            return;
        }

        tiempoSinAvance += Time.deltaTime;
        if (tiempoSinAvance >= TiempoMaximoAtascado)
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

        agente.SetDestination(destino.position);
        destinoPatrullaActual = destino;
        ultimaPosicionPatrulla = transform.position;
        tiempoSinAvance = 0f;
    }
}
