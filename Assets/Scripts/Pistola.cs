using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class Pistola : MonoBehaviour
{
    public GameObject proyectil;
    public PlayerInput PlayerInput;
    public MovimientoJugador mover;

    public float delayDisparo = 1f;
    public int balasTotales = 3;

    public Transform contenedorBalas;
    public Image balaPrefab;
    public Sprite balaLlena;
    public Sprite balaVacia;
    public Animator animator;
    private int balasActuales;
    private float contador;
    private InputAction dispararAccion;
    private InputAction recargarAccion;

    private List<Image> balasUI = new List<Image>();

    void Start()
    {
        dispararAccion = PlayerInput.actions.FindAction("Humano/disparar", true);
        recargarAccion = PlayerInput.actions.FindAction("Humano/recargar", true);

        contador = delayDisparo + 1f;
        balasActuales = balasTotales;

        crearBalasUI();
        actualizarBalasUI();
    }

    void Update()
    {
        contador += Time.deltaTime;

        if (dispararAccion.WasPerformedThisFrame())
        {
            if (contador > delayDisparo && balasActuales > 0)
            {
                this.disparar();
                contador = 0f;
                balasActuales--;

                actualizarBalasUI();
            }

        }
        if (recargarAccion.WasPerformedThisFrame())
        {
            balasActuales = balasTotales;

            actualizarBalasUI();
        }

    }

    void crearBalasUI()
    {
        foreach (Transform hijo in contenedorBalas)
        {
            Destroy(hijo.gameObject);
        }

        balasUI.Clear();

        for (int i = 0; i < balasTotales; i++)
        {
            Image bala = Instantiate(balaPrefab, contenedorBalas);
            balasUI.Add(bala);
        }
    }

    void actualizarBalasUI()
    {
        for (int i = 0; i < balasUI.Count; i++)
        {
            if (i < balasActuales)
            {
                balasUI[i].sprite = balaLlena;
                balasUI[i].color = Color.white;
            }
            else
            {
                balasUI[i].sprite = balaVacia;
                balasUI[i].color = Color.gray;
            }
        }
    }

    private void disparar()
    {
        GameObject nuevoProyectil = Instantiate(proyectil, this.transform.position, this.transform.rotation);
        Bala movProyectil = nuevoProyectil.GetComponent<Bala>();
        movProyectil.establecerDireccion(mover.direccionMirada);

        if (animator != null)
        {
            animator.SetTrigger("disparar");
        }
    }
}
