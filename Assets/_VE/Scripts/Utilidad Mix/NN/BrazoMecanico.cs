using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BrazoMecanico : MonoBehaviour
{
    [SerializeField]
    Transform[] bones;
    float[] bonesLengths;

    [SerializeField]
    int solverInteractions = 5;


    [SerializeField]
    Transform targetPositions;

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

    // Update is called once per frame
    void Update()
    {
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
            bones[i].position = finalBonesPositions[i];

            //bones[i].eulerAngles = new Vector3(CalcularRotacionX(), 0, CalcularRotaciones());
            //bones[i].GetChild(0).transform.localEulerAngles = new Vector3(0, CalcularRotacionX(bones[i], (i == bones.Length - 1) ? targetPositions : bones[i + 1]) + 90 + 180, 0);
        }


    }


    Vector3[] SolverInversePositions(Vector3[] forwardPositions)
    {
        Vector3[] inversePositions = new Vector3[forwardPositions.Length];

        for (int i = (forwardPositions.Length - 1); i >= 0; i--)
        {
            if (i == forwardPositions.Length - 1)
            {
                inversePositions[i] = targetPositions.position;
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
}
