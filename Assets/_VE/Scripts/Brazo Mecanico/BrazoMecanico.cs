using UnityEngine;

public class BrazoMecanico : MonoBehaviour
{
    [SerializeField]
    Transform[] bones;
    float[] bonesLengths;

    [SerializeField]
    int solverInteractions = 5;
    public bool puedoValidar;

    [SerializeField]
    Transform targetPositions;
    public Transform targetPositionInicial;

    [Header("Interpolación")]
    [SerializeField, Range(0.1f, 10f)]
    float velocidad = 2f; // velocidad ajustable en inspector

    [Header("Offset del Target")]
    [SerializeField, Range(0f, 5f)]
    float offsetDistancia = 0.5f; // distancia mínima al target


    void Start()
    {
        bonesLengths = new float[bones.Length];

        for (int i = 0; i < bones.Length; i++)
        {
            if (i < bones.Length - 1)
            {
                bonesLengths[i] = (bones[i + 1].position - bones[i].position).magnitude;
            }
            else
            {
                bonesLengths[i] = 0f;
            }

        }
    }

    void Update()
    {
        if (puedoValidar || targetPositions == null) return;
        SolveIK();
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 1; i < bones.Length; i++)
        {
            Gizmos.DrawSphere(bones[i].transform.position, 0.1f);
            Gizmos.DrawLine(bones[i].transform.position, bones[i - 1].transform.position);
        }

        // Dibujamos el punto donde realmente se está "deteniendo" el brazo (offset)
        if (targetPositions != null && bones.Length > 0)
        {
            Vector3 dir = (targetPositions.position - bones[bones.Length - 1].position).normalized;
            Vector3 offsetPos = targetPositions.position - dir * offsetDistancia;

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(offsetPos, 0.12f);
        }
    }

    public void SolveIK()
    {
        Vector3[] finalBonesPositions = new Vector3[bones.Length];

        for (int i = 0; i < bones.Length; i++)
        {
            finalBonesPositions[i] = bones[i].position;
        }

        for (int i = 0; i < solverInteractions; i++)
        {
            finalBonesPositions = SolverForwardPositions(SolverInversePositions(finalBonesPositions));
        }

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

        for (int i = (forwardPositions.Length - 1); i >= 0; i--)
        {
            if (i == forwardPositions.Length - 1)
            {
                //Aplicamos el offset al target
                Vector3 dir = (targetPositions.position - forwardPositions[i]).normalized;
                inversePositions[i] = targetPositions.position - dir * offsetDistancia;
            }
            else
            {
                Vector3 posPrimaSiguiente = inversePositions[i + 1];
                Vector3 posBaseActual = forwardPositions[i];
                Vector3 direccion = (posBaseActual - posPrimaSiguiente).normalized;
                float longitud = bonesLengths[i];
                inversePositions[i] = posPrimaSiguiente + (direccion * longitud);
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
                Vector3 posPrimaSegundaAnterior = forwardPositions[i - 1];
                Vector3 direccion = (posPrimaActual - forwardPositions[i - 1]).normalized;
                float longitud = bonesLengths[i - 1];
                forwardPositions[i] = posPrimaSegundaAnterior + (direccion * longitud);
            }
        }

        return forwardPositions;
    }

    // Método para activar el retorno
    [ContextMenu("volver")]
    public void RegresarAPosicionInicial()
    {
        targetPositions = targetPositionInicial;
    }

    // Lógica de interpolación al volver a la posicion inicial
    public void AsignarTarget(Transform nuevotarget)
    {
        targetPositions = nuevotarget;
    }
}
