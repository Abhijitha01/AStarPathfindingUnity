using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class GridXZ<TGridObject>
{
    private const float HEX_VERTICAL_OFFSET_MULTIPLIER = 0.75f;
    public EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x; public int z;
    }
    private int width;
    private int height;
    private float cellSize;
    private Vector3 orginPosition;
    private TGridObject[,] gridArray;
    public GridXZ(int width, int height, float cellSize,Vector3 orginPosition,Func<GridXZ<TGridObject>,int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.orginPosition = orginPosition;
        gridArray = new TGridObject[width, height];
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                gridArray[x, z] = createGridObject(this,x,z);
            }
        }
        bool showDebug = false;
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

    public Vector3 GetworldPosition(int x, int z)
    {
        return 
            new Vector3(x,0,0)*cellSize +
            new Vector3(0,0,z)*cellSize * HEX_VERTICAL_OFFSET_MULTIPLIER +
            ((z%2)==1 ? new Vector3(1,0,0) * cellSize * 0.5f : Vector3.zero) +
            orginPosition;
    }
    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }
    public void SetGridObject(int x, int z, TGridObject value)
    {
        if(x>=0 && z>=0 && x<width && z<height)
        {
            gridArray[x,z] = value;
            if(OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, z = z });

        }
    }
    private void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        int roughX = Mathf.FloorToInt((worldPosition-orginPosition).x/cellSize);
        int roughZ = Mathf.FloorToInt((worldPosition - orginPosition).z / cellSize/ HEX_VERTICAL_OFFSET_MULTIPLIER);
        Vector3Int roughXZ = new Vector3Int(roughX, 0, roughZ);

        bool oddRow = roughZ % 2 == 1;

        List<Vector3Int> neighbourXZList = new List<Vector3Int>
        {
            roughXZ + new Vector3Int(-1,0,0),
            roughXZ + new Vector3Int(+1,0,0),

            roughXZ + new Vector3Int(oddRow?+1:-1,0,+1),
            roughXZ + new Vector3Int(+0,0,+1),

            roughXZ + new Vector3Int(oddRow?+1:-1,0,-1),
            roughXZ + new Vector3Int(+0,0,-1),
        };
        //Debug.Log("##########");
        //Debug.Log("roughZ: "+roughXZ);
        Vector3Int closestXZ = roughXZ;
        foreach (Vector3Int neighnourXZ  in neighbourXZList)
        {
            if (Vector3.Distance(worldPosition, GetworldPosition(neighnourXZ.x, neighnourXZ.z)) < 
                Vector3.Distance(worldPosition, GetworldPosition(closestXZ.x, closestXZ.z)))
            {
                //Closer than the closest
                closestXZ = neighnourXZ;
            }
        }
        x = closestXZ.x;
        z = closestXZ.z;
    }
    public void SetGridObject(Vector3 worldPosition,TGridObject value)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        SetGridObject(x, z, value);
    }
    public TGridObject GetGridObject(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            return gridArray[x,z];
        }
        else
        {
            return default(TGridObject);
        }
    }
    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, z;
        GetXZ(worldPosition,out x,out z);
        return GetGridObject(x, z);
    }
    
}
