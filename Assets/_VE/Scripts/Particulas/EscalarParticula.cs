using System.Collections;
using UnityEngine;

public class EscalarParticula : MonoBehaviour
{
    public Vector3 escalaInicial = Vector3.one;
    public Vector3 escalaFinal = Vector3.one * 2f;
    public float duracion = 2f;

    void Start()
    {
        transform.localScale = escalaInicial;
        StartCoroutine(EscalarSuavemente());
    }

    IEnumerator EscalarSuavemente()
    {
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            float t = tiempo / duracion;
            transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t);

            tiempo += Time.deltaTime;
            yield return null;
        }

        transform.localScale = escalaFinal;
    }
}
