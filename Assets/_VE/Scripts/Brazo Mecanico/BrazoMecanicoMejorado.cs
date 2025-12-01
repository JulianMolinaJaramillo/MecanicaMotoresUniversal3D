using System.Collections;
using UnityEngine;

public class BrazoMecanicoMejorado : MonoBehaviour
{
    public Transform destino;
    public Transform objetivoMira;
    public Transform puntoAgarre;  // posición exacta donde quiero colocar el objeto tomado
    public bool puedoValidar;
    public MoverObjeto[] dedos;
    public RotacionObjeto rotacionObjeto;

    [SerializeField]
    Transform[] bones;
    private float[] bonesLengths;
    [SerializeField]
    private int solverInteractions = 5;
    [SerializeField]
    private Transform targetPositions;

    // Guardado de pose
    private Vector3[] posesInicialesPos;
    private Quaternion[] posesInicialesRot;

    // Objeto tomado
    private Transform objetoTomado = null;

    // Control IK ON/OFF
    private bool usarIK = true;

    [Header("Interpolación")]
    [SerializeField, Range(0.1f, 10f)]
    private float velocidad = 2f;

    [Header("Offset del Target")]
    [SerializeField, Range(0f, 5f)]
    private float offsetDistancia = 0.5f;

    [Header("Velocidad de retorno")]
    [SerializeField, Range(0.1f, 10f)]
    private float duracionRetorno = 2f;

    [Header("Idle Movement")]
    [SerializeField] private Transform[] puntosIdle;   // posiciones posibles
    [SerializeField, Range(0.1f, 10f)]
    private float velocidadIdle = 1.5f;

    [SerializeField, Range(0.2f, 5f)]
    private float tiempoEntreIdle = 2f;

    private Coroutine idleRoutine;
    private bool enIdle = false;

    void Start()
    {
        bonesLengths = new float[bones.Length];
        posesInicialesPos = new Vector3[bones.Length];
        posesInicialesRot = new Quaternion[bones.Length];

        for (int i = 0; i < bones.Length; i++)
        {
            if (i < bones.Length - 1)
                bonesLengths[i] = (bones[i + 1].position - bones[i].position).magnitude;
            else
                bonesLengths[i] = 0f;

            posesInicialesPos[i] = bones[i].position;
            posesInicialesRot[i] = bones[i].rotation;
        }

        IniciarIdle();     
    }

    void Update()
    {
        if (!usarIK) return;
        if (puedoValidar) return;
        if (targetPositions == null) return;

        SolveIK();
    }

    [ContextMenu("idle")]
    public void IniciarIdle()
    {
        if (puntosIdle == null || puntosIdle.Length == 0) return;
        if (idleRoutine != null) StopCoroutine(idleRoutine);

        enIdle = true;
        idleRoutine = StartCoroutine(RutinaIdle());
        StartCoroutine(MovimientoDedos());
        StartCoroutine(MovimientoMano());
    }

    private IEnumerator MovimientoDedos()
    {
        while (enIdle)
        {
            for (int i = 0; i < dedos.Length; i++)
            {
                dedos[i].IniciarDesplazamientoObjeto();
            }

            yield return new WaitForSeconds(4f);

            for (int i = 0; i < dedos.Length; i++)
            {
                dedos[i].RetornarPosicionOriginal();
            }

            yield return new WaitForSeconds(4f);

            yield return null;
        }
    }

    IEnumerator MovimientoMano()
    {
        while (enIdle)
        {
            rotacionObjeto.velocidadRotacion = 50;

            yield return new WaitForSeconds(4f);

            rotacionObjeto.velocidadRotacion = -50;

            yield return new WaitForSeconds(4f);

            yield return null;
        }
    }

    public void DetenerIdle()
    {
        enIdle = false;
        if (idleRoutine != null)
        {
            StopCoroutine(idleRoutine);
            idleRoutine = null;
        }
    }

