using UnityEngine;

public class ActivarDesactivarHijos : MonoBehaviour
{
    public string motor;
    /// <summary>
    /// Activa todos los hijos directos del objeto actual.
    /// </summary>
    [ContextMenu("Activar")]
    public void ActivarTodosLosHijos()
    {
        if (motor == ManagerMinijuego.singleton.motorArmado)
        {      
            foreach (Transform hijo in transform)
            {
                GuardarPieza pieza = hijo.GetComponent<GuardarPieza>();
                hijo.gameObject.SetActive(true);
                pieza.QuitarMaterial();
            }
        }    
    }


    /// <summary>
    /// Desactiva todos los hijos directos del objeto actual.
    /// </summary>
    [ContextMenu("Desactivar")]
    public void DesactivarTodosLosHijos()
    {
        foreach (Transform hijo in transform)
        {
            hijo.gameObject.SetActive(false);
        }
    }
}
