using UnityEngine;

public class SnapAnimacion : MonoBehaviour
{
    [Header("Configuración general")]
    public float ajuste = 0.01f; // Para ajustar el empuje

    [Header("Modo Rotación")]
    public bool usarRotacionZ; // Para indicar si vamos a rotar o a posicionar
    public float anguloRotacion = 15f; // Grados de rotación en Z al hacer contacto

    [Header("Modo Escala")]
    public bool usarEscala = false;
    public float escalaYObjetivo = 2f;

    Vector3 offset = Vector3.zero; // para calcular la diferencia en las posiciones
    Vector3 posInicial = Vector3.zero; // Para guardar la posicion original
    Quaternion rotacionInicial; //´para guardar la rotacion original
    Vector3 escalaInicial; //´para guardar la rotacion original
    bool enContacto; // para validar si el collider hizo contacto
    
    private void Start()
    {
        posInicial = transform.parent.position; // GUardamos la posicion original
        rotacionInicial = transform.parent.rotation; // GUardamos la rotacion original
        escalaInicial = transform.parent.localScale; // GUardamos la escala original
        offset = transform.parent.position - transform.position; // Calculamos la diferencia
    }

    private void Update()
    {
        if (!enContacto)
        {
            if (usarRotacionZ)
            {
                // Volver a la rotación inicial suavemente
                transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, rotacionInicial, 0.1f);
            }
            else if (usarEscala)
            {
                transform.parent.localScale = Vector3.Lerp(transform.parent.localScale, escalaInicial, 0.1f);
            }
            else
            {
                // Volver a la posición inicial suavemente
                transform.parent.position = Vector3.Lerp(transform.parent.position, posInicial, 0.1f);
            }

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Snap"))
        {
            enContacto = true;
            if (usarRotacionZ)
            {
                // Calculamos rotación en el eje Z
                Quaternion rotacionObjetivo = Quaternion.Euler(transform.parent.rotation.eulerAngles.x, transform.parent.rotation.eulerAngles.y, anguloRotacion);
                //Aplicamos interpolacion suave
                transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, rotacionObjetivo, 0.1f);
            }
            else if (usarEscala)
            {
                Vector3 nuevaEscala = new Vector3(
                    transform.parent.localScale.x,
                    escalaYObjetivo,
                    transform.parent.localScale.z
                );
                transform.parent.localScale = Vector3.Lerp(transform.parent.localScale, nuevaEscala, 0.1f);
            }
            else
            {
                //Calculamos nueva posicion y aplicamos
                transform.parent.position = (other.transform.position.y + ajuste) * Vector3.up + new Vector3(transform.parent.position.x, 0, transform.parent.position.z) + offset.y * Vector3.up;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        enContacto = false;
    }
}
