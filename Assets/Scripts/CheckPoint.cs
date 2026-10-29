
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Transform puntoReaparicion;

    private bool activado;

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (activado || !otro.CompareTag("Player"))
            return;

        AdministradorCheckPoint administrador =
            AdministradorCheckPoint.instancia;

        if (administrador == null)
        {
            Debug.LogError(
                "No existe AdministradorCheckPoint en la escena."
            );
            return;
        }

        Vector3 posicion = puntoReaparicion != null
            ? puntoReaparicion.position
            : transform.position;

        administrador.GuardarCheckpoint(posicion);

        activado = true;

        Debug.Log("¡Checkpoint activado!");
    }
}
