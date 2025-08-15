using UnityEngine;

/// <summary>
/// Clase utilizada para recolectar los tornillos de la pieza que tenga este script
/// </summary>
public class AsignarTornillos : MonoBehaviour
{
    public ApretarTornillos[] apretarTornillos;


    // Start is called before the first frame update
    void Start()
    {
        if (ManagerMinijuego.singleton != null)
        {
            for (int i = 0; i < apretarTornillos.Length; i++)
            {
                ManagerMinijuego.singleton.tornillosParaApretar.Add(apretarTornillos[i]);         
            }
        }
    }
}
