using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RotatingRoutes.Pathfinding
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private LayerMask _hexLayer;


        [SerializeField] private List<Vector3> _wayPoints = new(10);
        private int _currentWaypointId = 0;
        [SerializeField] private List<ConnectPoint> _currentConnectPoints = new();

        private RaycastHit[] _raycastHitsForConnectCheck = new RaycastHit[5];

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

            if (Vector3.Distance(transform.position, _wayPoints[_currentWaypointId]) < waypointThreshold) // reached waypoint
            {
                if (++_currentWaypointId >= _wayPoints.Count) //check movement for another tile
                    CheckNextTileConnection();
            }

        }

        [ContextMenu("Check Next Tile")]
        private void CheckNextTileConnection()
        {
            Debug.Log("Checking NEXT TILE!!");
            _wayPoints.Clear();
            Array.Clear(_raycastHitsForConnectCheck, 0, _raycastHitsForConnectCheck.Length);
            Physics.SphereCastNonAlloc(transform.position, .15f, transform.forward, _raycastHitsForConnectCheck, .2f, _hexLayer);
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

        private bool SetNewWaypoints(List<ConnectPoint> points)
        {
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
            Debug.Log("GAME OVER!!");
            return false;
        }
    }
}