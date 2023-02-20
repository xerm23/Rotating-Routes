using RotatingRoutes.Pathfinding;
using RotatingRoutes.Util.Extensions;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace RotatingRoutes.Hex
{
    public enum HexTileType
    {
        WalkableStraight,
        WalkableNarrowCurve,
        WalkableWideCurve,
        Blocker,
        Transition,
        Water,
        HillTransition,
        Finale,
        Start
    }

    public class HexTile : MonoBehaviour
    {
        [SerializeField] HexTileSOGroups[] _hexTileGroups;
        [SerializeField] MeshFilter _meshFilter;
        [SerializeField] MeshRenderer _meshRenderer;

        [field: SerializeField] public bool UsableStatus { get; private set; }
        [field: SerializeField] public HexTileStatus HexTileStatus { get; private set; }

        private GameObject _instantiatedPrefab;

        private bool _generatedWaypoints;

        public void SetHexStatus(HexTileStatus hexStatus) => HexTileStatus = hexStatus;

        private bool _walkPathSetup = false;
        public override string ToString()
        {
            return $"{HexTileStatus.Type} {HexTileStatus.ModelId}";
        }
        public void SetupTile()
        {
            UsableStatus = true;
            gameObject.SetActive(true);
            var tileGroup = _hexTileGroups.First(x => x.TileType == HexTileStatus.Type);
            var model = tileGroup.Models[HexTileStatus.ModelId];
            _meshFilter.mesh = model.GetComponent<MeshFilter>().sharedMesh;
            _meshRenderer.materials = model.GetComponent<MeshRenderer>().sharedMaterials;
            _meshFilter.transform.localScale = model.transform.localScale;

            if (tileGroup.BasePrefab != null && _instantiatedPrefab == null)
                _instantiatedPrefab = Instantiate(tileGroup.BasePrefab, transform);

            UsableStatus = HexTileStatus.Type <= HexTileType.WalkableWideCurve;
            SetWalkablePath();
        }


        private void SetWalkablePath()
        {
            if (HexTileStatus.Type > HexTileType.WalkableWideCurve && !_walkPathSetup)
                return;
            _walkPathSetup = true;
            LineRenderer linePath = GetComponentInChildren<LineRenderer>(true);
            Transform wayPointsContainer = transform.GetChild(1).Find("WaypointsContainer");
            GenerateWaypointsReference(linePath, wayPointsContainer);
        }
        private void GenerateWaypointsReference(LineRenderer linePath, Transform container)
        {
            if (_generatedWaypoints)
                return;
            _generatedWaypoints = true;
            Debug.Log("GENERATE WAYPOINTS");
            container.DestroyChildren();
            for (int i = 0; i < linePath.positionCount; i++)
            {
                Vector3 position = linePath.GetPosition(i);
                GameObject wayPoint = new($"Waypoint_{i}");
                wayPoint.transform.SetParent(container);
                wayPoint.transform.localPosition = position;
            }
        }
        public void SetUsableStatus(bool status) => UsableStatus = status;

        public void SetTileAsLeftStarting(PlayerMover playerMover)
        {
            playerMover.LeftStartPosition = GetComponentsInChildren<ConnectPoint>().OrderBy(go => (playerMover.transform.position - go.transform.position).sqrMagnitude).First().transform.position;
            SetUsableStatus(false);
        }

        public void SetTileAsRightStarting(PlayerMover playerMover)
        {
            playerMover.RightStartPosition = GetComponentsInChildren<ConnectPoint>().OrderBy(go => (playerMover.transform.position - go.transform.position).sqrMagnitude).First().transform.position;
            SetUsableStatus(false);
        }
    }
}