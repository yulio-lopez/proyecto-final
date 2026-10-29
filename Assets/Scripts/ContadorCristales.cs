
using UnityEngine;
using TMPro;

public class ContadorCristales : MonoBehaviour
{
    public int cristales = 0;
    public TextMeshProUGUI textoCristales;

    private const string claveCristales = "TotalCristales";

    void Start()
    {
        // Recuperar el total guardado.
        cristales = PlayerPrefs.GetInt(claveCristales, 0);

        actualizarTexto();
    }

    public void agregarCristal()
    {
        cristales++;

        // Guardar el nuevo total.
        PlayerPrefs.SetInt(claveCristales, cristales);
        PlayerPrefs.Save();

        actualizarTexto();
    }

    void actualizarTexto()
    {
        if (textoCristales != null)
        {
            textoCristales.text = cristales.ToString();
        }
    }
}
