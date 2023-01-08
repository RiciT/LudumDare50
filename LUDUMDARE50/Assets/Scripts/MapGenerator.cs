using AStar;
using AStar.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance;

    public const int width = 100;
    public const int height = 60;

    public Tilemap tileMapWall;
    public TileBase[] tileBases;
    public int evaluateCount = 100;
    public int mapTryCount = 0;
    public GameObject background;

    private short[,] map;
    public List<Vector2> emptyCells;

    private PathFinderOptions pathfinderOptions = new PathFinderOptions
    {
        PunishChangeDirection = false,
        UseDiagonals = false,
        SearchLimit = width * height,
    };
    private WorldGrid worldGrid;
    private PathFinder pathfinder;

    private void Start()
    {
        Instance = this;
    }

    #region Debug Spawncells
    /*
    if (emptyCells != null)
    {
        for (int i = 0; i < emptyCells.Count; i++)
        {
            Vector3Int pos = new Vector3Int((int)emptyCells[i].y - width / 2, (int)emptyCells[i].x - height / 2, 0);
            tileMapWall.SetTile(pos, tileBases[0]);
        }
    }
    */
    #endregion

    public IEnumerator CreateMap()
    {
        yield return new WaitForSeconds(1);

        GenerateMap();
        DisplayMap();
        RefineMap();

        yield return new WaitForSeconds(1);

        MenuManager.Instance.UpdateTutorial();
    }
    public void ClearMap()
    {
        tileMapWall.ClearAllTiles();
    }

    private void GenerateMap()
    {
        background.transform.localScale = new Vector3(width, height, 1);
        do
        {
            mapTryCount++;

            map = CellularAutomata.GenerateMap(width, height);
            emptyCells = CellularAutomata.GetEmptyCells(map);

            worldGrid = new WorldGrid(map);
            pathfinder = new PathFinder(worldGrid, pathfinderOptions);
        }
        while (!CheckMap());
    }
    private void RefineMap()
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == 0)
                {
                    if (i != 0 && i != map.GetLength(0) - 1 && j != 0 && j != map.GetLength(1) - 1)
                    {
                        if (map[i, j - 1] == 0 && map[i + 1, j] == 0 && map[i, j + 1] == 1 && map[i - 1, j] == 1)
                        {
                            Vector3Int pos = new Vector3Int(j - width / 2, i - height / 2, 0);
                            tileMapWall.SetTile(pos, tileBases[1]);
                            map[i, j] = 2;
                        }
                        else if (map[i + 1, j] == 0 && map[i, j + 1] == 0 && map[i - 1, j] == 1 && map[i, j - 1] == 1)
                        {
                            Vector3Int pos = new Vector3Int(j - width / 2, i - height / 2, 0);
                            tileMapWall.SetTile(pos, tileBases[2]);
                            map[i, j] = 2;
                        }
                        else if (map[i, j + 1] == 0 && map[i - 1, j] == 0 && map[i, j - 1] == 1 && map[i + 1, j] == 1)
                        {
                            Vector3Int pos = new Vector3Int(j - width / 2, i - height / 2, 0);
                            tileMapWall.SetTile(pos, tileBases[3]);
                            map[i, j] = 2;
                        }
                        else if (map[i - 1, j] == 0 && map[i, j - 1] == 0 && map[i + 1, j] == 1 && map[i, j + 1] == 1)
                        {
                            Vector3Int pos = new Vector3Int(j - width / 2, i - height / 2, 0);
                            tileMapWall.SetTile(pos, tileBases[4]);
                            map[i, j] = 2;
                        }
                    }
                }
            }
        }
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == 2)
                {
                    if (i != 0 && i != map.GetLength(0) - 1 && j != 0 && j != map.GetLength(1) - 1)
                    {
                        if (map[i + 1, j] == 2)
                        {
                            map[i, j] = 0;
                            map[i + 1, j] = 0;

                            Vector3Int pos = new Vector3Int(j - width / 2, i - height / 2, 0);
                            tileMapWall.SetTile(pos, null);
                            pos = new Vector3Int(j - width / 2, i + 1 - height / 2, 0);
                            tileMapWall.SetTile(pos, null);
                        }
                        else if (map[i - 1, j] == 2)
                        {
                            map[i, j] = 0;
                            map[i - 1, j] = 0;

                            Vector3Int pos = new Vector3Int(j - width / 2, i - height / 2, 0);
                            tileMapWall.SetTile(pos, null);
                            pos = new Vector3Int(j - width / 2, i - 1 - height / 2, 0);
                            tileMapWall.SetTile(pos, null);
                        }
                        else if (map[i, j - 1] == 2)
                        {
                            map[i, j] = 0;
                            map[i, j - 1] = 0;

                            Vector3Int pos = new Vector3Int(j - width / 2, i - height / 2, 0);
                            tileMapWall.SetTile(pos, null);
                            pos = new Vector3Int(j - 1 - width / 2, i - height / 2, 0);
                            tileMapWall.SetTile(pos, null);
                        }
                        else if (map[i, j + 1] == 2)
                        {
                            map[i, j] = 0;
                            map[i, j + 1] = 0;

                            Vector3Int pos = new Vector3Int(j - width / 2, i - height / 2, 0);
                            tileMapWall.SetTile(pos, null);
                            pos = new Vector3Int(j + 1 - width / 2, i - height / 2, 0);
                            tileMapWall.SetTile(pos, null);
                        }
                    }
                }
            }
        }
    }

    private void DisplayMap()
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                Vector3Int pos = new Vector3Int(j - width / 2, i - height / 2, 0);
                if (map[i, j] == 0)
                {
                    tileMapWall.SetTile(pos, tileBases[map[i, j]]);
                }
            }
        }
    }

    private bool CheckMap()
    {
        for (int i = 0; i < evaluateCount; i++)
        {
            Vector2 start = emptyCells[Random.Range(0, emptyCells.Count - 1)];
            Vector2 end = emptyCells[Random.Range(0, emptyCells.Count - 1)];

            bool evaluation = EvaluateMap(start, end);
            if (!evaluation)
            {
                return false;
            }
        }
        return true;
    }
    private bool EvaluateMap(Vector2 start, Vector2 end)
    {
        Position[] path = pathfinder.FindPath(new Position((int)start.x, (int)start.y), new Position((int)end.x, (int)end.y));
        return path.Length == 0 ? false : true;
    }
    public Vector2 GetSpawnPoint()
    {
        Vector2 emptyCell = emptyCells[Random.Range(0, emptyCells.Count)];
        return new Vector2(emptyCell.y + 0.5f, emptyCell.x) - new Vector2(width / 2, height / 2);
    }
}