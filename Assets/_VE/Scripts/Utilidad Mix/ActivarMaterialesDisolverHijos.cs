using UnityEngine;

public class ActivarMaterialesDisolverHijos : MonoBehaviour
{
    /// <summary>
    /// Activa todos los hijos actuales del objeto padre (este GameObject)
    /// </summary>
    public void ActivarMaterialesDisolucion(float tiempoDisolver, int disolverAdentro)
    {
        if (transform.childCount > 0)
        {
            foreach (Transform hijo in transform)
            {
                MoverPieza pieza = hijo.GetComponent<MoverPieza>();

                if (pieza != null)
                {
                    pieza.AgregarDisolver(tiempoDisolver, disolverAdentro);
                }                
            }
        }
    }
}
