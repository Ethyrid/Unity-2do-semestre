using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Trap : MonoBehaviour
{
    [Header("Configuraci�n de la Trampa")]
    [Tooltip("Segundos de luz que quita por segundo")]
    [SerializeField] private float lightDamagePerSecond = 2.0f;

    [Header("Efectos de Feedback")]
    [Tooltip("ParticleSystem")]
    [SerializeField] private ParticleSystem activationParticles;

    [Tooltip("El script controla Play/Stop/Emit de las part�culas al da�ar? (Desmarcar para zonas con part�culas siempre activas)")]
    [SerializeField] private bool controlParticlesOnDamage = true;
    [Tooltip("Cu�ntas part�culas emitir cada vez que se aplica da�o (si controlParticlesOnDamage est� en 'Emit') - No usado en Play/Stop")]
    [SerializeField] private int particlesPerHit = 15;
    [Tooltip("Sonido que se reproduce CADA VEZ que se aplica da�o")]
    [SerializeField] private AudioClip damageTickSound;

    [Header("Configuraci�n de Da�o Continuo")]
    [Tooltip("Tiempo en segundos entre cada tick de da�o mientras se est� dentro")]
    [SerializeField] private float damageTickRate = 1.0f;

    private Collider trapCollider;
    private float lastDamageTime = -Mathf.Infinity;
    private PlayerResource currentPlayerResource = null;

    private void Awake()
    {
        trapCollider = GetComponent<Collider>();
        trapCollider.isTrigger = true;

        if (activationParticles != null && controlParticlesOnDamage)
        {
            activationParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = activationParticles.main;
            if (main.playOnAwake)
            {
                Debug.LogWarning("Trap: ParticleSystem tiene 'Play On Awake'. Se desactivar� porque controlParticlesOnDamage=true.", this);
                main.playOnAwake = false;
            }
        }
        else if (activationParticles == null)
        {
            Debug.LogWarning("Trap: No se ha asignado un ParticleSystem en activationParticles.", this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentPlayerResource = other.GetComponent<PlayerResource>();

            if (activationParticles != null && controlParticlesOnDamage)
            {
                activationParticles.Play();
            }
            lastDamageTime = Time.time - damageTickRate;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (currentPlayerResource != null && currentPlayerResource.IsAlive)
        {
            if (Time.time >= lastDamageTime + damageTickRate)
            {
                ApplyDamageAndSound();
                lastDamageTime = Time.time;
            }
        }

        else if (currentPlayerResource != null && !currentPlayerResource.IsAlive)
        {
            StopEffects();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopEffects();
            currentPlayerResource = null;
            lastDamageTime = -Mathf.Infinity;
        }
    }
    private void ApplyDamageAndSound()
    {
        if (currentPlayerResource == null || !currentPlayerResource.IsAlive) return;

        float damageAmount = lightDamagePerSecond * damageTickRate;
        currentPlayerResource.TakeDamage(damageAmount);

        if (damageTickSound)
        {
            AudioSource.PlayClipAtPoint(damageTickSound, transform.position);
        }
    }
    private void StopEffects()
    {
        if (activationParticles != null && controlParticlesOnDamage)
        {
            activationParticles.Stop();
        }
    }
}