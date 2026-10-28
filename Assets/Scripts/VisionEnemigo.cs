using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class VisionEnemigo : MonoBehaviour
{
    public float distanciaVision = 5f;
    public LayerMask capasVision;

    public Transform posicionJugador;

    private NavMeshAgent agente;

    private Vector2 ultimaDireccion = Vector2.right;

    private float posicionX;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agente = GetComponentInParent<NavMeshAgent>();
        posicionX = Mathf.Abs(transform.localPosition.x);
    }

    // Update is called once per frame
    void Update()
    {
        detectarJugador();
    }

    private void detectarJugador()
    {
        Vector2 direccion;

        if (agente.velocity.x > 0.1f)
        {
            direccion = Vector2.right;
            ultimaDireccion = direccion;

            transform.localPosition = new Vector3(
                posicionX,
                transform.localPosition.y,
                transform.localPosition.z
            );

            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (agente.velocity.x < -0.1f)
        {
            direccion = Vector2.left;
            ultimaDireccion = direccion;

            transform.localPosition = new Vector3(
                -posicionX,
                transform.localPosition.y,
                transform.localPosition.z
            );

            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            direccion = ultimaDireccion;
        }
        RaycastHit2D impacto = Physics2D.Raycast(this.transform.position, direccion, distanciaVision, capasVision);

        Debug.DrawRay(this.transform.position, direccion * distanciaVision, Color.black);
        if (impacto.collider != null)
        {
            if (impacto.collider.CompareTag("Player"))
            {
                posicionJugador = impacto.collider.transform;
            } 
            else
            {
                posicionJugador = null;
            }
        }
        else
        {
            posicionJugador = null;
        }
    }
}
