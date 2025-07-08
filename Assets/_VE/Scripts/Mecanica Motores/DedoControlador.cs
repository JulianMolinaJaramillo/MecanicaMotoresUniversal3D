using UnityEngine;

public class DedoControlador : MonoBehaviour
{
    public Transform targetPrincipal; // el que sigue al cursor
    public Transform palma; // Bone.004 o Bone.005, según el diseño

    public Transform[] targetsDedos; // target_dedo1, 2 y 3

    public float distanciaCierre = 0.2f; // qué tanto se cierran los dedos
    public float velocidad = 5f;

    void Update()
    {
        for (int i = 0; i < targetsDedos.Length; i++)
        {
            // Calcular dirección desde cada dedo hacia el target principal
            Vector3 direccion = (targetPrincipal.position - palma.position).normalized;

            // Offset desde la palma hacia adelante
            Vector3 nuevaPos = palma.position + direccion * distanciaCierre;

            // Movimiento suave del target del dedo hacia la posición
            targetsDedos[i].position = Vector3.Lerp(targetsDedos[i].position, nuevaPos, Time.deltaTime * velocidad);
        }
    }
}
