using UnityEngine;
using UnityEngine.SceneManagement;

public class SalidaNivel : MonoBehaviour
{
    public string escenaVictoria = "Victoria";

    private bool nivelCompletado = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (nivelCompletado)
            return;

        if (collision.CompareTag("Player"))
        {
            nivelCompletado = true;

            SceneManager.LoadScene(escenaVictoria);
        }
    }
}
