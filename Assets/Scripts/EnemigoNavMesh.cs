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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        agente.updateRotation = false;
        agente.updateUpAxis = false;
        agente.SetDestination(destinoB.position);
    }

    // Update is called once per frame
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
            }

            Vector3 destinoJugador = visionEnemigo.posicionJugador.position;
            destinoJugador.y = alturaPersecucion;
            agente.SetDestination(destinoJugador);
        }
        else
        {
            persiguiendoJugador = false;
            contador += Time.deltaTime;
            if (contador >= tiempoEspera)
            {

                Patrullar();
            }
        }
    }

    void Patrullar()
    {
        // Si llegó al punto B
        if (yendoAPuntoB && Vector2.Distance(transform.position, destinoB.position) < 0.2f)
        {
            yendoAPuntoB = false;
        }

        // Si llegó al punto A
        if (!yendoAPuntoB && Vector2.Distance(transform.position, destinoA.position) < 0.2f)
        {
            yendoAPuntoB = true;
        }

        // Elegir destino
        if (yendoAPuntoB)
        {
            agente.SetDestination(destinoB.position);
        }
        else
        {
            agente.SetDestination(destinoA.position);
        }
    }
}
