using System.Collections;
using UnityEngine;

public class PlataformaDesaparece : MonoBehaviour
{
    public float tiempoEspera = 1.0f;
    public float tiempoDesaparecida = 3f;

    private Collider2D colision;
    private SpriteRenderer sprite;

    private bool activada = false;

    void Start()
    {
        colision = GetComponent<Collider2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !activada)
        {
            StartCoroutine(Desaparecer());
        }
    }

    IEnumerator Desaparecer()
    {
        activada = true;

        yield return new WaitForSeconds(tiempoEspera);

        colision.enabled = false;
        sprite.enabled = false;

        yield return new WaitForSeconds(tiempoDesaparecida);

        sprite.enabled = true;
        colision.enabled = true;

        activada = false;
    }
}
