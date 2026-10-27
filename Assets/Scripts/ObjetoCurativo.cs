using UnityEngine;

public class ObjetoCurativo : MonoBehaviour
{
    public int cura = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            VidaJugador vidaJugador = collision.GetComponent<VidaJugador>();
            vidaJugador.curarDano(cura);
            Destroy(this.gameObject);
        }
    }
}

