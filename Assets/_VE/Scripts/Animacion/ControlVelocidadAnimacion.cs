using UnityEngine;
using UnityEngine.UI;

public class ControlVelocidadAnimacion : MonoBehaviour
{
    [Header("Parámetros de velocidad")]
    public Slider sliderVelocidad;
    public float velocidadBase = 2f;   // Factor multiplicador

    [Header("Referencias")]
    public RotacionObjeto[] rotacionObjetos;
    public SnapAnimacion[] snaps;
    public Transform[] snapsReestablecer;
    public Transform ejeBarillas;   
    public Animator animator;

    //Referencias privadas
    [HideInInspector]
    public bool puedoValidar;
    private Vector3[] posicionesSnaps;
    private Quaternion rotacionEje;

    private void Start()
    {
        AsignarVelocidades();
        posicionesSnaps = new Vector3[snapsReestablecer.Length]; // Nos aseguramos de que tengan el mismo tamaño

        //Asignamos las posiciones originales de los puntos de contacto
        for (int i = 0; i < snapsReestablecer.Length; i++)
        {
            posicionesSnaps[i] = snapsReestablecer[i].position;
        }
        rotacionEje = ejeBarillas.rotation; // guardamos la rotacion original del eje de barillas
    }

    private void Update()
    {
        if (puedoValidar)
        {
            AsignarVelocidades();
            if (sliderVelocidad.value == 0f)
            {
                ReestablecerSnapResortes();
            }
        }         
    }

    public void AsignarVelocidades()
    {
        if (animator != null)
        {
            animator.speed = sliderVelocidad.value * velocidadBase;
        }

        for (int i = 0; i < rotacionObjetos.Length; i++)
        {
            rotacionObjetos[i].velocidadRotacion = sliderVelocidad.value * -1000;
        }

        if (sliderVelocidad.value < 0.3f)
        {
            for (int i = 0; i < snaps.Length; i++)
            {
                snaps[i].ajuste = -0.01f;
            }
        }
        else
        {
            for (int i = 0; i < snaps.Length; i++)
            {
                snaps[i].ajuste = -0.03f;
            }
        }
    }

    public void ReestablecerSnapResortes()
    {
        //Asignamos las posiciones originales de los puntos de contacto
        for (int i = 0; i < snapsReestablecer.Length; i++)
        {
            snapsReestablecer[i].position = posicionesSnaps[i];
        }
        // Asignamos la rotacion original al eje de barillas
        ejeBarillas.rotation = rotacionEje;
    }
}
