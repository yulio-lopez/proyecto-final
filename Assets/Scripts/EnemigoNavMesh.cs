using UnityEngine;
using UnityEngine.AI;

public class EnemigoNavMesh : MonoBehaviour
{
    public Transform destinoA;
    public Transform destinoB;
    private NavMeshAgent agente;
    public VisionEnemigo visionEnemigo;
    private bool yendoAPuntoB = true;
    public float tiempoEspera = 2f;
    private float contador = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.updateRotation = false;
        agente.updateUpAxis = false;
        agente.SetDestination(destinoB.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (visionEnemigo.posicionJugador != null)
        {
            contador = 0f;
            agente.SetDestination(visionEnemigo.posicionJugador.position);
        }
        else
        {
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
