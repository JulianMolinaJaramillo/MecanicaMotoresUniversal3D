using UnityEngine;

public class FijarRotacionHijo : MonoBehaviour
{
    private Quaternion rotacionInicial;

    void Start()
    {
        rotacionInicial = transform.rotation;
    }

    void LateUpdate()
    {
        transform.rotation = rotacionInicial;
    }
}
