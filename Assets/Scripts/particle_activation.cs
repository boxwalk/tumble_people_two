using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particle_activation : MonoBehaviour
{
    private BoxCollider2D coll;
    private ParticleSystem particle;
    [SerializeField] private bool particle_active;
    [SerializeField] private LayerMask mask;
    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        particle = GetComponent<ParticleSystem>();
        particle.Play();
        particle_active = true;
    }
    void Update()
    {
        if (coll.IsTouchingLayers(mask))
        {
            if(particle_active == false)
            {
                particle.Play();
            }
            particle_active = true;
        }else
        {
            if (particle_active == true)
            {
                particle.Stop();
            }
            particle_active = false;
        }
    }
}
