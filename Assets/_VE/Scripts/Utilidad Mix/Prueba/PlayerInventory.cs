using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Herramienta actualmente en la mano")]
    public ToolComponent heldTool;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Clic izquierdo
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 3f))
            {
                ToolComponent clickedTool = hit.collider.GetComponent<ToolComponent>();

                if (clickedTool != null)
                {
                    PickUpTool(clickedTool);
                }
            }
        }
    }

    /// <summary>
    /// Intenta recoger una herramienta o acoplarla si ya hay una en mano
    /// </summary>
    void PickUpTool(ToolComponent tool)
    {
        if (heldTool == null)
        {
            heldTool = tool;
            tool.transform.SetParent(transform);
            tool.transform.localPosition = new Vector3(0.5f, 0, 1); // Posición relativa en mano
            tool.transform.localRotation = Quaternion.identity;

            if (tool.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.isKinematic = true;
            }
        }
        else
        {
            // Verifica compatibilidad y acopla
            if (heldTool.IsCompatibleWith(tool))
            {
                heldTool.Attach(tool);
            }
            else if (tool.IsCompatibleWith(heldTool))
            {
                tool.Attach(heldTool);
                heldTool = tool; // Ahora sostenemos el nuevo como base
            }
            else
            {
                Debug.Log("No son compatibles.");
            }
        }
    }
}

