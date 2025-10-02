using System.Collections;
using UnityEngine;

public class ManagerVehiculos : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject[] vehiculos;
    public Transform[] posicionesBus;
    public Transform[] posicionesCarros;
    public Transform[] posicionesTaxis;
    public float tiempoSpawn;
    
    private Coroutine coroutine;
    private bool detener;

    private void Start()
    {
        SpawnVehiculo();
    }

    public void SpawnVehiculo()
    {
        if (!detener)
        {
            int numeroX = Random.Range(0, 5);

            // Instanciar un vehiculo aleatorio
            GameObject vehiculoInstanciado = Instantiate(vehiculos[numeroX], transform);

            MovimientoCarril movimientoCarril = vehiculoInstanciado.GetComponent<MovimientoCarril>();

            if (movimientoCarril.soyBus)
            {
                movimientoCarril.AsignarDestinos(posicionesBus);
            }
            else if (movimientoCarril.soyCarro)
            {
                movimientoCarril.AsignarDestinos(posicionesCarros);
            }
            else
            {
                movimientoCarril.AsignarDestinos(posicionesTaxis);
            }

            int tiempoRespawnAleatorio = Random.Range(2, 5);
            tiempoSpawn = Mathf.RoundToInt(tiempoRespawnAleatorio);

            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(Respawn());
        }    
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(tiempoSpawn);
        SpawnVehiculo();
    }

    [ContextMenu("detener")]
    public void DetenerSpawn()
    {
        detener = true;
    }

    [ContextMenu("detener")]
    public void ResstablecerSpawn()
    {
        detener = false;
    }

    private void OnEnable()
    {
        if (!detener)
        {
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(Respawn());
        }     
    }
}