    /// <summary>
    /// Mueve el brazo entre puntos idle aleatorios cuando no tiene tarea
    /// </summary>
    IEnumerator RutinaIdle()
    {
        usarIK = true;

        while (enIdle)
        {
            // elegir un punto aleatorio
            Transform elegido = puntosIdle[Random.Range(0, puntosIdle.Length)];

            // mover hasta el punto elegido usando IK
            targetPositions = elegido;

            // esperar a llegar
            while (Vector3.Distance(bones[bones.Length - 1].position, elegido.position) > offsetDistancia + 0.1f)
                yield return null;

            // pausa aleatoria
            yield return new WaitForSeconds(tiempoEntreIdle);
        }
    }

    /// <summary>
    /// Tomar objeto
    /// </summary>
    /// <param name="objetivo"> objeto a tomar</param>
    public void IniciarPickUp(Transform objetivo)
    {
        DetenerIdle();
        StopAllCoroutines();
        GuardarPoseActual();
        usarIK = true;
        StartCoroutine(RutinaPickUp(objetivo));
    }

    [ContextMenu("coger")]
    public void IniciarPickUp()
    {
        DetenerIdle();
        StopAllCoroutines();
        GuardarPoseActual();
        usarIK = true;
        StartCoroutine(RutinaPickUp(objetivoMira));
    }

    IEnumerator RutinaPickUp(Transform objetivo)
    {
        for (int i = 0; i < dedos.Length; i++)
        {
            dedos[i].RetornarPosicionOriginal();
        }

        targetPositions = objetivo;

        // Ir interpolando hasta estar cerca
        yield return StartCoroutine(IrHastaObjetivo(objetivo));

        // Tomar
        AgarrarObjeto(objetivo);

        // Volver al inicio
        yield return StartCoroutine(VolverAPoseInicial());
    }

    /// <summary>
    /// Colocar objeto
    /// </summary>
    /// <param name="puntoDestino"> Punto donde colocar </param>

    public void ColocarObjetoEn(Transform puntoDestino)
    {
        DetenerIdle();
        StopAllCoroutines();
        GuardarPoseActual();
        usarIK = true;
        StartCoroutine(RutinaPlace(puntoDestino));
    }

    /// <summary>
    /// Colocar objeto
    /// </summary>
    [ContextMenu("soltar")]
    public void ColocarObjetoEn()
    {
        DetenerIdle();
        StopAllCoroutines();
        GuardarPoseActual();
        usarIK = true;
        StartCoroutine(RutinaPlace(destino));
    }

    IEnumerator RutinaPlace(Transform puntoDestino)
    {
        // Ir al punto de dejar
        yield return StartCoroutine(IrHastaObjetivo(puntoDestino));

        // Soltar
        SoltarObjeto();

        // Regresar
        yield return StartCoroutine(VolverAPoseInicial());
    }


    /// <summary>
    /// MOVER HASTA OBJETIVO
    /// </summary>
    /// <param name="objetivo"> El punto objetivo donde dejar el objeto </param>
    /// <returns></returns>

    IEnumerator IrHastaObjetivo(Transform objetivo)
    {
        targetPositions = objetivo;

        while (Vector3.Distance(bones[bones.Length - 1].position, objetivo.position) >
               offsetDistancia + 0.05f)
        {
            yield return null;
        }
    }


    /// <summary>
    /// VOLVER A POSE INICIAL
    /// </summary>
    /// <returns></returns>

    IEnumerator VolverAPoseInicial()
    {
        puedoValidar = true; // detener IK visual

        float t = 0;
        float dur = duracionRetorno;

        while (t < 1)
        {
            t += Time.deltaTime / dur;

            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].position = Vector3.Lerp(bones[i].position, posesInicialesPos[i], t);
                bones[i].rotation = Quaternion.Slerp(bones[i].rotation, posesInicialesRot[i], t);
            }

