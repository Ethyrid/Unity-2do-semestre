using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Trap : MonoBehaviour
{
    [Header("Configuraci�n de la Trampa")]
    [Tooltip("Segundos de luz que esta trampa quita al jugador")]
    [SerializeField] private float lightDamage = 5.0f;

    [Header("Efectos de Feedback (Opcional)")]
    [Tooltip("Prefab de part�culas que se instancia al tocar la trampa (ej. un 'splash')")]
    [SerializeField] private GameObject activationParticles;

    [Tooltip("Sonido que se reproduce al tocar la trampa (ej. un 'sizzle' o 'splash')")]
    [SerializeField] private AudioClip activationSound;

    [Header("Configuraci�n de Re-activaci�n")]
    [Tooltip("Si se marca, la trampa se puede re-usar despu�s del 'cooldown'")]
    [SerializeField] private bool canReactivate = false;
    [SerializeField] private float reactivationCooldown = 3.0f;

    private Collider trapCollider;
    private bool isReady = true;

    private void Awake()
    {
        trapCollider = GetComponent<Collider>();
        trapCollider.isTrigger = true;
        isReady = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isReady || !other.CompareTag("Player"))
        {
            return;
        }

        PlayerResource player = other.GetComponent<PlayerResource>();

        if (player != null && player.IsAlive)
        {
            player.TakeDamage(lightDamage);

            if (activationSound)
            {
                AudioSource.PlayClipAtPoint(activationSound, transform.position);
            }

            if (activationParticles)
            {
                Instantiate(activationParticles, other.transform.position, Quaternion.identity);
            }

            if (canReactivate)
            {
                isReady = false;
                Invoke(nameof(Reactivate), reactivationCooldown);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void Reactivate()
    {
        Debug.Log("Trampa reactivada");
        isReady = true;
    }
}