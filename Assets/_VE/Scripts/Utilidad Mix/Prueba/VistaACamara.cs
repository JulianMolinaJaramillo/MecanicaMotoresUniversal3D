using UnityEngine;

public class VistaACamara : MonoBehaviour
{
    public Camera mainCamera;
    private float rotacionInicial;

    void Start()
    {
        // Guardamos la rotación inicial en Z
        rotacionInicial = transform.eulerAngles.z;
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);

        
        Vector3 euler = transform.eulerAngles;
        euler.z = rotacionInicial;
        transform.eulerAngles = euler;
    }
}
