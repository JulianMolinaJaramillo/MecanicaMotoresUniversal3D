using UnityEngine;

public class CopiarTransform : MonoBehaviour
{
    [Header("Objeto a seguir")]
    public GameObject objetoReferencia;

    void Update()
    {
        if (objetoReferencia != null)
        {          
            transform.rotation = objetoReferencia.transform.rotation;
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        }
    }
}
