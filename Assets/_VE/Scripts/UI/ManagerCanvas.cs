
using TMPro;
using UnityEngine;

public class ManagerCanvas : MonoBehaviour
{
    [Header("ESTA ES UNA CLASE SINGLETON")]
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public TextMeshProUGUI txtTitulo; // Referencia al texto titulo
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public TextMeshProUGUI txtDescripcion; // Referencia al texto descripcion
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject inventarioLleno;

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
        txtTitulo.text = titulo;
        txtDescripcion.text = descripcion;
    }

    /// <summary>
    /// Metodo utilizaod para borrar la informacion del titulo y descripcion
    /// </summary>
    public void BorrarInformacionPieza()
    {
        txtTitulo.text = "";
        txtDescripcion.text = "";
    }

    /// <summary>
    /// Para habilitar la notificacion de inventario lleno
    /// </summary>
    public void NotificarInventarioLLeno()
    {
        inventarioLleno.SetActive(true);
    }
}
