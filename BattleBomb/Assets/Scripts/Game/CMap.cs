using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class CMap
{
    public CMapElement[][] map;
    public string mapName;
    public Vector3Int size;
    public Vector3Int []starting;

    public const int B_BLOCK = 0b001;
    public const int B_BROCKABLE = 0b010;
    public const int B_MOVABLE = 0b100;

    public CMap() { }
    public CMap(Vector3Int size)
    {
        int i, j;
        CMapElement blockwall = new CMapElement(CMap.B_BLOCK, Color.white);

        this.size = size;

        map = new CMapElement[size.x + 2][];
        for (i = 0; i <= size.x + 1; i++) map[i] = new CMapElement[size.z + 2];

        for (i = 0; i <= size.x + 1; i++) map[i][0] = map[i][size.z + 1] = blockwall;
        for (j = 0; j <= size.z + 1; j++) map[0][j] = map[size.x + 1][j] = blockwall;
    }

    public bool openMap(string name)
    {
        int i, j, n;
        string input;
        string[] parseInput;

        try
        {
            mapName = name;
            StreamReader reader = new StreamReader("map/" + name + ".map");

            if (reader == null) return false;

            // 크기
            input = reader.ReadLine();
            parseInput = input.Split(' ');

            size = new Vector3Int(int.Parse(parseInput[0]), 1, int.Parse(parseInput[1]));

            // 맵 초기화
            CMapElement blockwall = new CMapElement(CMap.B_BLOCK, Color.white);

            map = new CMapElement[size.x + 2][];
            for (i = 0; i <= size.x + 1; i++) map[i] = new CMapElement[size.z + 2];

            for (i = 0; i <= size.x + 1; i++) map[i][0] = map[i][size.z + 1] = blockwall;
            for (j = 0; j <= size.z + 1; j++) map[0][j] = map[size.x + 1][j] = blockwall;

            // 구성요소
            for (i = 1; i <= size.x; i++)
            {
                input = reader.ReadLine();
                string[] elements = input.Split(' ');
                for (j = 1; j <= size.z; j++)
                {
                    int type = int.Parse(elements[j * 2 - 2]);
                    string hexColor = elements[j * 2 - 1];

                    int r = int.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                    int g = int.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                    int b = int.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

                    map[i][j] = new CMapElement(type, new Color(r / 255f, g / 255f, b / 255f));
                }
            }

            // 스타팅
            input = reader.ReadLine();
            n = int.Parse(input);

            starting = new Vector3Int[n];
            for (i = 0; i < n; i++)
            {
                input = reader.ReadLine();
                parseInput = input.Split(' ');

                starting[i] = new Vector3Int(int.Parse(parseInput[0]), 0, int.Parse(parseInput[1]));
            }

            reader.Close();
            return true;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Failed to open map file: " + e.Message);
            return false;
        }
    }

}
