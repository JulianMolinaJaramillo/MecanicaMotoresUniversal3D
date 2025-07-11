using UnityEngine;

public class ActivarMaterialesDisolverHijos : MonoBehaviour
{
    public float tiempoDisolver;
    /// <summary>
    /// Activa todos los hijos actuales del objeto padre (este GameObject)
    /// </summary>
    public void ActivarMaterialesDisolucion()
    {
        if (transform.childCount > 0)
        {
            foreach (Transform hijo in transform)
            {
                MoverPieza pieza = hijo.GetComponent<MoverPieza>();
                pieza.AgregarDisolver(tiempoDisolver);

            }
        }
    }
}
