using System.Collections;
using UnityEngine;

public class TemblorObjeto : MonoBehaviour
{
    public float intensidad = 0.1f;      // Qué tan fuerte es el temblor
    public float duracion = 1.0f;        // Cuánto dura el temblor (en segundos)

    private Vector3 posicionInicial;

    private void OnEnable()
    {
        StopAllCoroutines(); // Opcional: cancela cualquier temblor anterior
        StartCoroutine(Temblar(duracion));
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
    }
}
