
using UnityEngine;

public class PlataformaMovil : MonoBehaviour
{
    public Transform[] puntosDestino;
    public int indiceDestino = 0;
    public float velocidad = 5f;

    private int sumador = 1;
    private Vector3 posicionAnterior;

    private Rigidbody2D rbPlataforma;
    private Transform jugadorEncima;

    void Start()
    {
        if (puntosDestino == null || puntosDestino.Length < 2)
        {
            Debug.LogError("La plataforma necesita al menos 2 puntos.");
            enabled = false;
            return;
        }

        indiceDestino = Mathf.Clamp(
            indiceDestino, 0, puntosDestino.Length - 1
        );

        rbPlataforma = GetComponent<Rigidbody2D>();
        posicionAnterior = transform.position;
    }

    void FixedUpdate()
    {
        Vector3 posicionNueva = Vector3.MoveTowards(
            transform.position,
            puntosDestino[indiceDestino].position,
            velocidad * Time.fixedDeltaTime
        );

        Vector3 desplazamiento = posicionNueva - transform.position;

        transform.position = posicionNueva;

        if (jugadorEncima != null)
        {
            jugadorEncima.position += desplazamiento;
        }

        if (Vector3.Distance(
            transform.position,
            puntosDestino[indiceDestino].position) < 0.05f)
        {
            indiceDestino += sumador;

            if (indiceDestino >= puntosDestino.Length)
            {
                sumador = -1;
                indiceDestino = puntosDestino.Length - 2;
            }
            else if (indiceDestino < 0)
            {
                sumador = 1;
                indiceDestino = 1;
            }
        }

        posicionAnterior = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D contacto in collision.contacts)
            {
                // El jugador está sobre la parte superior
                if (contacto.normal.y < -0.5f)
                {
                    jugadorEncima = collision.transform;
                    break;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform == jugadorEncima)
        {
            jugadorEncima = null;
        }
    }
}
