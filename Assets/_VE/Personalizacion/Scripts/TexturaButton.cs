using UnityEngine.UI;
using UnityEngine;

public class TexturaButton : MonoBehaviour
{
    [Header("Textura que representa este botón")]
    public Texture texturaAsignada;
    public GestorPersonalizacion gestor;
    private Button boton;

    void Start()
    {
        boton = GetComponent<Button>();
        boton.onClick.AddListener(EnviarColor);
    }

    void EnviarColor()
    {
        gestor.CambiarTextura(texturaAsignada);
    }
}
