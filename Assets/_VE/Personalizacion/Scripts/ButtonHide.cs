using UnityEngine.UI;
using UnityEngine;

public class ButtonHide : MonoBehaviour
{
    [Header("Color que representa este botón")]
    
    public Button[] botonesDesactivar;
    private Button boton;

    void Start()
    {
        boton = GetComponent<Button>();
        boton.onClick.AddListener(EsconderBotones);
    }

    void EsconderBotones()
    {
        for (int i = 0; i < botonesDesactivar.Length; i++)
        {
            botonesDesactivar[i].gameObject.SetActive(false);
        }
    }
}

