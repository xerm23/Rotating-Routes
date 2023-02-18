using Codice.Client.Common.GameUI;
using DG.Tweening;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using static DG.DemiEditor.DeEditorUtils;

namespace RotatingRoutes.Hex
{

    public class HexGridGenerator : MonoBehaviour
    {
        [SerializeField] HexTile hexTilePrefab;

        [SerializeField] int rowAmount;
        [SerializeField] int colAmount;
        [SerializeField] GameObject[] hillTransition;
        [SerializeField] GameObject[] hillTiles;

        private const float X_OFFSET = 2f;
        private const float Z_OFFSET = 1.75f;

        private Dictionary<(int row, int column), HexTile> _spawnedHexTiles = new();

        void Start()
        {
            HexTile.Pool.SetOriginal(hexTilePrefab);
            HexTile.Pool.SetParent(transform);
            Generate();
        }
        [SerializeField] float blockerChance = 5;
        [SerializeField] float straightWalkableChance = 10;
        [SerializeField] float narrowCurveChance = 20;

        [SerializeField] int blockerMinimumRange = 2;

        int totalBlockers, totalWalkableStraight, totalNarrowCurve;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) { Generate(); }

        }

        [ContextMenu("Generate!")]
        private void Generate()
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
            _spawnedHexTiles.Clear();
            totalBlockers = 0;
            totalWalkableStraight = 0;
            totalNarrowCurve = 0;


            for (int i = 0; i < rowAmount; i++)
                for (int j = 0; j < colAmount; j++)
                {
                    HexTile tile = HexTile.Pool.Get();
                    _spawnedHexTiles.Add((i, j), tile);
                    bool blockerCondition = Random.Range(0, 100) < blockerChance
                                            && !TileTypeInRange(i, j, HexTileType.Blocker, blockerMinimumRange)
                                            && i >= 1
                                            && i <= rowAmount - 1
                                            && j >= 1
                                            && j <= colAmount - 1;
                    if (blockerCondition)
                    {
                        totalBlockers++;
                        tile.SetHexTileType(HexTileType.Blocker);
                    }
                    else SpawnTileAsWalkable(i, j, tile);
                    tile.gameObject.name = $"Tile {i}-{j}";
                    tile.transform.SetPositionAndRotation(new Vector3(i % 2 != 0 ? X_OFFSET * j + X_OFFSET / 2 : X_OFFSET * j, 0, Z_OFFSET * i), Quaternion.Euler(0, Random.Range(0, 6) * 60, 0));
                }


            Debug.Log($"Total blocker {totalBlockers}, Total walkable straight {totalWalkableStraight}, Total narrow {totalNarrowCurve} Total tiles {_spawnedHexTiles.Count}");
            //SpawnSideHills();
        }

        private void SpawnTileAsWalkable(int row, int col, HexTile tile)
        {
            if (Random.Range(0, 100) < straightWalkableChance)// && !TileTypeInRange(row, col, HexTileType.WalkableStraight, 1))
            {
                tile.SetHexTileType(HexTileType.WalkableStraight);
                totalWalkableStraight++;
            }
            else if (Random.Range(0, 100) < narrowCurveChance)
            {
                totalNarrowCurve++;
                tile.SetHexTileType(HexTileType.WalkableNarrowCurve);
            }
            else tile.SetHexTileType(HexTileType.WalkableWideCurve);
        }


        private bool TileTypeInRange(int row, int col, HexTileType hexType, int range)
        {
            var tiles = TilesInRange((row, col), range);
            return tiles.Any(x => x.HexTileType == hexType);
        }
        private List<HexTile> TilesInRange((int x, int y) center, int range)
        {
            List<HexTile> resultsTiles = new();

            for (int q = -range; q <= range; q++)
            {
                for (int r = Mathf.Max(-range, -range - q); r <= Mathf.Min(range, range - q); r++)
                {
                    if (q == 0 && r == 0)
                        continue;

                    var (row, col) = Offset((center.x & 1) == 0, q, r);
                    var tileCoord = (center.x + row, center.y + col);

                    _spawnedHexTiles.TryGetValue(tileCoord, out var tile);
                    if (tile != null)
                        resultsTiles.Add(tile);
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