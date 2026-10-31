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

    private const float DistanciaLlegadaPatrulla = 0.2f;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        agente.updateRotation = false;
        agente.updateUpAxis = false;
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

        agente.SetDestination(destino.position);
        destinoPatrullaActual = destino;
    }
}
