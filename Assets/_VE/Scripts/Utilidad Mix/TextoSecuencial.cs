using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextoSecuencial : MonoBehaviour
{
    public TextMeshProUGUI textoUI;     // Asignar desde el inspector
    public float velocidadTexto = 0.05f;     // Velocidad entre letras
    public float tiempoEspera = 1f; // Tiempo de espera entre repeticiones

    private string textoCompleto;
    private Coroutine coroutinaActual;

    /// <summary>
    /// Metodo invocado al objeto activarse en scena
    /// </summary>
    private void OnEnable()
    {
        textoCompleto = textoUI.text; // Tomamos el texto
        coroutinaActual = StartCoroutine(MostrarTexto());
    }

    /// <summary>
    /// Metodo invocado al objeto desactivarse en scena
    /// </summary>
    private void OnDisable()
    {
         StopCoroutine(coroutinaActual);
         textoUI.text = textoCompleto; // Reasignamos el texto
    }

    /// <summary>
    /// Currutina encangada de mostrar el texto letra por letra
    /// </summary>
    /// <returns></returns>
    public IEnumerator MostrarTexto()
    {
        textoUI.text = "";

        for (int i = 0; i < textoCompleto.Length; i++)
        {
            textoUI.text += textoCompleto[i];
            yield return new WaitForSeconds(velocidadTexto);
        }

        yield return new WaitForSeconds(tiempoEspera);
        ReproducirDeNuevo();
    }

    /// <summary>
    /// Para reiniciar la aparicion de letras
    /// </summary>
    public void ReproducirDeNuevo()
    {
        if (coroutinaActual != null) StopCoroutine(coroutinaActual);

        coroutinaActual = StartCoroutine(MostrarTexto());
    }
}
