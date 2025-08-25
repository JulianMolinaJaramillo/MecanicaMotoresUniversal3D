using UnityEngine.UI;
using UnityEngine;

public class ButtonHide : MonoBehaviour
{
    [Header("Botones que quiero desactivar segun la raza")]
    public Button[] botonesDesactivar;

    private Button boton;

    void Start()
    {
        boton = GetComponent<Button>();
        boton.onClick.AddListener(EsconderBotones);
    }

    public void EsconderBotones()
    {
        for (int i = 0; i < botonesDesactivar.Length; i++)
        {
            botonesDesactivar[i].gameObject.SetActive(false);
        }
    }

    public void RestablecerBotones()
    {
        for (int i = 0; i < botonesDesactivar.Length; i++)
        {
            botonesDesactivar[i].gameObject.SetActive(true);
        }
    }
}

