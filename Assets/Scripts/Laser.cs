using System.Collections;
using UnityEngine;
public class Laser : MonoBehaviour
{
    [Header("Tiempo del laser")]
    public float tiempoEncendido = 3f;
    public float tiempoApagado = 2f;

    [Header("Daño")]
    public int dano = 1;
    public float tiempoEntreDanos = 1f;

    private SpriteRenderer sprite;
    private Collider2D colision;

    private bool activo = true;
    private float siguienteDano = 0f;
    private bool jugadorDanado = false;

    [Header("Sonido")]
    public AudioSource audioSource;
    public AudioClip sonidoEncendido;

    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        colision = GetComponent<Collider2D>();

        StartCoroutine(CicloLaser());
    }

    IEnumerator CicloLaser()
    {
        while (true)
        {
            // Encender laser
            activo = true;
            sprite.enabled = true;
            colision.enabled = true;

            // Reproducir sonido al encender
            if (audioSource != null && sonidoEncendido != null)
            {
                audioSource.clip = sonidoEncendido;
                audioSource.loop = true;
                audioSource.Play();
            }

            yield return new WaitForSeconds(tiempoEncendido);

            // Apagar laser
            activo = false;
            sprite.enabled = false;
            colision.enabled = false;

            // Detener sonido al apagar
            if (audioSource != null)
            {
                audioSource.Stop();
            }

            // Reiniciar el daño
            jugadorDanado = false;

            yield return new WaitForSeconds(tiempoApagado);
        }
    }


    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (!activo) return;

        if (otro.CompareTag("Player") && !jugadorDanado)
        {
            VidaJugador vida = otro.GetComponent<VidaJugador>();

            if (vida != null)
            {
                vida.recibirDano(dano);
                jugadorDanado = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            jugadorDanado = false;
        }
    }

}
