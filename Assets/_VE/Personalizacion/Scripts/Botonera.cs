using UnityEngine.UI;
using UnityEngine;

public class Botonera : MonoBehaviour
{
    [Header("Color que representa este botón")]

    public Button[] botonesRazas;
    public Button[] botonesMorfologia;
    public Button[] botonesTrajes;


    void Start()
    {

    }

    public void EsconderBotones(string dato)
    {
        if (dato == "Bruto")
        {
            for (int i = 0; i < botonesTrajes.Length; i++)
            {
                if (botonesTrajes[i].name == "btnArmadura" || botonesTrajes[i].name == "btnSinAtuendo")
                {
                    botonesTrajes[i].gameObject.SetActive(true);
                }
                else
                {
                    botonesTrajes[i].gameObject.SetActive(false);
                }
            }
        }
        else if (dato == "Antiheroe")
        {
            for (int i = 0; i < botonesTrajes.Length; i++)
            {
                if (botonesTrajes[i].name == "btnArmadura")
                {
                    botonesTrajes[i].gameObject.SetActive(true);
                }
                else
                {
                    botonesTrajes[i].gameObject.SetActive(false);
                }
            }
        }
        else if (dato == "GenioMal")
        {
            for (int i = 0; i < botonesTrajes.Length; i++)
            {
                if (botonesTrajes[i].name == "btnTunica")
                {
                    botonesTrajes[i].gameObject.SetActive(true);
                }
                else
                {
                    botonesTrajes[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
