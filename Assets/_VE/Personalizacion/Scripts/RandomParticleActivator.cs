using UnityEngine;

[DisallowMultipleComponent]
public class RandomParticleActivator : MonoBehaviour
{
    public ParticleSystem target;
    public float minTime = 1f;
    public float maxTime = 3f;

    void Start()
    {
        if (target == null) target = GetComponent<ParticleSystem>();
        StartCoroutine(Loop());
    }

    System.Collections.IEnumerator Loop()
    {
        while (true)
        {
            float wait = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(wait);
            target.Play();
        }
    }
}