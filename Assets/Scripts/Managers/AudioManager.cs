using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RotatingRoutes.Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        [SerializeField] private AudioSource _musicSource;

        [SerializeField] private AudioClip popSound;
        [SerializeField] private AudioClip crackSound;
        AudioSource _audioSource;
        public static bool SoundTurnedOn => Convert.ToBoolean(PlayerPrefs.GetInt("SoundOn", 1));
        public void SetSoundStatus(bool status)
        {
            int volume = Convert.ToInt16(status);
            PlayerPrefs.SetInt("SoundOn", volume);
            _musicSource.volume = volume;
        }


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
            _musicSource.volume = Convert.ToInt16(SoundTurnedOn);
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayPopSound()
        {
            PlayClip(popSound);
        }


        private void PlayClip(AudioClip clip)
        {
            if (!SoundTurnedOn)
                return;
            _audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
            _audioSource.PlayOneShot(clip);
        }

    }
}