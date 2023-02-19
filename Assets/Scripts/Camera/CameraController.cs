using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RotatingRoutes.CameraControl
{

    public class CameraController : MonoBehaviour
    {
        [SerializeField] Transform _player;
        [SerializeField] float _movementOffsetMultiplicator;
        private float _lerpSpeed = 4f;
        private Vector3 _initialOffset = new(0, 15, -6);
        private Vector3 _previousPlayerPosition;
        [SerializeField] private Vector3 _deltaDirection;
        private float _directionLerpSpeed = 1f;

        public static Action<int, int> OnCameraPositionChanged;

        private int _currentX, _currentZ;

        private void Awake()
        {
            _previousPlayerPosition = _player.position;
            _deltaDirection = _player.forward;
            transform.position = _player.position + _initialOffset;
            _currentX = (int)transform.position.x - 1;
            _currentZ = (int)transform.position.z - 1;
        }


        private void LateUpdate()
        {
            _deltaDirection = Vector3.Lerp(_deltaDirection, (_player.position - _previousPlayerPosition).normalized * _movementOffsetMultiplicator, _directionLerpSpeed * Time.deltaTime);
            _previousPlayerPosition = _player.position;
            Vector3 targetCameraPosition = _player.position + _initialOffset + _deltaDirection;
            targetCameraPosition.z = Mathf.Clamp(targetCameraPosition.z, _player.position.z + _initialOffset.z * 1.25f, _player.position.z + _initialOffset.z * .15f);
            targetCameraPosition.x = Mathf.Clamp(targetCameraPosition.x, _player.position.x - 2, _player.position.x + 2);

            transform.position = Vector3.Lerp(transform.position, targetCameraPosition, _lerpSpeed * Time.deltaTime);

            if ((int)transform.position.x != _currentX || (int)transform.position.z != _currentZ)
            {
                _currentX = (int)transform.position.x;
                _currentZ = (int)transform.position.z;
                OnCameraPositionChanged?.Invoke(_currentX, _currentZ);
            }
        }
    }
}