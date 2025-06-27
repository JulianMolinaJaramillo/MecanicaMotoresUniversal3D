using TMPro;
using UnityEngine;

public class ManagerCanvas : MonoBehaviour
{
    [Header("ESTA ES UNA CLASE SINGLETON")]
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public TextMeshProUGUI txtTituloPieza; // Referencia al texto titulo de la pieza
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public TextMeshProUGUI txtDescripcionPieza; // Referencia al texto descripcion para la pieza
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject mensajeAlerta;
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public TextMeshProUGUI txtMensaje; // Referencia al texto que nos indica si algo esta incorrecto o el inventario esta lleno
    
    public static ManagerCanvas singleton;

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

    /// <summary>
    /// Metodo utilizado para actualizar la informacion de la pieza tomada de la mesa
    /// </summary>
    /// <param name="titulo"> Nombre tecnico de la pieza</param>
    /// <param name="descripcion"> Descripcion de para que sirve esa pieza </param>
    public void ActualizarInformacionPieza(string titulo, string descripcion)
    {
        txtTituloPieza.text = titulo;
        txtDescripcionPieza.text = descripcion;
    }

    /// <summary>
    /// Metodo utilizaod para borrar la informacion del titulo y descripcion
    /// </summary>
    public void BorrarInformacionPieza()
    {
        txtTituloPieza.text = "";
        txtDescripcionPieza.text = "";
    }

    /// <summary>
    /// Para habilitar la notificacion de inventario lleno o cualquier otro mensaje de alerta
    /// </summary>
    public void AlertarMensaje(string texto)
    {
        txtMensaje.text = texto;
        mensajeAlerta.SetActive(true);
    }
}
