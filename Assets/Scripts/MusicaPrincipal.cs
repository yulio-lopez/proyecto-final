using UnityEngine;

public class MusicaPrincipal : MonoBehaviour
{
    [SerializeField] private AudioClip pista;

    private static AudioSource fuenteMusica;

    private void Awake()
    {
        if (fuenteMusica != null)
        {
            Reanudar();
            return;
        }

        if (pista == null)
        {
            Debug.LogError("No se asignó la pista principal.", this);
            return;
        }

        GameObject reproductor = new GameObject("MusicaPrincipal");
        DontDestroyOnLoad(reproductor);

        fuenteMusica = reproductor.AddComponent<AudioSource>();
        fuenteMusica.clip = pista;
        fuenteMusica.loop = true;
        fuenteMusica.playOnAwake = false;
        fuenteMusica.spatialBlend = 0f;
        fuenteMusica.Play();
    }

    public static void Pausar()
    {
        if (fuenteMusica != null && fuenteMusica.isPlaying)
            fuenteMusica.Pause();
    }

    public static void Reanudar()
    {
        if (fuenteMusica == null)
            return;

        fuenteMusica.UnPause();
        if (!fuenteMusica.isPlaying)
            fuenteMusica.Play();
    }
}
