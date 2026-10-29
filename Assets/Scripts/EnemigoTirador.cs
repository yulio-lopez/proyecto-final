using UnityEngine;
using UnityEngine.AI;

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

    private NavMeshAgent agente;
    private bool yendoAPuntoB = true;
    private float contadorEspera = 0f;
    private float contadorDisparo = 0f;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();

        agente.updateRotation = false;
        agente.updateUpAxis = false;

        agente.SetDestination(destinoB.position);
    }

    void Update()
    {
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

    void Patrullar()
    {
        if (destinoA == null || destinoB == null)
            return;

        if (Vector2.Distance(
            transform.position,
            destinoB.position) < 0.2f)
        {
            yendoAPuntoB = false;
        }

        if (Vector2.Distance(
            transform.position,
            destinoA.position) < 0.2f)
        {
            yendoAPuntoB = true;
        }

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
