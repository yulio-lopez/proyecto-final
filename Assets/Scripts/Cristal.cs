using UnityEngine;

public class Cristal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ContadorCristales contador =
                collision.GetComponent<ContadorCristales>();

            if (contador != null)
            {
                contador.agregarCristal();
                Destroy(gameObject);
            }
        }
    }
}
