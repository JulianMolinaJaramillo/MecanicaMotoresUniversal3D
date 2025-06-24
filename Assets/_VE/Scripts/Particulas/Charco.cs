using UnityEngine;

public class Charco : MonoBehaviour
{
    public float duracion = 0.5f;             // Tiempo que tarda en expandirse
    public float escalaFinal = 1.5f;          // Cuánto se expandirá en X y Z
    public AnimationCurve curvaDeExpansion;   // Curva para animar suavemente

    private Vector3 escalaInicial;
    private Vector3 escalaObjetivo;
    private float tiempoActual = 0f;

    void Start()
    {
        escalaInicial = transform.localScale;
        escalaObjetivo = new Vector3(escalaFinal, escalaInicial.y, escalaFinal);
    }

    void Update()
    {
        if (tiempoActual < duracion)
        {
            tiempoActual += Time.deltaTime;
            float t = Mathf.Clamp01(tiempoActual / duracion);
            float curvaT = curvaDeExpansion.Evaluate(t);

            // Escalamos solo en X y Z, mantenemos Y
            float nuevaX = Mathf.Lerp(escalaInicial.x, escalaObjetivo.x, curvaT);
            float nuevaZ = Mathf.Lerp(escalaInicial.z, escalaObjetivo.z, curvaT);
            transform.localScale = new Vector3(nuevaX, escalaInicial.y, nuevaZ);
        }
    }
}
