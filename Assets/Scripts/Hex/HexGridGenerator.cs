using RotatingRoutes.Util.ObjectPooling;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RotatingRoutes.Hex
{
    [System.Serializable]
    public struct HexTileStatus
    {
        public HexTileType Type;
        public float RotationAngle;
        public int ModelId;
        public int ScaleY;

        public HexTileStatus(HexTileType type, float rotationAngle, int modelId = -1, int scaleY = 1)
        {
            Type = type;
            RotationAngle = rotationAngle;
            ModelId = modelId;
            ScaleY = scaleY;
        }

        public override string ToString()
        {
            return $"Status: {Type} angle: {RotationAngle} model: {ModelId} scale  {ScaleY}";
        }
    }

    public class HexGridGenerator : MonoBehaviour
    {
        [SerializeField] HexTile hexTilePrefab;

        [SerializeField] int rowAmount;
        [SerializeField] int colAmount;
        [SerializeField] GameObject[] hillTransition;
        [SerializeField] GameObject[] hillTiles;

        public const float X_OFFSET = 2f;
        public const float Z_OFFSET = 1.75f;

        public Dictionary<(int row, int column), HexTileStatus> SpawnedHexTiles { get; private set; } = new();

        private Camera _mainCam;

        private void Awake()
        {
            _mainCam = Camera.main;
            Generate();
        }
        [SerializeField] float blockerChance = 5;
        [SerializeField] float straightWalkableChance = 10;
        [SerializeField] float narrowCurveChance = 20;

        [SerializeField] int blockerMinimumRange = 2;

        int totalBlockers, totalWalkableStraight, totalNarrowCurve;


        [ContextMenu("Generate!")]
        private void Generate()
        {
            //foreach (Transform child in transform)
            //    Destroy(child.gameObject);
            SpawnedHexTiles.Clear();
            totalBlockers = 0;
            totalWalkableStraight = 0;
            totalNarrowCurve = 0;


            for (int i = 0; i < rowAmount; i++)
                for (int j = 0; j < colAmount; j++)
                {
                    HexTileType tileType;
                    //HexTile tile = HexTile.Pool.Get();
                    bool blockerCondition = Random.Range(0, 100) < blockerChance
                                            && !TileTypeInRange(i, j, HexTileType.Blocker, blockerMinimumRange)
                                            && i >= 1
                                            && i <= rowAmount - 1
                                            && j >= 1
                                            && j <= colAmount - 1;
                    if (blockerCondition)
                    {
                        totalBlockers++;
                        tileType = HexTileType.Blocker;
                        //tile.SetHexTileType(HexTileType.Blocker);
                    }
                    else tileType = SpawnTileAsWalkable(i, j);
                    float tileRotation = Random.Range(0, 6) * 60;

                    SpawnedHexTiles.Add((i, j), new(tileType, tileRotation));
                }


            Debug.Log($"Total blocker {totalBlockers}, Total walkable straight {totalWalkableStraight}, Total narrow {totalNarrowCurve} Total tiles {SpawnedHexTiles.Count}");
            //SpawnSideHills();
        }

        private HexTileType SpawnTileAsWalkable(int row, int col)
        {
            if (Random.Range(0, 100) < straightWalkableChance && !TileTypeInRange(row, col, HexTileType.WalkableStraight, 1))
            {
                totalWalkableStraight++;
                return HexTileType.WalkableStraight;
            }
            else if (Random.Range(0, 100) < narrowCurveChance)
            {
                totalNarrowCurve++;
                return HexTileType.WalkableNarrowCurve;
            }
            return HexTileType.WalkableWideCurve;
        }


        private bool TileTypeInRange(int row, int col, HexTileType hexType, int range)
        {
            var tiles = TilesInRange((row, col), range);
            return tiles.Any(x => x == hexType);
        }
        private List<HexTileType> TilesInRange((int x, int y) center, int range)
        {
            List<HexTileType> resultsTiles = new();

            for (int q = -range; q <= range; q++)
            {
                for (int r = Mathf.Max(-range, -range - q); r <= Mathf.Min(range, range - q); r++)
                {
                    if (q == 0 && r == 0)
                        continue;

                    var (row, col) = Offset((center.x & 1) == 0, q, r);
                    var tileCoord = (center.x + row, center.y + col);

                    if (SpawnedHexTiles.ContainsKey(tileCoord))
                    {
                        resultsTiles.Add(SpawnedHexTiles[tileCoord].Type);
                    }
                }
            }

            return resultsTiles;
        }

        private (int row, int col) Offset(bool oddCenter, int q, int r)
        {
            var col = oddCenter
                ? q + (r - (r & 1)) / 2
                : q + (r + (r & 1)) / 2;
            var row = r;

            return (row, col);
        }

        private void SpawnSideHills()
        {

            int hillColAmount = 4;

            //LEFT SIDE
            for (int i = -hillColAmount; i < 0; i++)
                for (int j = 0; j < colAmount; j++)
                {
                    if (i == -1)
                        Instantiate(hillTransition[Random.Range(0, hillTransition.Length)], new Vector3(j % 2 != 0 ? X_OFFSET * i + X_OFFSET / 2 : X_OFFSET * i, 0, Z_OFFSET * j), Quaternion.identity, transform).transform.Rotate(0, 180, 0);
                    else
                    {
                        Instantiate(hillTiles[Random.Range(0, hillTiles.Length)], new Vector3(j % 2 != 0 ? X_OFFSET * i + X_OFFSET / 2 : X_OFFSET * i, 0, Z_OFFSET * j), Quaternion.identity, transform).transform.localScale = new Vector3(1, Random.Range(1, 3), 1);
                    }
                }

            //RIGHT SIDE
            for (int i = rowAmount; i < rowAmount + hillColAmount; i++)
                for (int j = 0; j < colAmount; j++)
                {
                    if (i == rowAmount)
                        Instantiate(hillTransition[Random.Range(0, hillTransition.Length)], new Vector3(j % 2 != 0 ? X_OFFSET * i + X_OFFSET / 2 : X_OFFSET * i, 0, Z_OFFSET * j), Quaternion.identity, transform);
                    else
                    {
                        Instantiate(hillTiles[Random.Range(0, hillTiles.Length)], new Vector3(j % 2 != 0 ? X_OFFSET * i + X_OFFSET / 2 : X_OFFSET * i, 0, Z_OFFSET * j), Quaternion.identity, transform).transform.localScale = new Vector3(1, Random.Range(1, 3), 1);
                    }
                }

            //DOWN SIDE
            for (int j = -hillColAmount; j < 0; j++)
                for (int i = 0; i < rowAmount; i++)
                    if (j == -1)
                        Instantiate(hillTransition[Random.Range(0, hillTransition.Length)], new Vector3(j % 2 != 0 ? X_OFFSET * i + X_OFFSET / 2 : X_OFFSET * i, 0, Z_OFFSET * j), Quaternion.identity, transform).transform.Rotate(0, 60 + i % 2 * 60, 0);
                    else
                    {
                        Instantiate(hillTiles[Random.Range(0, hillTiles.Length)], new Vector3(j % 2 != 0 ? X_OFFSET * i + X_OFFSET / 2 : X_OFFSET * i, 0, Z_OFFSET * j), Quaternion.identity, transform).transform.localScale = new Vector3(1, Random.Range(1, 3), 1);
                    }


        }

    }
}