            yield return null;
        }

        // Apagar IK y limpiar target
        usarIK = false;
        targetPositions = null;

        puedoValidar = false;
    }



    /// <summary>
    /// GUARDAR POSE
    /// </summary>
    void GuardarPoseActual()
    {
        for (int i = 0; i < bones.Length; i++)
        {
            posesInicialesPos[i] = bones[i].position;
            posesInicialesRot[i] = bones[i].rotation;
        }
    }

    /// <summary>
    /// TOMAR Y SOLTAR OBJETOS
    /// </summary>
    /// <param name="obj"> Objeto a tomar</param>

    void AgarrarObjeto(Transform obj)
    {
        ManagerDesplazamientoMotor.singleton.DesactivacionInicialMotores();

        objetoTomado = obj;

        // Hacer hijo del punto de agarre
        obj.SetParent(puntoAgarre);

        // Colocar exactamente en la posición especificada
        obj.localPosition = Vector3.zero;
        obj.localRotation = Quaternion.identity;
    }

    void SoltarObjeto()
    {
        if (objetoTomado != null)
        {
            // Guardamos referencia antes de liberar
            Transform obj = objetoTomado;

            // Lo soltamos
            obj.SetParent(null);

            // Posicionar exactamente donde está el target final (el último "destino" usado)
            // Esto asume que lo sueltas en "destino" o en el puntoDestino que envíes a la rutina
            if (destino != null)
            {
                obj.position = destino.position;
                obj.rotation = destino.rotation;
            }

            objetoTomado = null;

            ManagerDesplazamientoMotor.singleton.ActivacionPosteriorMotores();

            // Reiniciar Idle
            IniciarIdle();
        }
    }

    /// <summary>
    /// Solver IK
    /// </summary>

    public void SolveIK()
    {
        Vector3[] finalBonesPositions = new Vector3[bones.Length];

        for (int i = 0; i < bones.Length; i++)
            finalBonesPositions[i] = bones[i].position;

        for (int i = 0; i < solverInteractions; i++)
            finalBonesPositions = SolverForwardPositions(SolverInversePositions(finalBonesPositions));

        for (int i = 0; i < bones.Length; i++)
        {
            bones[i].position = Vector3.Lerp(
                bones[i].position,
                finalBonesPositions[i],
                Time.deltaTime * velocidad
            );
        }
    }

    Vector3[] SolverInversePositions(Vector3[] forwardPositions)
    {
        Vector3[] inversePositions = new Vector3[forwardPositions.Length];

        for (int i = forwardPositions.Length - 1; i >= 0; i--)
        {
            if (i == forwardPositions.Length - 1)
            {
                Vector3 dir = (targetPositions.position - forwardPositions[i]).normalized;
                inversePositions[i] = targetPositions.position - dir * offsetDistancia;
            }
            else
            {
                Vector3 posPrimaSiguiente = inversePositions[i + 1];
                Vector3 posBaseActual = forwardPositions[i];
                Vector3 direccion = (posBaseActual - posPrimaSiguiente).normalized;
                float longitud = bonesLengths[i];
                inversePositions[i] = posPrimaSiguiente + direccion * longitud;
            }
        }

        return inversePositions;
    }

    Vector3[] SolverForwardPositions(Vector3[] inversePositions)
    {
        Vector3[] forwardPositions = new Vector3[inversePositions.Length];

        for (int i = 0; i < inversePositions.Length; i++)
        {
            if (i == 0)
            {
                forwardPositions[i] = bones[0].position;
            }
            else
            {
                Vector3 posPrimaActual = inversePositions[i];
                Vector3 posAnterior = forwardPositions[i - 1];
                Vector3 direccion = (posPrimaActual - posAnterior).normalized;
                float longitud = bonesLengths[i - 1];
                forwardPositions[i] = posAnterior + direccion * longitud;
            }
        }

        return forwardPositions;
    }
}
