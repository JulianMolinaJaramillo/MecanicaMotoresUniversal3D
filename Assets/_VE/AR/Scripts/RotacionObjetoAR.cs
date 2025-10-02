using UnityEngine;

public class RotacionObjetoAR : MonoBehaviour
{
    // Velocidad de rotación en grados por segundo
    public float velocidadRotacion = 50f;
    public bool rotarEnZ, rotarEnY, rotarEnX;
    public float velocidadRetorno = 2f; // Velocidad del retorno suave

    private Quaternion rotacionInicial;
    private bool regresandoARotacionOriginal = false;
    
    void Start()
    {
        // Guardar la rotación inicial
        rotacionInicial = transform.rotation;
    }

    void Update()
    {
        if (regresandoARotacionOriginal)
        {
            // Interpolación suave hacia la rotación original
            transform.rotation = Quaternion.Lerp(transform.rotation, rotacionInicial, Time.deltaTime * velocidadRetorno);

            return;
        }

        if (rotarEnZ)
        {
            // Rotar el objeto que tenga el script alrededor del eje Z
            transform.Rotate(0, 0, velocidadRotacion * Time.deltaTime);
        }
        else if (rotarEnY)
        {
            // Rotar el objeto que tenga el script alrededor del eje Y
            transform.Rotate(0, velocidadRotacion * Time.deltaTime, 0);
        }
        else if (rotarEnX)
        {
            // Rotar el objeto que tenga el script alrededor del eje X
            transform.Rotate(velocidadRotacion * Time.deltaTime, 0, 0);
        }
    }

    /// <summary>
    /// Metodo para retornar a la rotacion original
    /// </summary>
    public void RegresarARotacionOriginal()
    {
        rotarEnY = false;
        rotarEnX = false;
        rotarEnZ = false;
        regresandoARotacionOriginal = true;
    }

    public void RotarEnX()
    {
        rotarEnY = false;
        rotarEnZ = false;
        rotarEnX = true;
    }

    public void RotarEnY()
    {
        rotarEnX = false;
        rotarEnZ = false;
        rotarEnY = true;
    }

    public void RotarEnZ()
    {
        rotarEnY = false;
        rotarEnX = false;
        rotarEnZ = true;       
    }

    public void RotarEnTodosLosEjes()
    {
        rotarEnY = true;
        rotarEnX = true;
        rotarEnZ = true;
    }
}
