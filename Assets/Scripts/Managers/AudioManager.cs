using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RotatingRoutes.Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [SerializeField] private AudioClip popSound;
        [SerializeField] private AudioClip crackSound;
        AudioSource _audioSource;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                GameManager.OnGameStateChange += PlayCrackSound;
            }
            else Destroy(gameObject);
        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChange -= PlayCrackSound;
        }


        private void PlayCrackSound(GameState gameState)
        {
            if (gameState != GameState.GameOver)
                return;
            PlayClip(crackSound);
        }

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayPopSound()
        {
            PlayClip(popSound);
        }


        private void PlayClip(AudioClip clip)
        {
            _audioSource.pitch = Random.Range(0.8f, 1.2f);
            _audioSource.PlayOneShot(clip);
        }

    }
}