using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : MonoBehaviour {

    private ParticleSystem thrusterParticle;

    private void Start()
    {
        thrusterParticle = GetComponent<ParticleSystem>();
    }

    public void Emit()
    {
        thrusterParticle.Emit(1);
    }
}
