using System.Collections;
using UnityEngine;

public class AumentarEscala : MonoBehaviour
{
    [Header("Configuración")]
    public float escalaObjetivo = 1.5f;     // Escala a la que va a crecer
    public float duracionEscalado = 0.1f;   // Tiempo en segundos para escalar
    public float tiempoEnEscala = 0.1f;       // Tiempo que se queda en escala objetivo
    public GameObject objetoObjetivo;

    private Vector3 escalaOriginal;
    private Coroutine rutinaEscala;

    void Awake()
    {
        escalaOriginal = transform.localScale; // Guardamos la escala inicial
    }

    public void Escalar()
    {
        if (rutinaEscala != null) StopCoroutine(rutinaEscala);
        rutinaEscala = StartCoroutine(EscalaRoutine());
    }

    public void AsignarObjetivo(GameObject objetivo)
    {
        objetoObjetivo = objetivo;
        if (rutinaEscala != null) StopCoroutine(rutinaEscala);
        rutinaEscala = StartCoroutine(EscalaRoutine());
    }

    private IEnumerator EscalaRoutine()
    {
        Vector3 destino = escalaOriginal * escalaObjetivo;

        // Ir hacia la escala objetivo
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duracionEscalado;
            transform.localScale = Vector3.Lerp(escalaOriginal, destino, t);
            yield return null;
        }

        // Mantenerse en la escala objetivo
        yield return new WaitForSeconds(tiempoEnEscala);

        // Volver a la escala original
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duracionEscalado;
            transform.localScale = Vector3.Lerp(destino, escalaOriginal, t);
            yield return null;
        }

        if (objetoObjetivo != null) objetoObjetivo.SetActive(true);
        rutinaEscala = null;
        this.gameObject.SetActive(false);
    }   
}
