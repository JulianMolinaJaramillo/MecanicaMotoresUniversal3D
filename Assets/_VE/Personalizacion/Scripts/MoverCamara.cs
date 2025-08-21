using System.Collections;
using UnityEngine;

public class MoverCamara : MonoBehaviour
{
    public Transform camara; // Camara objetivo
    public Transform posicionInicial;
    public Transform[] posicionesCamara; // Lista de posiciones de cámara para ejercer una rotacion
    public float velocidadPos = 1; // Velocidad de desplazamiento

    private Coroutine miCoroutine;
    public static MoverCamara singleton;

    private void Awake()
    {
        // Configurar Singleton
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this);
        }
    }

    [ContextMenu("mover")]
    public void ReestablecerPosicionCamara()
    {
        if (miCoroutine != null)
        {
            StopCoroutine(miCoroutine);
        }

        miCoroutine = StartCoroutine(MoverCamaraInterpolada(posicionInicial, velocidadPos));
    }

    public void IniciarMovimientoCamara(int posicionDeseada)
    {
        if (miCoroutine != null)
        {
            StopCoroutine(miCoroutine);
        }

        miCoroutine = StartCoroutine(MoverCamaraInterpolada(posicionesCamara[posicionDeseada], velocidadPos));
    }

    /// <summary>
    /// Currutina encargada del movimiento de la pieza suavizado
    /// </summary>
    /// <param name="posicionDeseada"> La posicion a la cual queremos moder la camara </param>
    /// <param name="duracion"> Tiempo del movimiento de la pieza </param
    private IEnumerator MoverCamaraInterpolada(Transform posicionDeseada, float duracion)
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
    }
}
