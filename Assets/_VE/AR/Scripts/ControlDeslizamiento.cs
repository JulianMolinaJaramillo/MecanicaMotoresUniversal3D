using System.Collections;
using UnityEngine;

public class ControlDeslizamiento : MonoBehaviour
{
    [Header("Referencia")]
    public SkinnedMeshRenderer skinnedMesh;

    [Header("Configuración")]
    public string nombreBlendShape = "Deslizamiento";
    public float velocidad = 30f; // Velocidad del cambio (unidades por segundo)

    private int indexBlendShape;
    private Coroutine corrutinaActiva;

    void Start()
    {
        if (skinnedMesh == null)
            skinnedMesh = GetComponent<SkinnedMeshRenderer>();

        // Buscar el índice del blendshape por nombre
        Mesh mesh = skinnedMesh.sharedMesh;
        indexBlendShape = mesh.GetBlendShapeIndex(nombreBlendShape);

        if (indexBlendShape < 0)
            Debug.LogError("BlendShape no encontrado: " + nombreBlendShape);
    }

    /// <summary>
    /// Llama a este método para animar el blendshape desde 0 hasta el valor objetivo.
    /// </summary>
    [ContextMenu("deslizar")]
    public void AnimarBlendShape()
    {
        if (indexBlendShape < 0) return;

        if (corrutinaActiva != null)
            StopCoroutine(corrutinaActiva);

        corrutinaActiva = StartCoroutine(CambiarBlendShape(100));
    }

    private IEnumerator CambiarBlendShape(float objetivo)
    {
        float valorActual = skinnedMesh.GetBlendShapeWeight(indexBlendShape);
        float direccion = Mathf.Sign(objetivo - valorActual); // +1 o -1

        while ((direccion > 0 && valorActual < objetivo) || (direccion < 0 && valorActual > objetivo))
        {
            valorActual += direccion * velocidad * Time.deltaTime;
            valorActual = Mathf.Clamp(valorActual, 0f, 100f); // los blendshapes van de 0 a 100
            skinnedMesh.SetBlendShapeWeight(indexBlendShape, valorActual);
            yield return null;
        }

        // Asegurar valor final exacto
        skinnedMesh.SetBlendShapeWeight(indexBlendShape, objetivo);
    }
}
