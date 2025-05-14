
using UnityEngine;

public class RotacionObjeto : MonoBehaviour
{
    // Velocidad de rotación en grados por segundo
    public float velocidadRotacion = 50f;
    public bool rotarEnZ, rotarEnY, rotarEnX;

    void Update()
    {
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
}
