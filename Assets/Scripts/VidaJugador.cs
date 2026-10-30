
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VidaJugador : MonoBehaviour
{
    public int vidaMaxima = 5;
    public int vidaActual;

    public Transform contenedorCorazones;
    public Image corazonPrefab;
    public Sprite corazonLleno;
    public Sprite corazonVacio;

    private bool estaMuerto = false;
    private Animator animator;
    private List<Image> corazones = new List<Image>();

    private MovimientoJugador movimiento;
    private Rigidbody2D rb;
    private Collider2D colliderJugador;

    [Header("Retroceso al recibir daño")]
    public float fuerzaRetrocesoX = 5f;
    public float fuerzaRetrocesoY = 2f;
    public float tiempoRetroceso = 0.15f;

    [Header("Invulnerabilidad")]
    public float tiempoInvulnerable = 1f;

    private bool esInvulnerable = false;

    private bool recibiendoRetroceso = false;

    void Start()
    {
        vidaActual = vidaMaxima;

        crearCorazones();
        actualizarCorazones();

        animator = GetComponentInChildren<Animator>();
        movimiento = GetComponent<MovimientoJugador>();
        rb = GetComponent<Rigidbody2D>();
        colliderJugador = GetComponent<Collider2D>();
    }

    private void LateUpdate()
    {
        if (estaMuerto)
            AlinearSpriteConSuelo(colliderJugador);
    }

    void crearCorazones()
    {
        foreach (Transform hijo in contenedorCorazones)
        {
            Destroy(hijo.gameObject);
        }

        corazones.Clear();

        for (int i = 0; i < vidaMaxima; i++)
        {
            Image corazon = Instantiate(
                corazonPrefab,
                contenedorCorazones
            );

            corazones.Add(corazon);
        }
    }

    void actualizarCorazones()
    {
        for (int i = 0; i < corazones.Count; i++)
        {
            if (i < vidaActual)
                corazones[i].sprite = corazonLleno;
            else
                corazones[i].sprite = corazonVacio;
        }
    }

    public void morir()
    {
        if (estaMuerto)
            return;

        estaMuerto = true;

        Debug.Log("Muere jugador");

        if (movimiento != null)
            movimiento.enabled = false;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (animator != null)
            animator.SetTrigger("morir");

        StartCoroutine(reaparecer());
    }


    IEnumerator reaparecer()
    {
        // Esperar a que termine la animación de muerte.
        if (animator != null)
        {
            yield return null;

            while (!animator.GetCurrentAnimatorStateInfo(0)
                .IsName("mapache_death"))
            {
                yield return null;
            }

            while (animator.GetCurrentAnimatorStateInfo(0)
                .IsName("mapache_death") &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
        }

        if (colliderJugador != null)
        {
            float tiempoInicioEsperaSuelo = Time.time;
            while (!colliderJugador.IsTouchingLayers() &&
                Time.time - tiempoInicioEsperaSuelo < 3f)
            {
                yield return null;
            }
        }

        AlinearSpriteConSuelo(colliderJugador);

        // Si existe un checkpoint, recargar su escena.
        if (AdministradorCheckPoint.instancia != null &&
            AdministradorCheckPoint.instancia.ExisteCheckpoint())
        {
            AdministradorCheckPoint.instancia.RecargarEscenaCheckpoint();
        }
        else
        {
            // Si no hay checkpoint, reiniciar la escena actual.
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager
                    .GetActiveScene().buildIndex
            );
        }
    }

    private void AlinearSpriteConSuelo(Collider2D collider)
    {
        if (collider == null || animator == null)
            return;

        SpriteRenderer spriteRenderer =
            animator.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            return;

        Vector3 posicion = animator.transform.position;
        posicion.y += collider.bounds.min.y - spriteRenderer.bounds.min.y;
        animator.transform.position = posicion;
    }

    public void AplicarRetroceso(Vector2 posicionAtacante)
    {
        if (estaMuerto || rb == null)
            return;

        // Dirección contraria al atacante.
        float direccion = transform.position.x < posicionAtacante.x
            ? -1f
            : 1f;

        // Aplicar impulso horizontal y vertical.
        rb.linearVelocity = new Vector2(
            direccion * fuerzaRetrocesoX,
            fuerzaRetrocesoY
        );

        StopCoroutine(nameof(FinalizarRetroceso));
        StartCoroutine(FinalizarRetroceso());
    }

    private IEnumerator FinalizarRetroceso()
    {
        recibiendoRetroceso = true;

        yield return new WaitForSeconds(tiempoRetroceso);

        recibiendoRetroceso = false;
    }

    public void recibirDano(int dano)
    {
        if (estaMuerto || esInvulnerable)
            return;

        vidaActual = Mathf.Max(0, vidaActual - dano);

        actualizarCorazones();

        if (vidaActual <= 0)
        {
            morir();
        }
        else
        {
            StartCoroutine(Invulnerabilidad());
        }
    }

    private IEnumerator Invulnerabilidad()
    {
        esInvulnerable = true;

        yield return new WaitForSeconds(tiempoInvulnerable);

        esInvulnerable = false;
    }

    public void curarDano(int cura)
    {
        if (estaMuerto)
            return;

        if (vidaActual < vidaMaxima)
        {
            vidaActual = Mathf.Min(vidaMaxima, vidaActual + cura);
            actualizarCorazones();
        }
    }

    public void aumentarVidaMaxima(int cantidad)
    {
        vidaMaxima += cantidad;
        vidaActual += cantidad;

        crearCorazones();
        actualizarCorazones();
    }
}
