using UnityEngine.UI;
using UnityEngine;

public class Botonera : MonoBehaviour
{
    [Header("Color que representa este botón")]

    public Button[] botonesRazas;
    public Button[] botonesMorfologia;
    public Button[] botonesTrajes;

    public GameObject[] sexoMasculinoActivo;
    public GameObject[] sexoFemeninoActivo;

    public ButtonHide buttonHideDemonio;
    public ButtonHide buttonHideHumano;
    public ButtonHide buttonHideHibrido;
    public ButtonHide buttonHideBestia;
    public ButtonHide buttonHideExtraterrestre;
    public ButtonHide buttonHideSuperHumano;

    public bool[] razaActiva;

    /// <summary>
    /// Para saber que raza esta activa
    /// </summary>
    /// <param name="dato"> true o false </param>
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

    /// <summary>
    /// Metodo invocado desde los botones de morfologia para saber que botones de traje se activan
    /// </summary>
    /// <param name="dato"> Morfologia </param>
    public void EsconderBotones(string dato)
    {
        if (razaActiva[0]) // Si es Humano
        {
            if (dato == "Antiheroe")
            {
                for (int i = 0; i < botonesTrajes.Length; i++)
                {
                    if (botonesTrajes[i].name == "btnArmadura" || botonesTrajes[i].name == "btnTrajeNeopreno")
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
            else if (dato == "Gigante")
            {
                for (int i = 0; i < botonesTrajes.Length; i++)
                {
                    if (botonesTrajes[i].name == "btnTunica" || botonesTrajes[i].name == "btnCasual" || botonesTrajes[i].name == "btnArmadura" || botonesTrajes[i].name == "btnTrajeNeopreno")
                    {
                        botonesTrajes[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        botonesTrajes[i].gameObject.SetActive(false);
                    }
                }
            }
            else if (dato == "Heroe")
            {
                for (int i = 0; i < botonesTrajes.Length; i++)
                {
                    if (botonesTrajes[i].name == "btnTunica" || botonesTrajes[i].name == "btnCasual" || botonesTrajes[i].name == "btnArmadura" || botonesTrajes[i].name == "btnTrajeNeopreno")
                    {
                        botonesTrajes[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        botonesTrajes[i].gameObject.SetActive(false);
                    }
                }
            }
            else if (dato == "Normal")
            {
                for (int i = 0; i < botonesTrajes.Length; i++)
                {
                    if (botonesTrajes[i].name == "btnTunica" || botonesTrajes[i].name == "btnSinAtuendo" || botonesTrajes[i].name == "btnArmadura" || botonesTrajes[i].name == "btnTrajeNeopreno")
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

        if (razaActiva[1]) // Si es Suerhumano
        {
            if (dato == "Bruto")
            {
                for (int i = 0; i < botonesTrajes.Length; i++)
                {
                    if (botonesTrajes[i].name == "btnArmadura" || botonesTrajes[i].name == "btnTrajeNeopreno")
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
            else if (dato == "Heroe")
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
            else if (dato == "Normal")
            {
                for (int i = 0; i < botonesTrajes.Length; i++)
                {
                    if (botonesTrajes[i].name == "btnTrajeNeopreno" || botonesTrajes[i].name == "btnSinAtuendo" || botonesTrajes[i].name == "btnArmadura")
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
            else if (dato == "Normal")
            {
                for (int i = 0; i < botonesTrajes.Length; i++)
                {
                    if (botonesTrajes[i].name == "btnTunica" || botonesTrajes[i].name == "btnArmadura" || botonesTrajes[i].name == "btnTrajeNeopreno")
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

        if (razaActiva[3]) // Si es Hibrido
        {
            if (dato == "Normal")
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
            else if (dato == "Heroe")
            {
                for (int i = 0; i < botonesTrajes.Length; i++)
                {
                    if (botonesTrajes[i].name == "btnArmadura" || botonesTrajes[i].name == "btnTrajeNeopreno")
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

        if (razaActiva[4]) // Si es Extraterrestre
        {
            if (dato == "Bruto")
            {
                for (int i = 0; i < botonesTrajes.Length; i++)
                {
                    if (botonesTrajes[i].name == "btnArmadura" || botonesTrajes[i].name == "btnTrajeNeopreno")
                    {
                        botonesTrajes[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        botonesTrajes[i].gameObject.SetActive(false);
                    }
                }
            }
            else if (dato == "Heroe")
            {
                for (int i = 0; i < botonesTrajes.Length; i++)
                {
                    if (botonesTrajes[i].name == "btnArmadura" || botonesTrajes[i].name == "btnTrajeNeopreno" || botonesTrajes[i].name == "btnTunica")
                    {
                        botonesTrajes[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        botonesTrajes[i].gameObject.SetActive(false);
                    }
                }
            }
            else if (dato == "Normal")
            {
                for (int i = 0; i < botonesTrajes.Length; i++)
                {
                    if (botonesTrajes[i].name == "btnArmadura" || botonesTrajes[i].name == "btnTrajeNeopreno")
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
                    if (botonesTrajes[i].name == "btnArmadura" || botonesTrajes[i].name == "btnTrajeNeopreno" || botonesTrajes[i].name == "btnTunica")
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

        if (razaActiva[5]) // Si es Bestia
        {
            if (dato == "Heroe")
            {
                for (int i = 0; i < botonesTrajes.Length; i++)
                {
                    if (botonesTrajes[i].name == "btnArmadura" || botonesTrajes[i].name == "btnTrajeNeopreno")
                    {
                        botonesTrajes[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        botonesTrajes[i].gameObject.SetActive(false);
                    }
                }
            }
            else if (dato == "Normal")
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
        }

    }

    /// <summary>
    /// Recibimos la clave actual cada vez que se actualiza un personaje
    /// </summary>
    /// <param name="clave"> Clave activa </param>
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
            for (int i = 0; i < sexoFemeninoActivo.Length; i++)
            {
                sexoFemeninoActivo[i].SetActive(false);
                sexoMasculinoActivo[i].SetActive(true);
            }
            
        }
        else
        {
            for (int i = 0; i < sexoFemeninoActivo.Length; i++)
            {
                sexoFemeninoActivo[i].SetActive(true);
                sexoMasculinoActivo[i].SetActive(false);
            }            
        }

        if (partes[0] == "Demonio")
        {
            HabilitarBooleanos(2);
            buttonHideHumano.RestablecerBotones();
            buttonHideSuperHumano.RestablecerBotones();
            buttonHideHibrido.RestablecerBotones();
            buttonHideExtraterrestre.RestablecerBotones();
            buttonHideBestia.RestablecerBotones();

            buttonHideDemonio.EsconderBotones();
            EsconderBotones(partes[1]);
        } 
        else if (partes[0] == "Humano")
        {
            HabilitarBooleanos(0);
            buttonHideDemonio.RestablecerBotones();
            buttonHideBestia.RestablecerBotones();
            buttonHideExtraterrestre.RestablecerBotones();
            buttonHideHibrido.RestablecerBotones();
            buttonHideSuperHumano.RestablecerBotones();

            buttonHideHumano.EsconderBotones();
            EsconderBotones(partes[1]);
        }
        else if (partes[0] == "Superhumano")
        {
            HabilitarBooleanos(1);
            buttonHideDemonio.RestablecerBotones();
            buttonHideBestia.RestablecerBotones();
            buttonHideExtraterrestre.RestablecerBotones();
            buttonHideHibrido.RestablecerBotones();
            buttonHideHumano.RestablecerBotones();

            buttonHideSuperHumano.EsconderBotones();
            EsconderBotones(partes[1]);
        }
        else if (partes[0] == "Hibrido")
        {
            HabilitarBooleanos(3);
            buttonHideDemonio.RestablecerBotones();
            buttonHideBestia.RestablecerBotones();
            buttonHideExtraterrestre.RestablecerBotones();
            buttonHideHumano.RestablecerBotones();
            buttonHideSuperHumano.RestablecerBotones();

            buttonHideHibrido.EsconderBotones();
            EsconderBotones(partes[1]);
        }
        else if (partes[0] == "Extraterrestre")
        {
            HabilitarBooleanos(4);
            buttonHideDemonio.RestablecerBotones();
            buttonHideBestia.RestablecerBotones();
            buttonHideHibrido.RestablecerBotones();
            buttonHideHumano.RestablecerBotones();
            buttonHideSuperHumano.RestablecerBotones();

            buttonHideExtraterrestre.EsconderBotones();
            EsconderBotones(partes[1]);
        }
        else if (partes[0] == "Bestia")
        {
            HabilitarBooleanos(5);
            buttonHideDemonio.RestablecerBotones();
            buttonHideHibrido.RestablecerBotones();
            buttonHideExtraterrestre.RestablecerBotones();
            buttonHideHumano.RestablecerBotones();
            buttonHideSuperHumano.RestablecerBotones();

            buttonHideBestia.EsconderBotones();
            EsconderBotones(partes[1]);
        }
    }    
}
