using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TemblorObjeto : MonoBehaviour
{
    public float intensidad = 0.1f;      // Qué tan fuerte es el temblor
    public float duracion = 1.0f;        // Cuánto dura el temblor (en segundos)
    public bool temblarAlActivar; 
    private Vector3 posicionInicial;
    private Coroutine coroutine;


    private void OnEnable()
    {
        if (temblarAlActivar)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            coroutine = StartCoroutine(Temblar(duracion));
        }    
    }

    [ContextMenu("Temblar")]
    public void Tiembla()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        
        coroutine = StartCoroutine(Temblar(duracion));
    }

    private IEnumerator Temblar(float tiempo)
    {
        posicionInicial = transform.localPosition;
        float tiempoRestante = tiempo;

        while (tiempoRestante > 0)
        {
            float offsetX = Random.Range(-intensidad, intensidad);
            float offsetY = Random.Range(-intensidad, intensidad);

            transform.localPosition = posicionInicial + new Vector3(offsetX, offsetY, 0);

            tiempoRestante -= Time.deltaTime;
            yield return null;
        }

        // Volver a la posición original al finalizar
        transform.localPosition = posicionInicial;
        coroutine = null;
    }
}
