using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Grid<TGridObject>
{
    public EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x; public int y;
    }
    private int width;
    private int height;
    private float cellSize;
    private Vector3 orginPosition;
    private TGridObject[,] gridArray;
    public Grid(int width, int height, float cellSize,Vector3 orginPosition,Func<Grid<TGridObject>,int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.orginPosition = orginPosition;
        gridArray = new TGridObject[width, height];
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject(this,x,y);
            }
        }
        bool showDebug = true;
        if(showDebug)
        {
            ShowDebug(width, height, cellSize);
        }
        

    }

    private void ShowDebug(int width, int height, float cellSize)
    {
        for (int i = 0; i < gridArray.GetLength(0); i++)
        {
            for (int j = 0; j < gridArray.GetLength(1); j++)
            {
                UtilsClass.CreateWorldText(gridArray[i, j].ToString(), null, GetworldPosition(i, j) + new Vector3(cellSize, cellSize) * .5f, 20, Color.black, TextAnchor.MiddleCenter);
                Debug.DrawLine(GetworldPosition(i, j), GetworldPosition(i, j + 1), Color.black, 100f);
                Debug.DrawLine(GetworldPosition(i, j), GetworldPosition(i + 1, j), Color.black, 100f);
            }
        }
        Debug.DrawLine(GetworldPosition(0, height), GetworldPosition(width, height), Color.black, 100f);
        Debug.DrawLine(GetworldPosition(width, 0), GetworldPosition(width, height), Color.black, 100f);
    }

    private Vector3 GetworldPosition(int x, int y)
    {
        return new Vector3(x, y)*cellSize + orginPosition;
    }
    public void SetGridObject(int x, int y, TGridObject value)
    {
        if(x>=0 && y>=0 && x<width && y<height)
        {
            gridArray[x,y] = value;
            if(OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });

        }
    }
    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition-orginPosition).x/cellSize);
        y = Mathf.FloorToInt((worldPosition - orginPosition).y / cellSize);
    }
    public void SetGridObject(Vector3 worldPosition,TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    }
    public TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x,y];
        }
        else
        {
            return default(TGridObject);
        }
    }
    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition,out x,out y);
        return GetGridObject(x, y);
    }
}
