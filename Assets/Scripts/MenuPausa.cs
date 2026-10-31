
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MenuPausa : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelPausa;
    public GameObject panelOpciones;

    [Header("Volumen")]
    public UnityEngine.UI.Slider sliderVolumen;

    private bool juegoPausado = false;

    void Start()
    {
        Time.timeScale = 1f;

        panelPausa.SetActive(false);
        panelOpciones.SetActive(false);

        // Cargar volumen guardado
        float volumen = PlayerPrefs.GetFloat("Volumen", 1f);
        AudioListener.volume = volumen;

        if (sliderVolumen != null)
            sliderVolumen.value = volumen;
    }

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (juegoPausado)
                Reanudar();
            else
                Pausar();
        }
    }

    public void Pausar()
    {
        juegoPausado = true;
        panelPausa.SetActive(true);
        panelOpciones.SetActive(false);

        Time.timeScale = 0f;
        MusicaPrincipal.Pausar();
    }

    public void Reanudar()
    {
        juegoPausado = false;
        panelPausa.SetActive(false);

        Time.timeScale = 1f;
        MusicaPrincipal.Reanudar();
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        MusicaPrincipal.Reanudar();

        string escenaActual = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(escenaActual);
    }

    public void IrMenuPrincipal()
    {
        Time.timeScale = 1f;
        MusicaPrincipal.Reanudar();

        SceneManager.LoadScene("MenuPrincipal");
    }


    public void AbrirOpciones()
    {
        panelPausa.SetActive(false);
        panelOpciones.SetActive(true);
    }

    public void CerrarOpciones()
    {
        panelOpciones.SetActive(false);
        panelPausa.SetActive(true);
    }

    public void CambiarVolumen(float volumen)
    {
        AudioListener.volume = volumen;

        PlayerPrefs.SetFloat("Volumen", volumen);
        PlayerPrefs.Save();
    }
}
