using UnityEngine;

public class RotacionBrazo : MonoBehaviour
{
    public Transform[] referencias, graficos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < referencias.Length - 1; i++)
        {
            graficos[i].transform.position = referencias[i].transform.position;
            graficos[i].LookAt(referencias[i + 1]);
        }
    }
}
