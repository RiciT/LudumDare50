using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CellularAutomata
{
    private static int iterationCount = 10;

    private const short empty = 1;
    private const short wall = 0;

    public static short[,] GenerateMap(int w, int h)
    {
        short[,] map = PopulateMap(w, h);        
        for (int i = 0; i < iterationCount; i++)
        {
            map = IterateMap(map);
        }

        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                if ((i == 0 || i == h - 1) || (j == 0 || j == w - 1)) map[i, j] = wall;
            }
        }
        return map;
    }
    private static short[,] PopulateMap(int w, int h)
    {
        short[,] map = new short[h, w];
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                //map[i, j] = Random.Range(0, 2) == 0 ? empty : wall;
                if (i <= 1 || i >= h - 2 || j <= 1 || j >= w - 2)
                {
                    map[i, j] = wall;
                }
                else
                {
                    map[i, j] = Random.Range(0, 2) == 0 ? empty : wall;
                }
            }
        }
        return map;
    }
    private static short[,] IterateMap(short[,] map)
    {
        short[,] newMap = map;
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                int wallCount = CountWallNeighbour(i, j, map);
                if (map[i, j] == wall && wallCount < 4)
                {
                    newMap[i, j] = empty;
                }
                else if (map[i, j] == empty && wallCount >= 5)
                {
                    newMap[i, j] = wall;
                }
            }
        }
        return newMap;
    }
    private static int CountWallNeighbour(int x, int y, short[,] map)
    {
        int count = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i != 0 || j != 0)
                {
                    if (x + i >= 0 && x + i < map.GetLength(0) && y + j >= 0 && y + j < map.GetLength(1))
                    {
                        if (map[x + i, y + j] == wall) count++;
                    }
                }
            }
        }
        return count;
    }

    public static void PrintArray(this short[,] arr)
    {
        string str = "";
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                str += arr[i, j];
            }
            str += "\n\r";
        }
        Debug.Log(str);
    }
    public static List<Vector2> GetEmptyCells(short[,] arr)
    {
        List<Vector2> list = new List<Vector2>();
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                if (arr[i, j] == empty)
                {
                    if (i != 0 && i != arr.GetLength(0) - 1 && j != 0 && j != arr.GetLength(1) - 1)
                    {
                        if (arr[i + 1, j] == empty && arr[i - 1, j] == empty && arr[i, j + 1] == empty && arr[i, j - 1] == empty)
                        {
                            list.Add(new Vector2(i, j));
                        }
                    }
                }
            }
        }
        return list;
    }
}
