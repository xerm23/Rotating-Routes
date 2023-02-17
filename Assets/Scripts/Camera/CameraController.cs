using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RotatingRoutes.CameraControl
{

    public class CameraController : MonoBehaviour
    {
        [SerializeField] Transform _player;
        [SerializeField] float _movementOffsetMultiplicator;
        private float _lerpSpeed = 2f;
        private Vector3 _initialOffset = new(0, 15, -6);
        private Vector3 _previousPlayerPosition;
        [SerializeField] private Vector3 _deltaDirection;
        private float _directionLerpSpeed = 1f;

        private void Awake()
        {
            _previousPlayerPosition = _player.position;
            _deltaDirection = _player.forward;
            transform.position = _player.position + _initialOffset;
        }


        private void LateUpdate()
        {
            _deltaDirection = Vector3.Lerp(_deltaDirection, (_player.position - _previousPlayerPosition).normalized * _movementOffsetMultiplicator, _directionLerpSpeed * Time.deltaTime);
            _previousPlayerPosition = _player.position;
            Vector3 targetCameraPosition = _player.position + _initialOffset + _deltaDirection;
            targetCameraPosition.z = Mathf.Clamp(targetCameraPosition.z, _player.position.z + _initialOffset.z * 1.35f, _player.position.z - _initialOffset.z * 1.35f);
            targetCameraPosition.x = Mathf.Clamp(targetCameraPosition.x, _player.position.x - 2, _player.position.x + 2);

            transform.position = Vector3.Lerp(transform.position, targetCameraPosition, _lerpSpeed * Time.deltaTime);
        }
    }
}