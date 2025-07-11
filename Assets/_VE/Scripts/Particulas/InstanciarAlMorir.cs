using UnityEngine;

public class InstanciarAlMorir : MonoBehaviour
{
    public GameObject prefabAInstanciar;

    private ParticleSystem sistema;
    private ParticleSystem.Particle[] particulas;
    private float[] vidasPasadas;
    private float[] umbralPorParticula;

    private float[] valoresUmbral = new float[] { 0.1f, 0.2f, 0.3f };

    void Start()
    {
        sistema = GetComponent<ParticleSystem>();

        int maxParticulas = sistema.main.maxParticles > 0 ? sistema.main.maxParticles : 100;

        particulas = new ParticleSystem.Particle[maxParticulas];
        vidasPasadas = new float[maxParticulas];
        umbralPorParticula = new float[maxParticulas];
    }

    void Update()
    {
        int cantidad = sistema.GetParticles(particulas);

        for (int i = 0; i < cantidad; i++)
        {
            float tiempoRestante = particulas[i].remainingLifetime;

            // Si es una nueva partícula, le asignamos un umbral aleatorio
            if (vidasPasadas[i] == 0)
            {
                umbralPorParticula[i] = valoresUmbral[Random.Range(0, valoresUmbral.Length)];
            }

            float umbralActual = umbralPorParticula[i];

            if (vidasPasadas[i] > umbralActual && tiempoRestante <= umbralActual)
            {
                Vector3 posicionMundo = sistema.transform.TransformPoint(particulas[i].position);
                GameObject instancia = Instantiate(prefabAInstanciar, posicionMundo, Quaternion.identity);

                // 🔥 Tiempo de destrucción aleatorio entre 0.5 y 1.5 segundos
                float tiempoDestruccion = Random.Range(1f, 1.5f);
                Destroy(instancia, tiempoDestruccion);
            }

            vidasPasadas[i] = tiempoRestante;
        }
    }
}
