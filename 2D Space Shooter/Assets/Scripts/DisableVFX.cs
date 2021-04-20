using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class DisableVFX : MonoBehaviour
{
    [SerializeField] private float despawnTime = 2f;
    [SerializeField] private List<ParticleSystem> particles = new List<ParticleSystem>();
    private void OnEnable()
    {
        particles?.ForEach(ps => ps?.Play());
        Invoke("Despawn", despawnTime);
    }

    private void Despawn() => gameObject.SetActive(false);
}
