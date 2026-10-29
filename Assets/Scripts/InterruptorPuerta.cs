
using UnityEngine;
using UnityEngine.InputSystem;

public class InterruptorPuerta : MonoBehaviour
{
    public Puerta puerta;

    private bool activado;
    private bool jugadorCerca;

    private void Update()
    {
        if (activado || !jugadorCerca)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            activado = true;
            puerta.Abrir();

            Debug.Log("¡Puerta abierta!");
        }
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }
}
