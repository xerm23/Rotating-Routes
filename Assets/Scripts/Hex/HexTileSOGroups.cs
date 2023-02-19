using UnityEngine;

namespace RotatingRoutes.Hex
{
    [CreateAssetMenu(fileName = nameof(HexTileSOGroups), menuName = "RotatingRoutes/" + nameof(HexTileSOGroups))]

    public class HexTileSOGroups : ScriptableObject
    {
        public HexTileType TileType;
        public GameObject BasePrefab;
        public GameObject[] Models;

        public GameObject GetModel(int modelId)
        {
            return Models[modelId];
        }

        public GameObject GeneratePrefab(Transform parent)
        {
            return Instantiate(BasePrefab, parent); ;
        }
    }
}