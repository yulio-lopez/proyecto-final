using UnityEngine;

public class VidaEnemigo : MonoBehaviour
{
    public int vidaMaxima = 3;
    public int dano = 1;
    public int vidaActual;

    void Start()
    {
        vidaActual = vidaMaxima;
    }

    public void recibirDano(int dano)
    {
        vidaActual -= dano;
        if (vidaActual <= 0)
        {
            morir();
        }
    }

    private void morir()
    {
        this.gameObject.SetActive(false);
    }
}
