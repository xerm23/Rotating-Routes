using RotatingRoutes.CameraControl;
using RotatingRoutes.Hex;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RotatingRoutes.Util.ObjectPooling
{
    public class HexPooler : MonoBehaviour
    {
        public HexTile GameObjectToPool;

        protected Transform _poolParent;
        protected List<HexTile> _pooledGameObjects = new();


        [SerializeField] HexTileSOGroups _tileSOGroup;
        private const int XCamOffset = 11;
        private const int ZCamOffset = 8;
        private HexGridGenerator _hexGridGenerator;

        private HexTileType Type => _tileSOGroup.TileType;
        private Dictionary<(int row, int column), HexTile> _spawnedTiles = new();

        private Dictionary<(int row, int column), HexTileStatus> TilesForThisPooler => _tilesForThisPooler ??= _hexGridGenerator.SpawnedHexTiles.Where(x => x.Value.Type == Type).ToDictionary(x => x.Key, x => x.Value);
        private Dictionary<(int row, int column), HexTileStatus> _tilesForThisPooler;
        private void Awake()
        {
            _poolParent = transform;
            _hexGridGenerator = GetComponentInParent<HexGridGenerator>();
            CameraController.OnCameraPositionChanged += ActivateCorrectElements;
        }
        private void OnDestroy() => CameraController.OnCameraPositionChanged -= ActivateCorrectElements;

        private void ActivateCorrectElements(int currentCameraX, int currentCameraZ)
        {
            foreach ((int i, int j) in TilesForThisPooler.Keys.ToList())
            {
                float xCoord = i % 2 != 0
                    ? HexGridGenerator.X_OFFSET * j + HexGridGenerator.X_OFFSET / 2
                    : HexGridGenerator.X_OFFSET * j;
                float zCoord = HexGridGenerator.Z_OFFSET * i;

                bool outsideCamFrame = currentCameraX - XCamOffset >= xCoord || xCoord >= currentCameraX + XCamOffset
                 || currentCameraZ - ZCamOffset / 2 >= zCoord || zCoord >= currentCameraZ + ZCamOffset * 2;

                if (_spawnedTiles.ContainsKey((i, j)) && outsideCamFrame)
                {
                    _spawnedTiles[(i, j)].gameObject.SetActive(false);
                    _spawnedTiles.Remove((i, j));
                }

                if (!_spawnedTiles.ContainsKey((i, j)) && !outsideCamFrame)
                {
                    var hexGameObject = GetGameObjectByRotationId(_hexGridGenerator.SpawnedHexTiles[(i, j)].RotationAngle, _hexGridGenerator.SpawnedHexTiles[(i, j)].ModelId);

                    _hexGridGenerator.SpawnedHexTiles[(i, j)] = hexGameObject.HexTileStatus;
                    hexGameObject.gameObject.name = $"Tile: ({i},{j})";
                    hexGameObject.transform.SetPositionAndRotation(new(xCoord, 0, zCoord), Quaternion.Euler(0, _hexGridGenerator.SpawnedHexTiles[(i, j)].RotationAngle, 0));
                    hexGameObject.SetupTile();
                    _spawnedTiles[(i, j)] = hexGameObject;
                    SetStartPositionTiles(i, j, hexGameObject);
                }
            }
        }

        private void SetStartPositionTiles(int i, int j, HexTile hexGameObject)
        {
            if (_hexGridGenerator.RowAmount != i)
                return;

            if (_hexGridGenerator.ColAmount / 2 == j)
                hexGameObject.SetTileAsLeftStarting(_hexGridGenerator.Player);

            if (1 + _hexGridGenerator.ColAmount / 2 == j)
                hexGameObject.SetTileAsRightStarting(_hexGridGenerator.Player);
        }

        public HexTile GetGameObjectByRotationId(float rotationAngle, int modelId)
        {
            var inactiveGO = _pooledGameObjects.FirstOrDefault(x => !x.gameObject.activeInHierarchy);

            if (modelId == -1)
                modelId = Random.Range(0, _tileSOGroup.Models.Length);
            if (inactiveGO != null)
            {
                inactiveGO.SetHexStatus(new(_tileSOGroup.TileType, rotationAngle, modelId));
                return inactiveGO;
            }

            GameObjectToPool.SetHexStatus(new(_tileSOGroup.TileType, rotationAngle, modelId));
            return AddOneObjectToThePool();
        }
        private HexTile AddOneObjectToThePool()
        {
            if (GameObjectToPool == null)
            {
                Debug.LogWarning("The " + gameObject.name + " ObjectPooler doesn't have any GameObjectToPool defined.", gameObject);
                return null;
            }
            var newGameObject = Instantiate(GameObjectToPool);
            newGameObject.transform.SetParent(_poolParent);
            newGameObject.name = GameObjectToPool.name + "-" + _pooledGameObjects.Count;
            _pooledGameObjects.Add(newGameObject);
            return newGameObject;
        }

    }
}