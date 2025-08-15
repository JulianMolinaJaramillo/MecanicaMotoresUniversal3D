using UnityEngine;

public class ExplosionPiston : MonoBehaviour
{
    public ParticleSystem explosion;
    public ControlVelocidadAnimacion controlVelocidadAnimacion;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Piston"))
        {
            if (controlVelocidadAnimacion.sliderVelocidad.value > 0.4)
            {
                if (!explosion.isPlaying)
                {
                    explosion.Play();
                }
            }         
        }
    }
}
