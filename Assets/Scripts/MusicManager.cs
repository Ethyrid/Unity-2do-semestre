using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Dependencias")]
    [Tooltip("Prefab del player")]
    [SerializeField] private PlayerResource playerResource;

    [Header("Fuentes de Audio")]
    [Tooltip("M�sica ambiental")]
    [SerializeField] private AudioSource ambientMusicSource;
    [Tooltip("M�sica de peligro")]
    [SerializeField] private AudioSource dangerMusicSource;

    [Header("Configuraci�n")]
    [Tooltip("Segundos de luz restantes para activar la m�sica de peligro")]
    [SerializeField] private float dangerThreshold = 10.0f;

    private bool isDangerMusicPlaying = false;

    private void Start()
    {
        if (playerResource == null || ambientMusicSource == null || dangerMusicSource == null)
        {
            Debug.LogError("MusicManager: �Faltan referencias!", this);
            enabled = false;
            return;
        }

        ambientMusicSource.loop = true;
        ambientMusicSource.Play();
        ambientMusicSource.mute = false;

        dangerMusicSource.loop = true;
        dangerMusicSource.Stop();

        isDangerMusicPlaying = false;
    }

    private void Update()
    {
        float currentTime = playerResource.GetCurrentLightTime();

        if (currentTime <= dangerThreshold && !isDangerMusicPlaying)
        {
            isDangerMusicPlaying = true;
            ambientMusicSource.mute = true;
            dangerMusicSource.Play();
            Debug.Log("M�sica de Peligro: PLAY");
        }
        else if (currentTime > dangerThreshold && isDangerMusicPlaying)
        {
            isDangerMusicPlaying = false;
            ambientMusicSource.mute = false;
            dangerMusicSource.Stop();
            Debug.Log("M�sica de Peligro: STOP");
        }
    }
}