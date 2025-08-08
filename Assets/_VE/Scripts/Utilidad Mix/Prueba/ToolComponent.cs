using UnityEngine;
using System.Collections;

public enum ToolType
{
    Rachet,
    Socket,
    Wrench,
    Screwdriver
}

public class ToolComponent : MonoBehaviour
{
    [Header("Tipo de esta herramienta")]
    public ToolType toolType;

    [Header("Tipo de herramienta con la que es compatible")]
    public ToolType compatibleWith;

    [Header("Punto de unión (solo si puede recibir otras herramientas)")]
    public Transform attachPoint;

    [Header("Componente actualmente acoplado")]
    public ToolComponent attachedComponent;

    /// <summary>
    /// Verifica si este objeto es compatible con otro
    /// </summary>
    public bool IsCompatibleWith(ToolComponent other)
    {
        return other.toolType == compatibleWith;
    }

    /// <summary>
    /// Une este objeto con otro si son compatibles
    /// </summary>
    public void Attach(ToolComponent other)
    {
        if (IsCompatibleWith(other) && attachedComponent == null)
        {
            attachedComponent = other;
            other.transform.SetParent(attachPoint); // Lo anidamos
            StartCoroutine(SmoothAttach(other));
        }
    }

    /// <summary>
    /// Acopla el objeto visualmente con una interpolación suave
    /// </summary>
    private IEnumerator SmoothAttach(ToolComponent other)
    {
        Vector3 startPos = other.transform.position;
        Quaternion startRot = other.transform.rotation;

        Vector3 targetPos = attachPoint.position;
        Quaternion targetRot = attachPoint.rotation;

        float duration = 0.4f;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            float factor = t / duration;

            other.transform.position = Vector3.Lerp(startPos, targetPos, factor);
            other.transform.rotation = Quaternion.Lerp(startRot, targetRot, factor);

            yield return null;
        }

        // Asegura posición final
        other.transform.position = targetPos;
        other.transform.rotation = targetRot;

        // Desactiva físicas del complemento
        if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }
    }
}

