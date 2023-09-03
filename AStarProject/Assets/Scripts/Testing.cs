using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
public class Testing : MonoBehaviour
{
    private GridXZ<int> grid;
    private void Start()
    {
        grid = new GridXZ<int>(4,5,10f,new Vector3(0,0),(GridXZ<int> g,int x, int y)=>new int());
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            grid.SetGridObject(UtilsClass.MousePosition3D(Camera.main), 20);
            Debug.Log(grid.GetGridObject(UtilsClass.MousePosition3D(Camera.main)));
            Debug.Log(UtilsClass.MousePosition3D(Camera.main));
        }
    }
}
