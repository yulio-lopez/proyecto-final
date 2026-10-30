using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuVictoria : MonoBehaviour
{
    public TextMeshProUGUI textoCristales;

    void Start()
    {
        Time.timeScale = 1f;

        int cristales = PlayerPrefs.GetInt("TotalCristales", 0);

        textoCristales.text = "Cristales recuperados: " + cristales;
    }

    public void VolverAJugar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("NivelUno");
    }

    public void IrMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }
}
