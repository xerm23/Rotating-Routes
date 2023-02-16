using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RotatingRoutes.CameraControl
{

    public class CameraController : MonoBehaviour
    {
        [SerializeField] Transform _player;
        private float _lerpSpeed = 5;
        private Vector3 _heightOffset = new(0, 14, 0);

        private void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, _player.position + _heightOffset, _lerpSpeed * Time.deltaTime);
        }
    }
}