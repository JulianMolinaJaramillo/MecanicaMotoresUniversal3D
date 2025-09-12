using UnityEngine;

public class ManagerBrazos : MonoBehaviour
{
    public BrazoMecanico[] brazosMecanicos;


    public static ManagerBrazos singleton;
    private void Awake()
    {
        // Configurar Singleton
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        RetornarBrazos();
    }

    public void AsignarTargets(Transform nuevoTarget)
    {
        for (int i = 0; i < brazosMecanicos.Length; i++)
        {
            brazosMecanicos[i].AsignarTarget(nuevoTarget);
        }
    }

    public void RetornarBrazos()
    {
        for (int i = 0; i < brazosMecanicos.Length; i++)
        {
            brazosMecanicos[i].RegresarAPosicionInicial();
        }
    }
}
