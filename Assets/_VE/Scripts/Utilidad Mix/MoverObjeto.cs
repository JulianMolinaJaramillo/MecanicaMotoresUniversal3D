using System.Collections;
using UnityEngine;

public class MoverObjeto : MonoBehaviour
{   
    public Vector3 posicionObjetivo; // Camara objetivo
    public Quaternion rotacionObjetivo; // Camara objetivo
    public float velocidadMovimiento = 1f; // Velocidad de desplazamiento
    public bool soloRotacion; // Si unicamente quiero rotar el objeto

    [Header("SOLO PARA ELEMENTOS UI")]
    public bool elementoUI;
    public GameObject btnAbrir;
    public GameObject btnCerrar;

    private Vector3 posicionInicial; // Para saber si debo o no mover la camara
    private Quaternion rotacionInicial; // Para saber si debo o no mover la camara

    private Coroutine movimientoActual; // Referencia de la corrutina activa
    private void Start()
    {
        posicionInicial = transform.localPosition; //  Guardamos la posicion de inicial
        rotacionInicial = transform.localRotation; //  Guardamos la rotacion de inicial      
    }

    [ContextMenu("Mover")]
    public void IniciarDesplazamientoObjeto()
    {
        // Si ya se está ejecutando una corrutina, la detenemos primero
        if (movimientoActual != null)
        {
            StopCoroutine(movimientoActual);
        }
        movimientoActual = StartCoroutine(MoverObjetoA(posicionObjetivo, rotacionObjetivo, velocidadMovimiento));
    }

    [ContextMenu("Devolver")]
    public void RetornarPosicionOriginal()
    {
        // Si ya se está ejecutando una corrutina, la detenemos primero
        if (movimientoActual != null)
        {
            StopCoroutine(movimientoActual);
        }
        movimientoActual = StartCoroutine(MoverObjetoA(posicionInicial, rotacionInicial, velocidadMovimiento));
    }

    /// <summary>
    /// Corrutina encargada del movimiento de la pieza suavizado
    /// </summary>
    /// <param name="posicionDeseada">La posición local deseada</param>
    /// <param name="rotacionDeseada">La rotación local deseada</param>
    /// <param name="duracion">Duración del movimiento</param>
    public IEnumerator MoverObjetoA(Vector3 posicionDeseada, Quaternion rotacionDeseada, float duracion)
    {
        Vector3 posicionInicio = transform.localPosition; //  Guardamos la posicion de inicio
        Quaternion rotacionInicio = transform.localRotation; //  Guardamos la rotacion de inicio

        float tiempo = 0f; // Damos un tiempo para la interpolacion

        while (tiempo < duracion)
        {
            // Asignamos la posicion y rotacion de la camara, con interpolacion lineal
            if (!soloRotacion) transform.localPosition = Vector3.Lerp(posicionInicio, posicionDeseada, tiempo / duracion);
            transform.localRotation = Quaternion.Lerp(rotacionInicio, rotacionDeseada, tiempo / duracion);

            tiempo += Time.deltaTime;
            yield return null;
        }

        if (!soloRotacion) transform.localPosition = posicionDeseada; // Aseguramos la posición final
        transform.localRotation = rotacionDeseada; // Aseguramos la rotacion final
    }

    private void OnEnable()
    {
        if (elementoUI) 
        {
            btnAbrir.SetActive(false);
            btnCerrar.SetActive(true);
            IniciarDesplazamientoObjeto();
        } 
    }

    public void HabilitarAyuda()
    {
        if (elementoUI)
        {
            btnAbrir.SetActive(false);
            btnCerrar.SetActive(true);
            IniciarDesplazamientoObjeto();
        }
    }
}
