using RotatingRoutes.Managers;
using RotatingRoutes.Pathfinding;
using RotatingRoutes.Util.ObjectPooling;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RotatingRoutes.Hex
{

    public class HexGridGenerator : MonoBehaviour
    {
        public int RowAmount => GameManager.BaseRowColAmount + GameManager.ProgressionAmount;
        public int ColAmount => GameManager.BaseRowColAmount + GameManager.ProgressionAmount;

        public PlayerMover Player;

        public const float X_OFFSET = 2f;
        public const float Z_OFFSET = 1.75f;

        public Dictionary<(int row, int column), HexTileStatus> SpawnedHexTiles { get; private set; } = new();

        private void Awake() => GenerateLevel();

        [SerializeField] float blockerChance = 5;
        [SerializeField] float straightWalkableChance = 10;
        [SerializeField] float narrowCurveChance = 20;

        [SerializeField] int blockerMinimumRange = 2;

        int totalBlockers, totalWalkableStraight, totalNarrowCurve;


        [ContextMenu("Generate!")]
        private void GenerateLevel()
        {
            SpawnedHexTiles.Clear();
            totalBlockers = 0;
            totalWalkableStraight = 0;
            totalNarrowCurve = 0;
            Player.transform.position = new Vector3(RowAmount + 1, 1, ColAmount * Z_OFFSET + ((ColAmount & 1) == 0 ? Z_OFFSET : 0) + ((RowAmount & 1) == 0 ? 0 : Z_OFFSET));


            for (int i = 0; i < RowAmount; i++)
                for (int j = 0; j < ColAmount; j++)
                {
                    HexTileType tileType;
                    bool blockerCondition = Random.Range(0, 100) < blockerChance
                                            && !TileTypeInRange(i, j, HexTileType.Blocker, blockerMinimumRange)
                                            && i >= 1
                                            && i <= RowAmount - 1
                                            && j >= 1
                                            && j <= ColAmount - 1;
                    if (blockerCondition)
                    {
                        totalBlockers++;
                        tileType = HexTileType.Blocker;
                    }
                    else tileType = SpawnTileAsWalkable(i, j);
                    float tileRotation = Random.Range(0, 6) * 60;

                    SpawnedHexTiles.Add((i, j), new(tileType, tileRotation));
                }


            Debug.Log($"Total blocker {totalBlockers}, Total walkable straight {totalWalkableStraight}, Total narrow {totalNarrowCurve} Total tiles {SpawnedHexTiles.Count}");
            SpawnSideHills();
            SetPlayerStartPositions();
        }

        void SetPlayerStartPositions()
        {
            SpawnedHexTiles[(RowAmount, ColAmount / 2)] = new(HexTileType.WalkableStraight, 120);
            SpawnedHexTiles[(RowAmount, 1 + ColAmount / 2)] = new(HexTileType.WalkableStraight, -120);
            SpawnedHexTiles[(RowAmount + 1, (RowAmount & 1) == 0 ? ColAmount / 2 : 1 + ColAmount / 2)] = new(HexTileType.WalkableNarrowCurve, 240);
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

            int hillColAmount = 6;

            //LEFT SIDE
            for (int i = -hillColAmount; i < 0; i++)
                for (int j = -RowAmount; j < RowAmount; j++)
                    SpawnedHexTiles.Add((j, i), new(i == -1 ? HexTileType.Transition : HexTileType.Water, 180));


            //RIGHT SIDE
            for (int i = ColAmount; i < ColAmount + hillColAmount; i++)
                for (int j = -RowAmount; j < RowAmount; j++)
                    SpawnedHexTiles.Add((j, i), new(i == ColAmount ? HexTileType.Transition : HexTileType.Water, 0));

            //return;
            //DOWN SIDE
            for (int j = -hillColAmount; j < 0; j++)
                for (int i = 0; i < RowAmount; i++)
                    SpawnedHexTiles.Add((j, i), new(j == -1 ? HexTileType.HillTransition : HexTileType.Finale, 0));

            //UP SIDE
            for (int j = RowAmount; j < RowAmount + 3; j++)
                for (int i = 0; i < ColAmount; i++)
                    SpawnedHexTiles.Add((j, i), new(HexTileType.HillTransition, 0));


        }

    }
}