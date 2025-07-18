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
            if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("AparecerPieza"); // Ejecutamos el efecto nombrado
            foreach (Transform hijo in transform)
            {
                if (!hijo.gameObject.activeSelf) // Solo si está inactivo
                {
                    hijo.gameObject.SetActive(true);

                    GuardarPieza pieza = hijo.GetComponent<GuardarPieza>();
                    if (pieza != null)
                    {
                        pieza.AgregarMaterialDisolver(0);
                    }
                }
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
