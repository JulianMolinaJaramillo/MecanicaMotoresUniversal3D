using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MesaMotor : MonoBehaviour
{
    [Header("ESTA ES UNA CLASE SINGLETON")]

    public bool mesaMotorActiva;
    public static MesaMotor singleton;
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
}
