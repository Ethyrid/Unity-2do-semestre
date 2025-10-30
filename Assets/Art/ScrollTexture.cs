using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    [Tooltip("Velocidad de desplazamiento horizontal")]
    public float scrollX = 0.05f;
    [Tooltip("Velocidad de desplazamiento vertical")]
    public float scrollY = 0.05f;

    [Tooltip("Animar textura base?")]
    public bool animateBaseMap = false;
    [Tooltip("Multiplicador de velocidad para la textura base")]
    public float baseMapSpeedMultiplier = 0.5f;

    private Renderer rend;
    private MaterialPropertyBlock propBlock;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogError("ScrollTexture: ¡No se encontró un componente Renderer!", this);
            enabled = false;
            return;
        }
        propBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        float offsetX = Time.time * scrollX;
        float offsetY = Time.time * scrollY;

        rend.GetPropertyBlock(propBlock);
        propBlock.SetVector("_BumpMap_ST", new Vector4(1, 1, offsetX, offsetY));

        if (animateBaseMap)
        {
            float baseOffsetX = offsetX * baseMapSpeedMultiplier;
            float baseOffsetY = offsetY * baseMapSpeedMultiplier;
            propBlock.SetVector("_BaseMap_ST", new Vector4(1, 1, baseOffsetX, baseOffsetY));
        }
        rend.SetPropertyBlock(propBlock);
    }
}