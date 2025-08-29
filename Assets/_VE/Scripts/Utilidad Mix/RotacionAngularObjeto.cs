using UnityEngine;

public class RotacionAngularObjeto : MonoBehaviour
{
    public enum EjeRotacion { X, Y, Z }
    public EjeRotacion eje = EjeRotacion.Z;

    [Header("Parámetros")]
    public float anguloMaximo = 15f;
    public GameObject[] herramientasManipulables; // Para referenciar a cada tido de herramienta con la que podemos interactuar

    [HideInInspector]
    public float velocidad = 0f; // valor que se actualizará desde fuera
    [HideInInspector]
    public bool estaManipulando; 

    private float anguloInicial;
    private float tiempoAnimacion = 0f;

    public static RotacionAngularObjeto singleton;

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

    void Start()
    {
        // Guardamos ángulo inicial en el eje seleccionado
        anguloInicial = GetValorEje(transform.localEulerAngles);
    }

    void Update()
    {
        if (estaManipulando)
        {
            tiempoAnimacion += Time.deltaTime * velocidad;

            float rotacion = Mathf.Sin(tiempoAnimacion) * anguloMaximo;
            float nuevoAngulo = anguloInicial + rotacion;

            // Aplicamos solo en el eje elegido
            Vector3 euler = transform.localEulerAngles;
            switch (eje)
            {
                case EjeRotacion.X:
                    euler.x = nuevoAngulo;
                    break;
                case EjeRotacion.Y:
                    euler.y = nuevoAngulo;
                    break;
                case EjeRotacion.Z:
                    euler.z = nuevoAngulo;
                    break;
            }

            transform.localRotation = Quaternion.Euler(euler);
        }    
    }

    private float GetValorEje(Vector3 euler)
    {
        switch (eje)
        {
            case EjeRotacion.X: return euler.x;
            case EjeRotacion.Y: return euler.y;
            default: return euler.z;
        }
    }

    /// <summary>
    /// Método para actualizar desde fuera (ej: slider.value)
    /// </summary>
    public void SetVelocidad(float valor)
    {
        velocidad = valor;
    }

    /// <summary>
    /// Para habilitar nuevamente las herramientas
    /// </summary>
    public void ReiniciarHerramientasRotatorias()
    {
        for (int i = 0; i < herramientasManipulables.Length; i++)
        {
            herramientasManipulables[i].SetActive(false);
        }
    }
}
