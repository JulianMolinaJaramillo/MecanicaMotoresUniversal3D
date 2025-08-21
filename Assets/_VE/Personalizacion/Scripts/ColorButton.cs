using UnityEngine.UI;
using UnityEngine;

public class ColorButton : MonoBehaviour
{
    [Header("Color que representa este botón")]
    public Color colorAsignado;
    public GestorPersonalizacion gestor;
    private Button boton;

    void Start()
    {
        boton = GetComponent<Button>();
        boton.onClick.AddListener(EnviarColor);
    }

    void EnviarColor()
    {
        gestor.CambiarColorConIntensidad(colorAsignado);
    }
}
