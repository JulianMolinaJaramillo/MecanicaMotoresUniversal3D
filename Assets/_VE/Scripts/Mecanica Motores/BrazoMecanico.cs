using UnityEngine;

public class BrazoMecanico : MonoBehaviour
{
    public Transform hombro;
    public Transform codo;
    public Transform mano;
    public Camera cam;

    public float velocidad = 5f;

    void Update()
    {
        // Obtener posición del mouse en mundo
        Vector3 mouseWorldPos = GetMouseWorldPosition();

        // Mover la mano suavemente hacia esa posición
        mano.position = Vector3.Lerp(mano.position, mouseWorldPos, Time.deltaTime * velocidad);

        // Hacer que el codo mire hacia la mano
        codo.LookAt(mano.position);

        // Hacer que el hombro mire hacia el codo
        hombro.LookAt(codo.position);
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
            return hit.point;

        return ray.GetPoint(10); // fallback si no hay colisión
    }
}
