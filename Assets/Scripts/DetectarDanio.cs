using UnityEngine;

public class DetectarDanio : MonoBehaviour
{
    public float siguienteDano = 0f;


    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Si una bala golpea a un enemigo
        if (CompareTag("Enemigo") && collider.CompareTag("Bala"))
        {
            VidaEnemigo vidaEnemigo = GetComponent<VidaEnemigo>();
            Bala bala = collider.GetComponent<Bala>();

            if (vidaEnemigo != null)
            {
                vidaEnemigo.recibirDano(bala.dano);
            }
        }

        if (CompareTag("Player") && collider.CompareTag("Enemigo"))
        {
            VidaJugador vidaJugador = GetComponent<VidaJugador>();
            VidaEnemigo enemigo = collider.GetComponent<VidaEnemigo>();

            if (vidaJugador != null)
            {
                vidaJugador.recibirDano(enemigo.dano);

                vidaJugador.AplicarRetroceso(collider.transform.position);
            }
        }

        ObjetoDanio objDanio = collider.GetComponent<ObjetoDanio>();
        if (objDanio != null && CompareTag("Player"))
        {
            VidaJugador vidaJUgador = GetComponent<VidaJugador>();
            vidaJUgador.recibirDano(objDanio.dano);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (CompareTag("Player") && collision.gameObject.gameObject.CompareTag("Enemigo"))
        {
            VidaJugador vidaJugador = GetComponent<VidaJugador>();
            VidaEnemigo enemigo = collision.gameObject.GetComponent<VidaEnemigo>();

            if (vidaJugador != null)
            {
                vidaJugador.recibirDano(enemigo.dano);
                vidaJugador.AplicarRetroceso(collision.transform.position);
            }
        }
    }
}
