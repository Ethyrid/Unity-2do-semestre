using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TorchCheckpoint : MonoBehaviour
{
    [Header("Efectos (Opcional)")]
    [SerializeField] private Light torchLight;
    [SerializeField] private GameObject activationParticles;
    [SerializeField] private AudioClip activationSound;

    private bool hasBeenUsed = false;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;

        if (torchLight) torchLight.enabled = false;

        if (activationParticles)
            activationParticles.SetActive(false);
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

            if (torchLight)
                torchLight.enabled = true;

            if (activationParticles)
                activationParticles.SetActive(true);

            if (activationSound)
            {
                AudioSource.PlayClipAtPoint(activationSound, transform.position);
            }
            Destroy(gameObject);
        }
    }
}