using UnityEngine;

public class ManagerMinijuego : MonoBehaviour
{
    public bool minijuegoActivo;
    public int sizeHerramienta;
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject miniJuegoAtornillar;
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public Transform[] posicionesMinijuego1;

    [HideInInspector]
    public Transform posicionMonijuegoActual;

    private int contador = 0;
    public static ManagerMinijuego singleton;

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

    private void Start()
    {
        posicionMonijuegoActual = posicionesMinijuego1[0];
    }

    public void ActivarMinijuego()
    {
        minijuegoActivo = true;
        miniJuegoAtornillar.SetActive(true);
        ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuego1[0], 1);
    }

    public void DesactivarMinijuego()
    {
        miniJuegoAtornillar.SetActive(false);
    }

    public void TorqueAplicado()
    {
        contador += 1;
        if (contador < 4)
        {
            ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuego1[contador], 1);
            posicionMonijuegoActual = posicionesMinijuego1[contador];
        }
        else
        {
            ControlCamaraMotor.singleton.IniciarMovimientoCamara(ControlCamaraMotor.singleton.posicionDown, 1);
            contador = 0;
            minijuegoActivo = false;
            DesactivarMinijuego();
        }
    }
}
