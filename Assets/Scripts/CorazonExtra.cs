using Unity.VisualScripting;
using UnityEngine;

public class CorazonExtra : MonoBehaviour
{
    public int corazon = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            VidaJugador vidaJugador = collision.GetComponent<VidaJugador>();
            vidaJugador.aumentarVidaMaxima(corazon);
            Destroy(this.gameObject);
        }
    }
}
