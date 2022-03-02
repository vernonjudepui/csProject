using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
public class Grid
{
    private int width;
    private int height;
    private int[,] gridArray;
    private float cellSize;

    public Grid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.gridArray = new int[width, height]; 
        Debug.Log(width + " " + height);
        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {

                UtilsClass.CreateWorldText(gridArray[x, y].ToString(),null,GetWorldPosition(x,y)+ new Vector3(cellSize,cellSize)*.5f,20,Color.black,TextAnchor.MiddleCenter);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1),Color.black,199f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x+1, y),Color.black,199f);
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.black, 199f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.black, 199f);
        }
        Debug.Log(width + "" + height);
        SetValue(1, 0, 56);
    }
    private Vector3 GetWorldPosition(int x, int y) {
        return new Vector3(x, y) * cellSize;
    }
    public void SetValue(int x, int y, int value) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            gridArray[x, y] = value;
        }
       
    }
}
