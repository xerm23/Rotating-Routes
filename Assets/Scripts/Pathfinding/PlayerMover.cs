using Codice.CM.Common;
using RotatingRoutes.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RotatingRoutes.Pathfinding
{
    public class PlayerMover : MonoBehaviour
    {
        public Vector3 LeftStartPosition;
        public Vector3 RightStartPosition;

        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private LayerMask _hexLayer;


        private List<Vector3> _wayPoints = new(10);
        private int _currentWaypointId = 0;
        private List<ConnectPoint> _currentConnectPoints = new();

        private readonly RaycastHit[] _raycastHitsForConnectCheck = new RaycastHit[5];

        private bool _levelCompleted;
        private readonly float _directionLerpSpeed = 5f;
        private void Awake()
        {
            GameManager.OnGameStarted += StartMoving;
        }

        private void StartMoving(StartSide startSide)
        {
            if (startSide == StartSide.Left)
                SetLeftAsStart();

            else SetRightAsStart();
        }

        private void Update()
        {
            MoveCharacter();
        }

        private void MoveCharacter()
        {
            if (_wayPoints.Count == 0)
                return;
            float step = _moveSpeed * Time.deltaTime;
            float waypointThreshold = .1f;

            transform.position = Vector3.MoveTowards(transform.position, _wayPoints[_currentWaypointId], step);

            Vector3 relativePos = (_wayPoints[_currentWaypointId] - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, relativePos, _directionLerpSpeed* Time.deltaTime);

            //Quaternion toRotation = Quaternion.LookRotation(relativePos);
            //transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, _directionLerpSpeed * Time.time);

            if (Vector3.Distance(transform.position, _wayPoints[_currentWaypointId]) < waypointThreshold) // reached waypoint
            {
                if (++_currentWaypointId >= _wayPoints.Count) //check movement for another tile
                    CheckNextTileConnection();
            }
        }

        [ContextMenu("GO NEXT")]
        private void CheckNextTileConnection()
        {
            _wayPoints.Clear();
            Array.Clear(_raycastHitsForConnectCheck, 0, _raycastHitsForConnectCheck.Length);
            Physics.SphereCastNonAlloc(transform.position, .15f, transform.forward, _raycastHitsForConnectCheck, .2f, _hexLayer);
            CheckFinishLine();
            SetNewWaypoints(ConnectPointsFromRaycast());
        }

        private List<ConnectPoint> ConnectPointsFromRaycast()
        {
            List<ConnectPoint> connectPoints = new();
            foreach (var raycastHit in _raycastHitsForConnectCheck)
            {
                if (raycastHit.transform != null && raycastHit.transform.TryGetComponent<ConnectPoint>(out var connectPoint))
                    connectPoints.Add(connectPoint);
            }
            return connectPoints;
        }

        private void CheckFinishLine()
        {
            foreach (var raycastHit in _raycastHitsForConnectCheck)
            {
                if (raycastHit.transform != null && raycastHit.transform.TryGetComponent<FinishLine>(out var finishline))
                {
                    _levelCompleted = true;
                    finishline.InvokeGameFinished(transform);
                }
            }
        }

        private bool SetNewWaypoints(List<ConnectPoint> points)
        {
            if (_levelCompleted)
                return false;

            foreach (var connectPoint in points)
            {
                if (_currentConnectPoints.Contains(connectPoint))
                    continue;
                _currentConnectPoints.FirstOrDefault()?.SetParentHexTileStatus(true);
                connectPoint.SetParentHexTileStatus(false);

                _currentConnectPoints = new()
                    {
                        connectPoint,
                        connectPoint.ComboConnectPoint
                    };

                _wayPoints = connectPoint.PathWaypoints;
                _currentWaypointId = 0;

                return true; // Reached to a next walkable tile in correct pos
            }
            GameManager.GameOver();
            return false;
        }


        public void SetStartWaypoint(Vector3 startWaypoint)
        {
            _wayPoints.Add(startWaypoint);
        }

        [ContextMenu("SetLeftStart")]
        public void SetLeftAsStart() => SetStartWaypoint(LeftStartPosition);

        [ContextMenu("SetRightStart")]
        public void SetRightAsStart() => SetStartWaypoint(RightStartPosition);
    }
}