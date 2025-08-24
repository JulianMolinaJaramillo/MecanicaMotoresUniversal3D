using UnityEngine.UI;
using UnityEngine;

public class Botonera : MonoBehaviour
{
    [Header("Color que representa este botón")]

    public Button[] botonesRazas;
    public Button[] botonesMorfologia;
    public Button[] botonesTrajes;

    public GameObject sexoMasculinoActivo;
    public GameObject sexoFemeninoActivo;

    public ButtonHide buttonHideDemonio;
    public ButtonHide buttonHideHumano;
    public ButtonHide buttonHideHibrido;
    public ButtonHide buttonHideBestia;
    public ButtonHide buttonHideExtraterrestre;
    public ButtonHide buttonHideSuperHumano;

    public bool[] razaActiva;

    public void EsconderBotones(string dato)
    {
        if (razaActiva[2]) // Si es demonio
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
            else if (dato == "GenioDelMal")
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

    public void RecibirClaveActual(string clave)
    {
        // Dividir en partes por el separador "_"
        string[] partes = clave.Split('_');

        // Ahora cada parte está en un índice diferente del array
        string raza = partes[0];       // "Raza"
        string rol = partes[1];        // "Morfologia"
        string atuendo = partes[2];    // "Traje"
        string sexo = partes[3];       // "Sexo"

        if (partes[3] == "Hombre")
        {
            sexoFemeninoActivo.SetActive(false);
            sexoMasculinoActivo.SetActive(true);
        }
        else
        {
            sexoFemeninoActivo.SetActive(true);
            sexoMasculinoActivo.SetActive(false);
        }

        if (partes[0] == "Demonio")
        {
            HabilitarBooleanos(2);
            buttonHideDemonio.EsconderBotones();
            EsconderBotones(partes[1]);
        }
    }

    public void HabilitarBooleanos(int dato)
    {
        for (int i = 0; i < razaActiva.Length; i++)
        {
            if (i == dato)
            {
                razaActiva[i] = true;
            }
            else
            {
                razaActiva[i] = false;
            }        
        }
    }
}
