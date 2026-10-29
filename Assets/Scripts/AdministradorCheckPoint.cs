
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdministradorCheckPoint : MonoBehaviour
{
    public static AdministradorCheckPoint instancia;

    private const string clavePosX = "posX";
    private const string clavePosY = "posY";
    private const string claveCheckpoint = "checkpointGuardado";
    private const string claveEscena = "escenaCheckpoint";

    private void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
    }

    public void GuardarCheckpoint(Vector3 posicion)
    {
        PlayerPrefs.SetFloat(clavePosX, posicion.x);
        PlayerPrefs.SetFloat(clavePosY, posicion.y);
        PlayerPrefs.SetInt(claveCheckpoint, 1);

        PlayerPrefs.SetString(
            claveEscena,
            SceneManager.GetActiveScene().name
        );

        PlayerPrefs.Save();

        Debug.Log("Checkpoint guardado: " + posicion);
    }

    public Vector3 ObtenerCheckpoint()
    {
        if (ExisteCheckpoint())
        {
            return new Vector3(
                PlayerPrefs.GetFloat(clavePosX),
                PlayerPrefs.GetFloat(clavePosY),
                0f
            );
        }

        return Vector3.zero;
    }

    public bool ExisteCheckpoint()
    {
        return PlayerPrefs.GetInt(claveCheckpoint, 0) == 1;
    }

    public void BorrarCheckpoint()
    {
        PlayerPrefs.DeleteKey(clavePosX);
        PlayerPrefs.DeleteKey(clavePosY);
        PlayerPrefs.DeleteKey(claveCheckpoint);
        PlayerPrefs.DeleteKey(claveEscena);

        PlayerPrefs.Save();

        Debug.Log("Checkpoint borrado.");
    }

    public void RecargarEscenaCheckpoint()
    {
        if (!ExisteCheckpoint())
        {
            Debug.LogWarning("No hay ningún checkpoint guardado.");
            return;
        }

        string escena = PlayerPrefs.GetString(claveEscena, "");

        if (string.IsNullOrEmpty(escena))
        {
            Debug.LogError("No se encontró la escena del checkpoint.");
            return;
        }

        SceneManager.LoadScene(escena);
    }
}
