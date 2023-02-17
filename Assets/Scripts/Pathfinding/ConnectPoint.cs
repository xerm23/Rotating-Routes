using RotatingRoutes.Hex;
using RotatingRoutes.Util.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RotatingRoutes.Pathfinding
{
    public class ConnectPoint : MonoBehaviour
    {
        [field: SerializeField] public ConnectPoint ComboConnectPoint { get; private set; }

        [SerializeField] Transform _waypointsContainer;
        [Tooltip("First or last index in line renderer")]
        [SerializeField] private bool _first;


        public List<Vector3> PathWaypoints => _waypointsContainer.GetComponentsInChildren<Transform>()
                                                      .Where(x => x != _waypointsContainer)
                                                      .Select(x => x.position)
                                                      .ReverseIf(!_first)
                                                      .ToList();
        private HexTile HexTile => _hexTile ??= GetComponentInParent<HexTile>();
        private HexTile _hexTile;


        public void SetParentHexTileStatus(bool status) => HexTile.SetUsableStatus(status);

    }



}