using UnityEngine;

public class ProyectilEnemigo : MonoBehaviour
{
    public float velocidad = 6f;
    public float tiempoVida = 4f;

    private Rigidbody2D rb;
    private bool disparado = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Disparar(Vector2 direccion)
    {
        if (rb == null)
        {
            Debug.LogError(
                "El proyectil necesita un Rigidbody2D."
            );
            Destroy(gameObject);
            return;
        }

        disparado = true;
        Vector2 direccionNormalizada = direccion.normalized;

        if (direccionNormalizada.sqrMagnitude > 0f)
        {
            float angulo = Mathf.Atan2(
                direccionNormalizada.y,
                direccionNormalizada.x
            ) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angulo);
        }

        rb.linearVelocity = direccionNormalizada * velocidad;

        Destroy(gameObject, tiempoVida);
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (!disparado)
            return;

        if (otro.CompareTag("Player"))
        {
            VidaJugador vida = otro.GetComponent<VidaJugador>();

            if (vida != null)
            {
                ObjetoDanio dano =
                    GetComponent<ObjetoDanio>();

                if (dano != null)
                {
                    vida.recibirDano(dano.dano);
                }
                else
                {
                    vida.recibirDano(1);
                }
                vida.AplicarRetroceso(transform.position);
            }

            Destroy(gameObject);
        }
    }
}
