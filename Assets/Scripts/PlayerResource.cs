using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.ParticleSystemJobs;

public class PlayerResource : MonoBehaviour
{
    [Header("Configuración de Luz (Tiempo)")]
    [SerializeField] private float maxLightTime = 20.0f;
    [SerializeField] private float sprintDrainMultiplier = 2.5f;
    [SerializeField] private float jumpCost = 1.5f;

    [Header("Efectos de Luz (Visual)")]
    [SerializeField] private Light playerLight;
    [SerializeField] private float minIntensity = 0.5f;
    [SerializeField] private float maxIntensity = 5.0f;

    [Header("Efectos (Fuego/Sonido)")]
    [Tooltip("Sistema de partículas de fuego del jugador")]
    [SerializeField] private ParticleSystem fireParticles;
    [SerializeField] private float minParticleSpeed = 0.2f;
    [SerializeField] private float maxParticleSpeed = 1.0f;

    [Tooltip("AudioSource del 'loop' de fuego del jugador")]
    [SerializeField] private AudioSource fireAudioSource;
    [SerializeField] private float minAudioVolume = 0.1f;
    [SerializeField] private float maxAudioVolume = 0.8f;

    [Header("Dependencias")]
    [SerializeField] private GameManager gameManager;

    [SerializeField]
    private float currentLightTime;

    public bool IsAlive { get; private set; }

    private ParticleSystem.MainModule fireParticlesMainModule;

    private void Start()
    {
        currentLightTime = maxLightTime;
        IsAlive = true;

        if (fireParticles != null)
        {
            fireParticlesMainModule = fireParticles.main;
        }

        if (fireAudioSource != null)
        {
            fireAudioSource.Play();
        }
        UpdateAllEffects();
    }

    public void HandleDrain(bool isSprinting)
    {
        if (!IsAlive) return;

        float drainMultiplier = isSprinting ? sprintDrainMultiplier : 1.0f;
        currentLightTime -= Time.deltaTime * drainMultiplier;

        if (currentLightTime <= 0)
        {
            currentLightTime = 0;
            IsAlive = false;
            UpdateAllEffects();

            if (gameManager != null)
                gameManager.HandleLose();
            else
                Debug.LogError("¡GameManager no está asignado en PlayerResource!");
        }
        else
        {
            UpdateAllEffects();
        }
    }

    public bool ConsumeForJump()
    {
        if (currentLightTime > jumpCost)
        {
            currentLightTime -= jumpCost;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Recharge()
    {
        Debug.Log("¡Luz Recargada!");
        currentLightTime = maxLightTime;
        UpdateAllEffects();
    }

    public void TakeDamage(float amount)
    {
        if (!IsAlive) return;

        Debug.Log($"El jugador recibió {amount} de daño de luz");
        currentLightTime -= amount;

        if (currentLightTime < 0)
        {
            currentLightTime = 0;
        }

        UpdateAllEffects();
    }

    public float GetLightPercentage()
    {
        return currentLightTime / maxLightTime;
    }

    private void UpdateAllEffects()
    {
        float lightPercentage = GetLightPercentage();

        if (playerLight != null)
        {
            playerLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, lightPercentage);
        }

        if (fireParticles != null)
        {
            fireParticlesMainModule.simulationSpeed = Mathf.Lerp(minParticleSpeed, maxParticleSpeed, lightPercentage);
        }

        if (fireAudioSource != null)
        {
            fireAudioSource.volume = Mathf.Lerp(minAudioVolume, maxAudioVolume, lightPercentage);
        }
    }
}