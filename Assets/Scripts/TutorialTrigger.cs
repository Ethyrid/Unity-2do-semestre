using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class TutorialTrigger : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("TextMeshPro")]
    [SerializeField] private GameObject tutorialTextObject;

    [Tooltip("El trigger debe destruirse después de usarse una vez?")]
    [SerializeField] private bool destroyAfterUse = true;

    private Collider myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
        myCollider.isTrigger = true;

        if (tutorialTextObject != null)
        {
            tutorialTextObject.SetActive(true);
        }

        else
        {
            Debug.LogError("TutorialTrigger: ¡No se ha asignado un objeto de texto!", this);
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {

            if (tutorialTextObject != null)
            {
                tutorialTextObject.SetActive(false);
            }

            if (destroyAfterUse)
            {
                myCollider.enabled = false;
                Destroy(gameObject);
            }
        }
    }
}