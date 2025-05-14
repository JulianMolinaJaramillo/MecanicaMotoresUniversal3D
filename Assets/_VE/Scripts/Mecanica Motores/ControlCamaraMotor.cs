using System.Collections;
using UnityEngine;

public class ControlCamaraMotor : MonoBehaviour
{
    public Transform camara; // Camara objetivo
    public Transform posicionUp, posicionDown, posicionLeft, posicionRight; // Posiciones en los puntos donde queremos mover la camara
    public float velocidadPos; // Velocidad de desplazamiento
    private bool noMover; // Para saber si debo o no mover la camara

    private void Update()
    {
        if (!noMover)
        {
            // Validamos si presionamos las flechas de direccion del tecla o las teclas ASDW
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                StartCoroutine(MoverCamara(posicionUp, velocidadPos));
                noMover = true;
            }

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                StartCoroutine(MoverCamara(posicionDown, velocidadPos));
                noMover = true;
            }

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                StartCoroutine(MoverCamara(posicionLeft, velocidadPos));
                noMover = true;
            }
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                StartCoroutine(MoverCamara(posicionRight, velocidadPos));
                noMover = true;
            }
        }
    }
    /// <summary>
    /// Currutina encargada del movimiento de la pieza suavizado
    /// </summary>
    /// <param name="posicionDeseada"> La posicion a la cual queremos moder la camara </param>
    /// <param name="duracion"> Tiempo del movimiento de la pieza </param>

    public IEnumerator MoverCamara(Transform posicionDeseada, float duracion)
    {
        Vector3 posicionInicio = camara.transform.position; //  Guardamos la posicion de inicio
        Quaternion rotacionInicio = camara.transform.rotation; //  Guardamos la rotacion de inicio

        float tiempo = 0f; // Damos un tiempo para la interpolacion

        while (tiempo < duracion)
        {
            // Asignamos la posicion y rotacion de la camara, con interpolacion lineal
            camara.transform.position = Vector3.Lerp(posicionInicio, posicionDeseada.position, tiempo / duracion);
            camara.transform.rotation = Quaternion.Lerp(rotacionInicio, posicionDeseada.rotation, tiempo / duracion);

            tiempo += Time.deltaTime;
            yield return null;
        }

        camara.transform.position = posicionDeseada.position; // Aseguramos la posición final
        camara.transform.rotation = posicionDeseada.rotation; // Aseguramos la rotacion final

        noMover = false; // indicamos que podemos volver a mover
    }
}
