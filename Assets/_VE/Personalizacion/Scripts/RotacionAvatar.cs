using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotacionAvatar : MonoBehaviour
{
    private bool rotando = false; // Para rastrear si el clic está sostenido
    public float velocidadRotacion = 70f; // Velocidad de rotación ajustable
    public float velocidadRetorno = 1f;
    private Vector3 utilmaPosMouse; // Última posición del mouse

    private Quaternion rotacionInicial; // Guardamos la rotación inicial
    private Coroutine resetCoroutine;   // Para evitar múltiples llamadas simultáneas

    void Start()
    {
        // Guardamos la rotación con la que empieza el objeto
        rotacionInicial = transform.rotation;
    }
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
    /// Metodo invocado desde btnBack para reestablecer la posicion del avatar de forma suavizada
    /// </summary>
    [ContextMenu("regresar")]
    public void ReestablecerPosicion()
    {
        // Si ya hay una corrutina corriendo, la detenemos
        if (resetCoroutine != null) StopCoroutine(resetCoroutine);

        resetCoroutine = StartCoroutine(RotacionSuavizada(rotacionInicial, velocidadRetorno)); // 1f = duración en segundos
    }

    private IEnumerator RotacionSuavizada(Quaternion destino, float duracion)
    {
        Quaternion inicio = transform.rotation;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            transform.rotation = Quaternion.Slerp(inicio, destino, tiempo / duracion);
            tiempo += Time.deltaTime;
            yield return null;
        }

        transform.rotation = destino; // asegurar posición final exacta
        resetCoroutine = null;
    }
}
