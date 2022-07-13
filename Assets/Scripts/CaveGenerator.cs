using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveGenerator : MonoBehaviour
{
    [Header("Map Generation")]
    public int width;
    public int height;
    [SerializeField] private float seed;

    [Header("Cave Generation")]
    [Range(0, 100)]
    [SerializeField] private int randomFillPercent;
    [SerializeField] private int smoothAmmount;

    [Header("Tiles")]
    [SerializeField] private Tilemap CaveTilemap;
    [SerializeField] private Tilemap BackgroundTilemap;
    [SerializeField] private Tilemap DecorTilemap;
    [SerializeField] private TileBase CaveTile;
    [SerializeField] private TileBase BackgroundTile;
    [SerializeField] private TileBase BorderTile;
    [SerializeField] private TileBase PlatformTile;
    [SerializeField] private TileBase[] CeilingDecor;
    [SerializeField] private TileBase[] FloorDecor;

    private int[,] map;

    void Start()
    {
        GenerateMap();
    }

    void Update()
    {
          
    }

    void GenerateMap()
    {
        ClearTiles();
        seed = Random.Range(-9999, 9999);
        map = GenerateArray(true);
        map = TerrainGeneration(map);
        SmoothCave(smoothAmmount);
        PlaceCaveTiles(map);
        PlaceBorder(map);
        CreateEntrance();
        PlaceDecorTiles(map);
    }

    void ClearTiles()
    {
        CaveTilemap.ClearAllTiles();
        BackgroundTilemap.ClearAllTiles();
        DecorTilemap.ClearAllTiles();
    }

    public int[,] GenerateArray(bool empty)
    {
        int[,] nmap = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                nmap[x, y] = empty ? 0 : 1;
            }
        }
        return nmap;
    }

    public int[,] TerrainGeneration(int[,] map)
    {
        System.Random pseudoRandom = new System.Random(seed.GetHashCode());
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x < 2 || y < 2 || x > width - 2 || y > height - 2)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = (pseudoRandom.Next(1, 100) < randomFillPercent) ? 1 : 2;
                }
            }
        }
        return map;
    }

    void SmoothCave(int smoothAmmount)
    {
        for (int i = 0; i < smoothAmmount; i++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int surroundingGroundCount = GetSurroundingGroundCount(x, y);
                    if (surroundingGroundCount > 4)
                    {
                        map[x, y] = 1;
                    }
                    else if (surroundingGroundCount < 4)
                    {
                        map[x, y] = 2;
                    }
                }
            }
        }
    }

    int GetSurroundingGroundCount(int gridX, int gridY)
    {
        int groundCount = 0;

        for (int x = gridX - 1; x <= gridX + 1; x++)
        {
            for (int y = gridY - 1; y <= gridY + 1; y++)
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    if (x != gridX || y != gridY)
                    {
                        if (map[x, y] == 1)
                        {
                            groundCount++;
                        }
                    }
                }
            }
        }

        return groundCount;
    }

    void PlaceBorder(int[,] map)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    CaveTilemap.SetTile(new Vector3Int(x, y, 0), BorderTile);
                }
            }
        }
    }

    public void PlaceCaveTiles(int[,] map)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 1)
                {
                    CaveTilemap.SetTile(new Vector3Int(x, y, 0), CaveTile);
                }
                BackgroundTilemap.SetTile(new Vector3Int(x, y, 0), BackgroundTile);
            }
        }
    }

    public void PlaceDecorTiles(int[,] map)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (CaveTilemap.GetTile(new Vector3Int(x, y, 0)) != null
                    && CaveTilemap.GetTile(new Vector3Int(x, y, 0)) == CaveTile)
                {
                    if (CaveTilemap.GetTile(new Vector3Int(x, y - 1, 0)) == null)
                    {
                        int j = Random.Range(0, CeilingDecor.Length*2);
                        if (j < CeilingDecor.Length)
                        {
                            DecorTilemap.SetTile(new Vector3Int(x, y - 1, 0), CeilingDecor[j]);
                        }
                    }

                    if (CaveTilemap.GetTile(new Vector3Int(x, y + 1, 0)) == null)
                    {
                        int j = Random.Range(0, FloorDecor.Length * 4);
                        if (j < FloorDecor.Length)
                        {
                            DecorTilemap.SetTile(new Vector3Int(x, y + 1, 0), FloorDecor[j]);
                        }
                    }
                }
            }
        }
    }

    void CreateEntrance()
    {
        for (int x = -5; x < 6; x++)
        {
            for (int y = 0; y <= 10; y++)
            {
                CaveTilemap.SetTile(new Vector3Int(width/2 + x, height - y, 0), null);
            }
            if (Mathf.Abs(x) < 3)
            {
                CaveTilemap.SetTile(new Vector3Int(width/2 + x, height - 5, 0), PlatformTile);
            }
        }
    }
}