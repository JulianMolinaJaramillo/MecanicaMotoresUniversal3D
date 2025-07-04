using System.Collections.Generic;
using UnityEngine;

public class RotarHijosPieza : MonoBehaviour
{
    public float velocidadRotacion = 50f;
    public bool rotarEnZ, rotarEnY, rotarEnX;
    public float velocidadRetorno = 2f;
    public GameObject btnRotar;

    private Quaternion rotacionInicial;
    private bool regresandoARotacionOriginal = false;
    private bool rotando = false;

    private List<Transform> hijosNoColocados = new List<Transform>();

    void Start()
    {
        rotacionInicial = transform.rotation;
    }

    void Update()
    {
        if (regresandoARotacionOriginal)
        {
            // Verificar si se agregó algún nuevo hijo no colocado durante el retorno
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform hijo = transform.GetChild(i);
                if (hijosNoColocados.Contains(hijo)) continue;

                MoverPieza mover = hijo.GetComponent<MoverPieza>();
                if (mover != null && !mover.piezaColocada)
                {
                    Vector3 posOriginal = hijo.position;
                    Quaternion rotOriginal = hijo.rotation;

                    hijo.SetParent(null);
                    hijo.position = posOriginal;
                    hijo.rotation = rotOriginal;

                    // Opcional: corregir eje Y si aún ves rotaciones indeseadas
                    //Vector3 rot = hijo.eulerAngles;
                    //rot.y = 0f;
                    //hijo.eulerAngles = rot;

                    hijosNoColocados.Add(hijo);
                }
            }

            // Retornar rotación suavemente al estado original
            transform.rotation = Quaternion.Lerp(transform.rotation, rotacionInicial, Time.deltaTime * velocidadRetorno);

            // Finalizar el retorno si estamos lo suficientemente cerca
            if (Quaternion.Angle(transform.rotation, rotacionInicial) < 0.1f)
            {
                transform.rotation = rotacionInicial;
                regresandoARotacionOriginal = false;
                rotando = false;
                btnRotar.SetActive(true);
                ReparentarHijosNoColocados(); // ← Reemparentar los hijos que se habían separado
            }

            return; // Importante: salir para no rotar mientras regresa
        }

        if (rotando)
        {
            // Separar los hijos no colocados justo antes de aplicar la rotación
            SepararHijosNoColocados();

            Vector3 rotacion = Vector3.zero;
            if (rotarEnX) rotacion.x = velocidadRotacion * Time.deltaTime;
            if (rotarEnY) rotacion.y = velocidadRotacion * Time.deltaTime;
            if (rotarEnZ) rotacion.z = velocidadRotacion * Time.deltaTime;

            transform.Rotate(rotacion);
        }
    }

    private void SepararHijosNoColocados()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform hijo = transform.GetChild(i);
            if (hijosNoColocados.Contains(hijo)) continue;

            MoverPieza mover = hijo.GetComponent<MoverPieza>();
            if (mover != null && !mover.piezaColocada)
            {
                Vector3 posOriginal = hijo.position;
                Quaternion rotOriginal = hijo.rotation;

                hijo.SetParent(null);
                hijo.position = posOriginal;
                hijo.rotation = rotOriginal;

                // Opcional: corregir eje Y
                //Vector3 rot = hijo.eulerAngles;
                //rot.y = 0f;
                //hijo.eulerAngles = rot;

                hijosNoColocados.Add(hijo);
            }
        }
    }

    private void ReparentarHijosNoColocados()
    {
        foreach (Transform hijo in hijosNoColocados)
        {
            if (hijo != null)
            {
                hijo.SetParent(transform);
            }
        }

        hijosNoColocados.Clear();
    }

    public void RegresarARotacionOriginal()
    {
        rotarEnX = false;
        rotarEnY = false;
        rotarEnZ = false;

        regresandoARotacionOriginal = true;
    }

    public void RotarEnX()
    {
        rotando = true;

        rotarEnX = true;
        rotarEnY = false;
        rotarEnZ = false;
    }

    public void RotarEnY()
    {
        rotando = true;

        rotarEnX = false;
        rotarEnY = true;
        rotarEnZ = false;
    }

    public void RotarEnZ()
    {
        rotando = true;

        rotarEnX = false;
        rotarEnY = false;
        rotarEnZ = true;
    }

    public void RotarEnTodosLosEjes()
    {
        rotando = true;

        rotarEnX = true;
        rotarEnY = true;
        rotarEnZ = true;
    }
}
