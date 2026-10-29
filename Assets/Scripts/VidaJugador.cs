using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    void Start()
    {
        vidaActual = vidaMaxima;
        crearCorazones();
        actualizarCorazones();
        animator = GetComponentInChildren<Animator>();
    }

    void crearCorazones()
    {
        foreach (Transform hijo in contenedorCorazones)
        {
            Destroy(hijo.gameObject);
        }
        corazones.Clear();

        // Crear un corazon por cada punto de vida maxima
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
        Debug.Log("Muere jugador");

        if (animator != null)
        {
            animator.SetTrigger("morir");
            estaMuerto = true;
        }
        //this.gameObject.SetActive(false);

    }

    public void recibirDano(int dano)
    {
        if (estaMuerto)
            return;
        Debug.Log("Recibe daño jugador");
        vidaActual = Mathf.Max(0, vidaActual - dano);

        actualizarCorazones();

        if (vidaActual <= 0)
        {
            this.morir();
        }
    }

    public void curarDano(int cura)
    {
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
