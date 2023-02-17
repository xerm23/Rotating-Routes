using System.Linq;
using UnityEngine;

namespace RotatingRoutes.Hex
{
    [CreateAssetMenu(fileName = nameof(HexTileSOGroups), menuName = "RotatingRoutes/" + nameof(HexTileSOGroups))]

    public class HexTileSOGroups : ScriptableObject
    {
        public HexTileType TileType;
        public GameObject[] Prefabs;

        public void GenerateRandomPrefab(Transform parent)
        {
            Instantiate(Prefabs[Random.Range(0, Prefabs.Length)], parent);
        }
    }
}