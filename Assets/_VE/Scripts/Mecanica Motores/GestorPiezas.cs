using System.Collections.Generic;
using UnityEngine;

public class GestorPiezas : MonoBehaviour
{
    public Transform padreRotador; // ← Objeto que tiene el script de rotación

    /// <summary>
    /// Mueve las piezas colocadas al padre rotador
    /// </summary>
    public void TransferirPiezasColocadas()
    {
        List<Transform> piezasAtrasladar = new List<Transform>();

        // Verificamos todos los hijos actuales
        foreach (Transform hijo in this.transform)
        {
            MoverPieza mover = hijo.GetComponent<MoverPieza>();
            if (mover != null && mover.piezaColocada)
            {
                piezasAtrasladar.Add(hijo);
            }
        }

        // Los reemparentamos sin alterar su posición ni rotación global
        foreach (Transform pieza in piezasAtrasladar)
        {
            Vector3 posicionGlobal = pieza.position;
            Quaternion rotacionGlobal = pieza.rotation;

            pieza.SetParent(padreRotador);
            pieza.position = posicionGlobal;
            pieza.rotation = rotacionGlobal;
        }
    }
}
