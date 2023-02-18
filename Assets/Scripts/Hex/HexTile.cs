using RotatingRoutes.Util.ObjectPooling;
using System.Linq;
using UnityEngine;

namespace RotatingRoutes.Hex
{
    public enum HexTileType
    {
        WalkableStraight,
        WalkableNarrowCurve,
        WalkableWideCurve,
        Blocker,
        Transition,
        Hill
    }

    public class HexTile : Poolable<HexTile>
    {
        [SerializeField] HexTileSOGroups[] _hexTileGroups;

        [field: SerializeField] public bool UsableStatus { get; private set; } = true;
        [field: SerializeField] public HexTileType HexTileType { get; private set; }

        public void SetHexTileType(HexTileType hexTileType)
        {
            HexTileType = hexTileType;
            SetupTile();
        }
        public override void Reset()
        {
            gameObject.SetActive(false);
        }

        public void SetupTile()
        {
            _hexTileGroups.First(x => x.TileType == HexTileType).GenerateRandomPrefab(transform);
            UsableStatus = HexTileType <= HexTileType.WalkableWideCurve;
            SetWalkablePath();
        }

        private void SetWalkablePath()
        {
            if (HexTileType > HexTileType.WalkableWideCurve)
                return;
            LineRenderer linePath = GetComponentInChildren<LineRenderer>(true);
            Transform wayPointsContainer = transform.GetChild(0).Find("WaypointsContainer");
            GenerateWaypointsReference(linePath, wayPointsContainer);
        }
        private void GenerateWaypointsReference(LineRenderer linePath, Transform container)
        {
            for (int i = 0; i < linePath.positionCount; i++)
            {
                Vector3 position = linePath.GetPosition(i);
                GameObject wayPoint = new($"Waypoint_{i}");
                wayPoint.transform.SetParent(container);
                wayPoint.transform.localPosition = position;
            }
        }
        public void SetUsableStatus(bool status) => UsableStatus = status;
    }
}