
using UnityEngine;

public class Cristal : MonoBehaviour
{
    public string idCristal;

    private void Start()
    {
        // Comprobar si este cristal ya fue recogido.
        if (PlayerPrefs.GetInt("Cristal_" + idCristal, 0) == 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        ContadorCristales contador =
            collision.GetComponent<ContadorCristales>();

        if (contador != null)
        {
            if (string.IsNullOrEmpty(idCristal))
            {
                Debug.LogError("Asigna un ID único a este cristal.");
                return;
            }

            string clave = "Cristal_" + idCristal;

            // Evitar sumar el mismo cristal más de una vez.
            if (PlayerPrefs.GetInt(clave, 0) == 1)
                return;

            PlayerPrefs.SetInt(clave, 1);
            PlayerPrefs.Save();

            contador.agregarCristal();
            Destroy(gameObject);
        }
    }
}
