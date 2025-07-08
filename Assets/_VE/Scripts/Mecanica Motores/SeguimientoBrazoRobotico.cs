using UnityEngine;

public class SeguimientoBrazoRobotico : MonoBehaviour
{
    public Transform ikTarget;
    public Camera mainCamera;
    public float distanceFromCamera = 5f;

    void Update()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = distanceFromCamera; // distancia desde la cámara
        
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        worldPosition.z = -12.048f; // limitar el eje Z si es necesario

        ikTarget.position = worldPosition;
    }
}
