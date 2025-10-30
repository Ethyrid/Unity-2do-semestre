using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.ParticleSystemJobs;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class ReadOnlyInspectorAttribute : PropertyAttribute { }
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyInspectorAttribute))]
public class ReadOnlyInspectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endif

public class PlayerResource : MonoBehaviour
{
    [Header("Configuración de Luz (Tiempo)")]
    [SerializeField] private float maxLightTime = 20.0f;
    [SerializeField] private float sprintDrainMultiplier = 2.5f;
    [SerializeField] private float jumpCost = 1.5f;

    [Tooltip("Segundos extra que el jugador sobrevive con luz 0")]
    [SerializeField] private float gracePeriodDuration = 1.5f;

    [Header("Efectos de Luz (Visual)")]
    [SerializeField] private Light playerLight;
    [SerializeField] private float minIntensity = 0.5f;
    [SerializeField] private float maxIntensity = 5.0f;

    [Header("Efectos (Fuego/Sonido)")]
    [SerializeField] private ParticleSystem fireParticles;
    [SerializeField] private float minParticleSpeed = 0.2f;
    [SerializeField] private float maxParticleSpeed = 1.0f;
    [SerializeField] private AudioSource fireAudioSource;
    [SerializeField] private float minAudioVolume = 0.1f;
    [SerializeField] private float maxAudioVolume = 0.8f;

    [Header("Dependencias")]
    [SerializeField] private GameManager gameManager;

    [Header("Estado Actual (Debugging)")]
    [SerializeField] private float currentLightTime;
    [SerializeField, ReadOnlyInspector] private float currentIntensity;
    [SerializeField, ReadOnlyInspector] private float currentParticleSpeed;
    [SerializeField, ReadOnlyInspector] private float currentAudioVolume;

    [Tooltip("Tiempo restante del período de gracia")]
    [SerializeField, ReadOnlyInspector] private float graceTimer = -1f;

    public bool IsAlive { get; private set; }

    private ParticleSystem.MainModule fireParticlesMainModule;

    private void Start()
    {
        currentLightTime = maxLightTime;
        IsAlive = true;
        graceTimer = -1f;

        if (fireParticles != null) fireParticlesMainModule = fireParticles.main;
        if (fireAudioSource != null) fireAudioSource.Play();

        UpdateAllEffects();
    }
    public void HandleDrain(bool isSprinting)
    {
        if (!IsAlive) return;

        if (graceTimer > 0)
        {
            graceTimer -= Time.deltaTime;
            if (graceTimer <= 0)
            {
                Die();
            }
            UpdateAllEffects();
            return;
        }

        float drainMultiplier = isSprinting ? sprintDrainMultiplier : 1.0f;
        currentLightTime -= Time.deltaTime * drainMultiplier;

        if (currentLightTime <= 0 && graceTimer < 0)
        {
            currentLightTime = 0;
            graceTimer = gracePeriodDuration;
            Debug.Log("Iniciando período de gracia...");
            UpdateAllEffects();
        }
        else if (currentLightTime > 0)
        {
            UpdateAllEffects();
        }
    }

    public bool ConsumeForJump()
    {
        if (graceTimer > 0) return false;

        if (currentLightTime > jumpCost)
        {
            currentLightTime -= jumpCost;
            return true;
        }
        return false;
    }

    public void Recharge()
    {
        IsAlive = true;
        graceTimer = -1f;
        currentLightTime = maxLightTime;
        UpdateAllEffects();
        Debug.Log("¡Luz Recargada! Período de gracia cancelado.");
    }

    public void TakeDamage(float amount)
    {
        if (!IsAlive) return;

        currentLightTime -= amount;

        if (currentLightTime <= 0 && graceTimer < 0)
        {
            currentLightTime = 0;
            graceTimer = gracePeriodDuration;
            Debug.Log("Daño inició período de gracia...");
        }
        else if (currentLightTime < 0)
        {
            currentLightTime = 0;
        }

        UpdateAllEffects();
    }

    public float GetLightPercentage()
    {
        return maxLightTime > 0 ? currentLightTime / maxLightTime : 0f;
    }

    public float GetCurrentLightTime()
    {
        return currentLightTime;
    }

    private void Die()
    {
        Debug.Log("Período de gracia terminado. Muerte.");
        IsAlive = false;
        currentLightTime = 0;
        UpdateAllEffects();

        if (gameManager != null)
            gameManager.HandleLose();
        else
            Debug.LogError("¡GameManager no está asignado en PlayerResource!");
    }


    private void UpdateAllEffects()
    {
        float linearPercentage = GetLightPercentage();
        float acceleratedPercentage = 1.0f - (1.0f - linearPercentage) * (1.0f - linearPercentage);

        currentIntensity = Mathf.Lerp(minIntensity, maxIntensity, acceleratedPercentage);
        currentParticleSpeed = Mathf.Lerp(minParticleSpeed, maxParticleSpeed, acceleratedPercentage);
        currentAudioVolume = Mathf.Lerp(minAudioVolume, maxAudioVolume, acceleratedPercentage);

        if (playerLight != null)
        {
            playerLight.intensity = currentIntensity;
        }
        if (fireParticles != null && fireParticles.main.startSpeed.constantMax > 0)
        {
            var main = fireParticles.main;
            main.simulationSpeed = currentParticleSpeed;
        }
        if (fireAudioSource != null)
        {
            fireAudioSource.volume = currentAudioVolume;
        }
    }
}