using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TorchCheckpoint : MonoBehaviour
{
    [Header("Componentes a Controlar")]
    [Tooltip("GameObject del cristal")]
    [SerializeField] private GameObject crystalVisualObject;
    [Tooltip("Luz encendida (Point Light)")]
    [SerializeField] private Light persistentLight;
    [Tooltip("Partículas que se activan")]
    [SerializeField] private ParticleSystem activationParticles;
    [Tooltip("Sonido que se reproduce al activar")]
    [SerializeField] private AudioClip activationSound;

    private bool hasBeenUsed = false;
    private Collider myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
        myCollider.isTrigger = true;

        if (persistentLight)
            persistentLight.enabled = true;

        if (crystalVisualObject)
            crystalVisualObject.SetActive(true);

        if (activationParticles)
            activationParticles.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenUsed || !other.CompareTag("Player"))
        {
            return;
        }

        PlayerResource player = other.GetComponent<PlayerResource>();

        if (player != null && player.IsAlive)
        {
            hasBeenUsed = true;
            player.Recharge();

            if (activationParticles)
                activationParticles.Play();

            if (activationSound)
            {
                AudioSource.PlayClipAtPoint(activationSound, transform.position);
            }

            if (crystalVisualObject)
                Destroy(crystalVisualObject);

            myCollider.enabled = false;
        }
    }
}