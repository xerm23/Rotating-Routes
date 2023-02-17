using DG.Tweening;
using System.Data;
using UnityEngine;

namespace RotatingRoutes.Hex
{

    public class HexGridGenerator : MonoBehaviour
    {
        [SerializeField] HexTile hexTilePrefab;


        [SerializeField] int rowAmount;
        [SerializeField] int colAmount;
        [SerializeField] GameObject[] walkableTiles;
        [SerializeField] GameObject[] hillTransition;
        [SerializeField] GameObject[] hillTiles;

        private const float X_OFFSET = 2f;
        private const float Z_OFFSET = 1.75f;

        void Start()
        {
            HexTile.Pool.SetOriginal(hexTilePrefab);
            HexTile.Pool.SetParent(transform);
            Generate();
        }


        [ContextMenu("Generate!")]
        private void Generate()
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            for (int i = 0; i < rowAmount; i++)
                for (int j = 0; j < colAmount; j++)
                {
                    HexTile tile = HexTile.Pool.Get();
                    tile.SetHexTileType((HexTileType)Random.Range(0, 3));
                    tile.gameObject.name = $"Tile {i}-{j}";
                    tile.transform.SetPositionAndRotation(new Vector3(i % 2 != 0 ? X_OFFSET * j + X_OFFSET / 2 : X_OFFSET * j, 0, Z_OFFSET * i), Quaternion.Euler(0, Random.Range(0, 6) * 60, 0));
                }

            //SpawnSideHills();
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