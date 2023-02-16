using RotatingRoutes.Pathfinding;
using RotatingRoutes.Util.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RotatingRoutes.Hex
{
    public class HexTile : MonoBehaviour
    {
        [SerializeField] LineRenderer _linePath;
        [SerializeField] Transform _waypointsContainer;

        [SerializeField] Transform _decorationContainer;
        private void Awake()
        {
            GenerateWaypoints();
        }


        private void OnEnable()
        {
            _decorationContainer.DisableChildren();
            _decorationContainer.GetChild(Random.Range(0, _decorationContainer.childCount)).gameObject.SetActive(true);
        }

        private void GenerateWaypoints()
        {
            for (int i = 0; i < _linePath.positionCount; i++)
            {
                Vector3 position = _linePath.GetPosition(i);
                GameObject wayPoint = new GameObject($"Waypoint_{i}");
                wayPoint.transform.SetParent(_waypointsContainer);
                wayPoint.transform.localPosition = position;
            }

        }

    }
}