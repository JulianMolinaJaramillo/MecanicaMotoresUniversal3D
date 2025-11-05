using System.Collections;
using UnityEngine;

public class MoverPiezaNissan : MonoBehaviour
{
    [Header("Objetivos de transformación")]
    public Vector3 posicionObjetivo; // Camara objetivo
    public Quaternion rotacionObjetivo; // Camara objetivo
    public Vector3 escalaObjetivo = Vector3.one; // Escala objetivo

    [Header("Parámetros de movimiento")]
    public float velocidadMovimiento = 0.5f; // Velocidad de desplazamiento
    public float tiempoFrecuencia = 0.1f; // Velocidad de desplazamiento
    public bool soloRotacion; // Si unicamente quiero rotar el objeto
    public bool soloEscala; // Solo escalar

    private Vector3 posicionInicial; // Para saber si debo o no mover la camara
    private Quaternion rotacionInicial; // Para saber si debo o no mover la camara
    private Vector3 escalaInicial;

    private Coroutine movimientoActual; // Referencia de la corrutina activa
    private void Start()
    {
        posicionInicial = transform.localPosition; //  Guardamos la posicion de inicial
        rotacionInicial = transform.localRotation; //  Guardamos la rotacion de inicial
        escalaInicial = transform.localScale; //Guardamos la escala de inicial
    }

    [ContextMenu("Mover")]
    public void IniciarDesplazamientoObjeto()
    {
        // Si ya se está ejecutando una corrutina, la detenemos primero
        if (movimientoActual != null)
        {
            StopCoroutine(movimientoActual);
        }
        movimientoActual = StartCoroutine(MoverObjetoA(posicionObjetivo, rotacionObjetivo, escalaObjetivo, velocidadMovimiento));
    }

    [ContextMenu("Devolver")]
    public void RetornarPosicionOriginal()
    {
        // Si ya se está ejecutando una corrutina, la detenemos primero
        if (movimientoActual != null)
        {
            StopCoroutine(movimientoActual);
        }
        movimientoActual = StartCoroutine(MoverObjetoA(posicionInicial, rotacionInicial, escalaInicial, velocidadMovimiento));
    }

    /// <summary>
    /// Corrutina encargada del movimiento de la pieza suavizado
    /// </summary>
    /// <param name="posicionDeseada">La posición local deseada</param>
    /// <param name="rotacionDeseada">La rotación local deseada</param>
    /// <param name="duracion">Duración del movimiento</param>
    public IEnumerator MoverObjetoA(Vector3 posicionDeseada, Quaternion rotacionDeseada, Vector3 escalaDeseada, float duracion)
    {
        Vector3 posicionInicio = transform.localPosition;
        Quaternion rotacionInicio = transform.localRotation;
        Vector3 escalaInicio = transform.localScale;

        float tiempo = 0f;

        while (tiempo < duracion)
        {
            float t = tiempo / duracion;

            if (soloEscala)
            {
                // Escalar sin modificar X/Y, pero sí interpolar la Z hacia la posición objetivo
                transform.localScale = Vector3.Lerp(escalaInicio, escalaDeseada, t);

                Vector3 nuevaPos = posicionInicial;
                nuevaPos.z = Mathf.Lerp(posicionInicio.z, posicionDeseada.z, t);
                transform.localPosition = nuevaPos;

                transform.localRotation = rotacionInicial; // Mantiene la rotación fija
            }
            else
            {
                // Movimiento y rotación normales
                if (!soloRotacion)
                    transform.localPosition = Vector3.Lerp(posicionInicio, posicionDeseada, t);

                transform.localRotation = Quaternion.Lerp(rotacionInicio, rotacionDeseada, t);
            }

            tiempo += Time.deltaTime;
            yield return null;
        }

        if (soloEscala)
        {
            transform.localScale = escalaDeseada;

            Vector3 posFinal = posicionInicial;
            posFinal.z = posicionDeseada.z; // Aseguramos posición final Z correcta
            transform.localPosition = posFinal;

            transform.localRotation = rotacionInicial;
        }
        else
        {
            if (!soloRotacion)
                transform.localPosition = posicionDeseada;
            transform.localRotation = rotacionDeseada;
        }

        yield return new WaitForSeconds(tiempoFrecuencia);
        RetornarPosicionOriginal();
    }
}
