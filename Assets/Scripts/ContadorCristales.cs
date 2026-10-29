using UnityEngine;
using TMPro;

public class ContadorCristales : MonoBehaviour
{
    public int cristales = 0;
    public TextMeshProUGUI textoCristales;

    void Start()
    {
        actualizarTexto();
    }

    public void agregarCristal()
    {
        cristales++;
        actualizarTexto();
    }

    void actualizarTexto()
    {
        textoCristales.text = cristales.ToString();
    }
}
