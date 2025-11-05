using UnityEngine;

public class DetectarLevasNissan : MonoBehaviour
{
    public MoverPiezaNissan[] moverPiezaNissan;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Snap"))
        {
            // Iniciamos desplazamiento de retenedor de valvulas y demas
            for (int i = 0; i < moverPiezaNissan.Length;  i++)
            {
                moverPiezaNissan[i].IniciarDesplazamientoObjeto();
            }
        }
    }
}
