using RotatingRoutes.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RotatingRoutes.Particles
{

    public class OnDeathParticle : MonoBehaviour
    {
        [SerializeField] GameObject _deathParticlesPrefab;
        [SerializeField] GameObject _sphere;

        void Start()
        {
            GameManager.OnGameStateChange += PlayDeathParticles;
        }

        private void PlayDeathParticles(GameState gameState)
        {
            if (gameState != GameState.GameOver)
                return;
            Instantiate(_deathParticlesPrefab, transform);
            _sphere.SetActive(false);
        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChange -= PlayDeathParticles;

        }
    }
}
