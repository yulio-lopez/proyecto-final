
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelPrincipal;
    public GameObject panelOpciones;
    public GameObject panelCreditos;

    [Header("Botones")]
    public Button botonContinuar;

    [Header("Volumen")]
    public Slider sliderVolumen;

    [Header("Cristales")]
    public string[] idsCristales;

    private void Start()
    {
        Time.timeScale = 1f;

        // Cargar volumen guardado
        float volumen = PlayerPrefs.GetFloat("Volumen", 1f);
        AudioListener.volume = volumen;

        if (sliderVolumen != null)
            sliderVolumen.value = volumen;

        // Mostrar menú principal
        panelPrincipal.SetActive(true);
        panelOpciones.SetActive(false);
        panelCreditos.SetActive(false);

        // Comprobar si existe una partida guardada
        bool hayPartida = PlayerPrefs.GetInt("checkpointGuardado", 0) == 1;

        if (botonContinuar != null)
            botonContinuar.interactable = hayPartida;
    }

    public void NuevaPartida()
    {
        // Reiniciar contador de cristales
        PlayerPrefs.SetInt("TotalCristales", 0);

        // Hacer que los cristales vuelvan a aparecer
        foreach (string id in idsCristales)
        {
            PlayerPrefs.DeleteKey("Cristal_" + id);
        }

        // Borrar checkpoint anterior
        PlayerPrefs.DeleteKey("posX");
        PlayerPrefs.DeleteKey("posY");
        PlayerPrefs.DeleteKey("checkpointGuardado");
        PlayerPrefs.DeleteKey("escenaCheckpoint");

        PlayerPrefs.Save();

        Time.timeScale = 1f;
        SceneManager.LoadScene("NivelUno");
    }

    public void ContinuarPartida()
    {
        if (PlayerPrefs.GetInt("checkpointGuardado", 0) == 1)
        {
            string escenaGuardada = PlayerPrefs.GetString(
                "escenaCheckpoint", "NivelUno"
            );

            Time.timeScale = 1f;
            SceneManager.LoadScene(escenaGuardada);
        }
    }

    public void AbrirOpciones()
    {
        panelPrincipal.SetActive(false);
        panelOpciones.SetActive(true);
        panelCreditos.SetActive(false);
    }

    public void CerrarOpciones()
    {
        panelOpciones.SetActive(false);
        panelPrincipal.SetActive(true);
    }

    public void CambiarVolumen(float volumen)
    {
        AudioListener.volume = volumen;

        PlayerPrefs.SetFloat("Volumen", volumen);
        PlayerPrefs.Save();
    }

    public void AbrirCreditos()
    {
        panelPrincipal.SetActive(false);
        panelCreditos.SetActive(true);
    }

    public void CerrarCreditos()
    {
        panelCreditos.SetActive(false);
        panelPrincipal.SetActive(true);
    }

    public void SalirJuego()
    {
        Debug.Log("Cerrando juego...");
        Application.Quit();
    }
}
