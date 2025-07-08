
using UnityEngine;

public class MesaMotor : MonoBehaviour
{
    [Header("ESTA ES UNA CLASE SINGLETON")]
    public bool mesaMotorActiva;
    public RotadorPiezas[] rotadorPiezas;
    public ExpansionRadial[] expansionRadials;

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

    public void DetenerInteraccionesMotor()
    {
        for (int i = 0; i < rotadorPiezas.Length; i++)
        {
            rotadorPiezas[i].RegresarARotacionOriginal();
            rotadorPiezas[i].dejarDeRotar = true;
        }

        expansionRadials[0].Contraer();
        //for (int i = 0; i < expansionRadials.Length; i++)
        //{
        //    expansionRadials[i].Contraer();
        //    expansionRadials[i].noInteractuar = true;
        //}
    }
}
