using UnityEngine;

public class RotacionAvatar : MonoBehaviour
{
    private bool rotando = false; // Para rastrear si el clic está sostenido
    public float velocidadRotacion = 70f; // Velocidad de rotación ajustable
    private Vector3 utilmaPosMouse; // Última posición del mouse

    void Update()
    {
        // Detecta si el clic izquierdo del mouse comenzó
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Comprueba si el rayo golpea este objeto
            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                rotando = true; // Comienza la rotación
                utilmaPosMouse = Input.mousePosition; // Registra la posición inicial del mouse
            }
        }

        // Detecta si se suelta el clic izquierdo del mouse
        if (Input.GetMouseButtonUp(0))
        {
            rotando = false; // Detiene la rotación
        }

        // Si está en modo rotación, calcula el desplazamiento del mouse
        if (rotando)
        {
            Vector3 actualPosicion = Input.mousePosition;
            float deltaX = actualPosicion.x - utilmaPosMouse.x; // Cambio horizontal del mouse

            // Gira el objeto en función del desplazamiento
            transform.Rotate(Vector3.up, -deltaX * velocidadRotacion * Time.deltaTime);

            // Actualiza la posición del mouse para la próxima iteración
            utilmaPosMouse = actualPosicion;
        }
    }

    /// <summary>
    /// Metodo invocado desde btnBack para reestablecer la posicion del avatar
    /// </summary>
    public void ReestablecerPosicion()
    {
        transform.eulerAngles = new Vector3(0f, 0f, 0f); //Establecemos las rotaciones en cero
    }
}
