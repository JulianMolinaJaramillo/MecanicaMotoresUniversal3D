using System.Collections.Generic;
using UnityEngine;

public class ColisionParticulas : MonoBehaviour
{
    public GameObject gotaPrefab;
    public float tiempoDeVida = 2f;

    private ParticleSystem sistemaParticulas;
    private List<ParticleCollisionEvent> eventosColision = new List<ParticleCollisionEvent>();

    void Start()
    {
        sistemaParticulas = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject other)
    {
        if (!other.CompareTag("Suelo"))
            return;

        int numEventos = sistemaParticulas.GetCollisionEvents(other, eventosColision);

        for (int i = 0; i < numEventos; i++)
        {
            Vector3 puntoColision = eventosColision[i].intersection;

            // Instanciar la gota justo donde chocó la partícula
            GameObject gota = Instantiate(gotaPrefab, puntoColision, Quaternion.identity);

            Destroy(gota, tiempoDeVida);
        }
    }
}
