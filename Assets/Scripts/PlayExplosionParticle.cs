using UnityEngine;

public class PlayExplosionParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem _ps;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (_ps != null)
            {
                _ps.Play();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_ps != null)
            {
                _ps.Play();
            }
        }
    }
}
