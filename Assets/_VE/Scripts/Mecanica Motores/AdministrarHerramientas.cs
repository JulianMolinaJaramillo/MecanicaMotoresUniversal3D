using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdministrarHerramientas : MonoBehaviour
{
    public List<GameObject> herramientas = new List<GameObject>();

    public static AdministrarHerramientas singleton;

    private void Awake()
    {
        // Configurar Singleton
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void ReactivarHerramientas()
    {
        for (int i = 0; i < herramientas.Count; i++)
        {
            herramientas[i].SetActive(true);
        }
    }
}
