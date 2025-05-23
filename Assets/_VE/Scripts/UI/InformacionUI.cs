
using TMPro;
using UnityEngine;

public class InformacionUI : MonoBehaviour
{
    [Header("ESTA ES UNA CLASE SINGLETON")]
    public TextMeshProUGUI txtTitulo; // Referencia al texto titulo
    public TextMeshProUGUI txtDescripcion; // Referencia al texto descripcion

    public static InformacionUI singleton;

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
}
