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
            }
        }
    }
    //ZonaDanina znDanina = collision.GetComponent<ZonaDanina>();
    //if (znDanina != null)
    //{
    //    siguienteDano = Time.time + znDanina.intervaloDano;

    //}

    //ZonaToxica znToxica = collision.GetComponent<ZonaToxica>();
    //if (znToxica != null)
    //{
    //    siguienteDano = Time.time + znToxica.intervaloDano;

    //}
    //}

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    ZonaDanina znDanina = collision.GetComponent<ZonaDanina>();
    //    if (znDanina != null)
    //    {
    //        if (Time.time >= siguienteDano)
    //        {
    //            vdJugador.recibirDano(znDanina.dano);
    //            siguienteDano = Time.time + znDanina.intervaloDano;
    //        }
    //    }

    //    ZonaToxica znToxica = collision.GetComponent<ZonaToxica>();
    //    if (znToxica != null)
    //    {
    //        if (Time.time >= siguienteDano)
    //        {
    //            vdJugador.recibirDano(znToxica.dano);
    //            siguienteDano = Time.time + znToxica.intervaloDano;
    //            znToxica.dano++;
    //        }
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    ZonaToxica znToxica = collision.GetComponent<ZonaToxica>();
    //    if (znToxica != null)
    //    {
    //        znToxica.dano = 1;

    //    }
    //}
}
