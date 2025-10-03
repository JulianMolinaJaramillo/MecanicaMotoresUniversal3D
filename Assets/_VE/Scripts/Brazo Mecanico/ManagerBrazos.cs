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

    public void AsignarTargetsAmbosBrazos(Transform nuevoTarget)
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

    public void AsignarTargetDerecho(Transform nuevoTarget)
    {
        brazosMecanicos[0].AsignarTarget(nuevoTarget);
        brazosMecanicos[1].RegresarAPosicionInicial();
    }

    public void AsignarTargetIzquierdo(Transform nuevoTarget)
    {
        brazosMecanicos[1].AsignarTarget(nuevoTarget);
        brazosMecanicos[0].RegresarAPosicionInicial();
    }

    public void DesactivarBrazos()
    {
        for (int i = 0; i < brazosMecanicos.Length; i++)
        {
            brazosMecanicos[i].gameObject.SetActive(false);
        }
    }

    public void ActivarBrazos()
    {
        for (int i = 0; i < brazosMecanicos.Length; i++)
        {
            brazosMecanicos[i].gameObject.SetActive(true);
        }
    }
}
