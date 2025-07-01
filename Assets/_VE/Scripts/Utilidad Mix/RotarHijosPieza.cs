using System.Collections.Generic;
using UnityEngine;

public class RotarHijosPieza : MonoBehaviour
{
    public float velocidadRotacion = 50f;
    public bool rotarEnZ, rotarEnY, rotarEnX;
    public float velocidadRetorno = 2f;

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
            transform.rotation = Quaternion.Lerp(transform.rotation, rotacionInicial, Time.deltaTime * velocidadRetorno);

            if (Quaternion.Angle(transform.rotation, rotacionInicial) < 0.1f)
            {
                transform.rotation = rotacionInicial;
                regresandoARotacionOriginal = false;
                rotando = false;

                ReparentarHijosNoColocados(); // ← ✅ Aquí vuelven todos
            }

            return;
        }

        if (rotando)
        {
            Vector3 rotacion = Vector3.zero;
            if (rotarEnX) rotacion.x = velocidadRotacion * Time.deltaTime;
            if (rotarEnY) rotacion.y = velocidadRotacion * Time.deltaTime;
            if (rotarEnZ) rotacion.z = velocidadRotacion * Time.deltaTime;

            transform.Rotate(rotacion);
        }
    }

    void LateUpdate()
    {
        // Solo validar hijos cuando está rotando o regresando
        if (!rotando && !regresandoARotacionOriginal) return;

        foreach (Transform hijo in transform)
        {
            if (hijosNoColocados.Contains(hijo)) continue;

            MoverPieza mover = hijo.GetComponent<MoverPieza>();
            if (mover != null && !mover.piezaColocada)
            {
                hijo.SetParent(null);
                hijosNoColocados.Add(hijo); // ← ✅ Esto es lo que garantiza el reparentado
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
