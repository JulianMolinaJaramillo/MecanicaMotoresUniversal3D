
using UnityEngine;

public class PulsoObjeto : MonoBehaviour
{
    public float aumentoEscala = 0.2f;     // Qué tanto cambiará la escala
    public float velocidad = 2f;             // Velocidad de la animación
    private Vector3 escalaInicial;

    void Start()
    {
        escalaInicial = transform.localScale;
    }

    void Update()
    {
        float escalaOffset = Mathf.Sin(Time.time * velocidad) * aumentoEscala;
        transform.localScale = escalaInicial + Vector3.one * escalaOffset;

        // Bloquear rotación en Z
        Vector3 currentRotation = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);
    }
}
