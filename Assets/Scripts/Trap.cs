using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Trap : MonoBehaviour
{
    [Header("Configuración de la Trampa")]
    [Tooltip("Segundos de luz que quita por segundo")]
    [SerializeField] private float lightDamagePerSecond = 2.0f;

    [Header("Efectos de Feedback")]
    [Tooltip("ParticleSystem")]
    [SerializeField] private ParticleSystem activationParticles;

    [Tooltip("El script controla Play/Stop/Emit de las partículas al dañar? (Desmarcar para zonas con partículas siempre activas)")]
    [SerializeField] private bool controlParticlesOnDamage = true;
    [Tooltip("Cuántas partículas emitir cada vez que se aplica daño (si controlParticlesOnDamage está en 'Emit') - No usado en Play/Stop")]
    [SerializeField] private int particlesPerHit = 15;
    [Tooltip("Sonido que se reproduce CADA VEZ que se aplica daño")]
    [SerializeField] private AudioClip damageTickSound;

    [Header("Configuración de Daño Continuo")]
    [Tooltip("Tiempo en segundos entre cada tick de daño mientras se está dentro")]
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
                Debug.LogWarning("Trap: ParticleSystem tiene 'Play On Awake'. Se desactivará porque controlParticlesOnDamage=true.", this);
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