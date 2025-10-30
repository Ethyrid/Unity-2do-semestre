using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class BreakingFloor : MonoBehaviour
{
    [Header("Efectos")]
    [Tooltip("Sonido que se reproduce cuando el suelo EMPIEZA a romperse")]
    [SerializeField] private AudioClip breakSound;

    [Header("Comportamiento")]
    [Tooltip("Tiempo en segundos DESPUÉS del sonido antes de que el suelo desaparezca")]
    [SerializeField] private float delayBeforeBreak = 0.5f;

    private bool isBreaking = false;
    private Collider myCollider;
    private Renderer myRenderer;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();

        myRenderer = GetComponent<Renderer>();
        if (myRenderer == null)
        {
            myRenderer = GetComponentInChildren<Renderer>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isBreaking && collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(BreakSequence());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isBreaking && other.CompareTag("Player"))
        {
            StartCoroutine(BreakSequence());
        }
    }


    private IEnumerator BreakSequence()
    {
        isBreaking = true;

        if (breakSound)
        {
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
        }

        if (delayBeforeBreak > 0)
        {
            yield return new WaitForSeconds(delayBeforeBreak);
        }

        if (myRenderer)
        {
            myRenderer.enabled = false;
        }

        myCollider.enabled = false;

        Destroy(gameObject, 0.1f);
    }
}