using RotatingRoutes.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RotatingRoutes.UI
{
    public class SoundButton : MonoBehaviour
    {
        [SerializeField] Sprite _soundOn;
        [SerializeField] Sprite _soundOff;

        [SerializeField] private Image _image;
        private void Start()
        {
            SetSoundButtonStatus();
            GetComponent<Button>().onClick.AddListener(ChangeSoundStatus);
        }

        private void ChangeSoundStatus()
        {
            AudioManager.Instance.SetSoundStatus(!AudioManager.SoundTurnedOn);
            SetSoundButtonStatus();
        }

        private void SetSoundButtonStatus()
        {
            _image.sprite = AudioManager.SoundTurnedOn ? _soundOn : _soundOff;
        }
    }
}