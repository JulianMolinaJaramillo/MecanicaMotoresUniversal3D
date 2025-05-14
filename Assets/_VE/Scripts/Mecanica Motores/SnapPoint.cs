
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    public string snapID; // Identificador del tipo de pieza que puede conectarse aquí

    private void OnTriggerStay(Collider other)
    {
        // Verificamos por tag
        if (other.CompareTag("SnapPoint"))
        {
            SnapPoint snap = other.GetComponent<SnapPoint>(); // Obtenemos el script del objeto que colisiona
            if (snapID == snap.snapID) // Si coinciden los ID de los snapPoint
            {
                MoverPieza moverPiezaPadre = snap.GetComponentInParent<MoverPieza>(); // Obtenemos el script del padre del objeto que colisiona
                if (!moverPiezaPadre.piezaColocada) // Si la pieza no esta colocada aun
                {
                    moverPiezaPadre.AgregarSegundoMaterial(1); // Le asignamos el material verde de pieza correcta

                    if (moverPiezaPadre.puedoValidar)// Se activa al momento de soltar el mouse de la piza seleccionada
                    {
                        moverPiezaPadre.IniciarMovimiento(); // Iniciamos el desplazamiento de la pieza
                    }
                }           
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Verificamos por tag
        if (other.CompareTag("SnapPoint"))
        {
            SnapPoint snap = other.GetComponent<SnapPoint>(); // Obtenemos el script del objeto que salió de la colision
            if (snapID == snap.snapID) // Si coinciden los ID de los snapPoint
            {
                MoverPieza moverPiezaPadre = snap.GetComponentInParent<MoverPieza>(); // Obtenemos el script del padre del objeto que salio de la colision
                if (!moverPiezaPadre.piezaColocada)
                {
                    moverPiezaPadre.AgregarSegundoMaterial(0); // Le agregamos el material rojo
                }         
            }
        }
    }
}


