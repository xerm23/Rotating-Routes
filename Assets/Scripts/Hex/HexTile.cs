using RotatingRoutes.Util.Extensions;
using UnityEngine;

namespace RotatingRoutes.Hex
{
    public class HexTile : MonoBehaviour
    {
        [SerializeField] LineRenderer _linePath;
        [SerializeField] Transform _waypointsContainer;

        [SerializeField] Transform _decorationContainer;

        [field: SerializeField] public bool UsableStatus { get; private set; } = true;
        private void Awake()
        {
            GenerateWaypointsReference();
        }

        public void SetUsableStatus(bool status) => UsableStatus = status;


        private void OnEnable()
        {
            _decorationContainer.DisableChildren();
            _decorationContainer.GetChild(Random.Range(0, _decorationContainer.childCount)).gameObject.SetActive(true);
        }

        private void GenerateWaypointsReference()
        {
            for (int i = 0; i < _linePath.positionCount; i++)
            {
                Vector3 position = _linePath.GetPosition(i);
                GameObject wayPoint = new($"Waypoint_{i}");
                wayPoint.transform.SetParent(_waypointsContainer);
                wayPoint.transform.localPosition = position;
            }

        }

    }
}