using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinding;

public class PathNode :  IAStarNode
{
    private GridXZ<PathNode> grid;
    public int x;
    public int z;
    public HexTileType tileType;
    public int tileCost;

    public PathNode(GridXZ<PathNode> grid, int x, int z)
    {
        this.grid = grid;
        this.x = x;
        this.z = z;
    }
    public Transform visualTransform;
    public void Show_Selected()
    {
        
        visualTransform.Find("Hexagon_Model_Selected").gameObject.SetActive(true);
        visualTransform.Find("Hexagon_Model").gameObject.SetActive(false);
        visualTransform.Find("Hexagon_Model_Path").gameObject.SetActive(false);
    }
    public void Hide_Selected()
    {
        visualTransform.Find("Hexagon_Model_Selected").gameObject.SetActive(false);
        visualTransform.Find("Hexagon_Model").gameObject.SetActive(true);
    }
    public void Show_Path()
    {
        visualTransform.Find("Hexagon_Model_Path").gameObject.SetActive(true);
        visualTransform.Find("Hexagon_Model").gameObject.SetActive(false);
    }
    public void Hide_Path()
    {
        visualTransform.Find("Hexagon_Model_Path").gameObject.SetActive(false);
        visualTransform.Find("Hexagon_Model").gameObject.SetActive(true);
    }
    public IEnumerable<IAStarNode> Neighbours => GetNeighbours();

    public float CostTo(IAStarNode neighbour)
    {
        PathNode currentNode = this;
        float cost = 0;
        if(neighbour is PathNode pathNodeNeighbour)
        {
            cost = currentNode.tileCost+pathNodeNeighbour.tileCost;
            return cost;
        }
        else
        {
            return -1f;
        }
    }

    public float EstimatedCostTo(IAStarNode target)
    {
        //Assuming we have same tiles from this node to target
        PathNode a = this;
        PathNode b = target as PathNode;

        return tileCost * Vector3.Distance(grid.GetworldPosition(a.x, a.z), grid.GetworldPosition(b.x, b.z));
    }
    public List<IAStarNode> GetNeighbours()
    {
        PathNode currentNode = this;
        List<PathNode> neighbourList = new List<PathNode>();
        List<IAStarNode> neighbours = new List<IAStarNode>();
        bool oddRow = currentNode.z % 2 == 1;

        if (currentNode.x - 1 >= 0)
        {
            // Left
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.z));
        }
        if (currentNode.x + 1 < grid.GetWidth())
        {
            // Right
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.z));
        }
        if (currentNode.z - 1 >= 0)
        {
            // Down
            neighbourList.Add(GetNode(currentNode.x, currentNode.z - 1));
        }
        if (currentNode.z + 1 < grid.GetHeight())
        {
            // Up
            neighbourList.Add(GetNode(currentNode.x, currentNode.z + 1));
        }

        if (oddRow)
        {
            if (currentNode.z + 1 < grid.GetHeight() && currentNode.x + 1 < grid.GetWidth())
            {
                neighbourList.Add(GetNode(currentNode.x + 1, currentNode.z + 1));
            }
            if (currentNode.z - 1 >= 0 && currentNode.x + 1 < grid.GetWidth())
            {
                neighbourList.Add(GetNode(currentNode.x + 1, currentNode.z - 1));
            }
        }
        else
        {
            if (currentNode.z + 1 < grid.GetHeight() && currentNode.x - 1 >= 0)
            {
                neighbourList.Add(GetNode(currentNode.x - 1, currentNode.z + 1));
            }
            if (currentNode.z - 1 >= 0 && currentNode.x - 1 >= 0)
            {
                neighbourList.Add(GetNode(currentNode.x - 1, currentNode.z - 1));
            }
        }
        foreach (var neighbour in neighbourList)
        {
            if (neighbour != null && neighbour.tileType != HexTileType.Water)
            {
                neighbours.Add(neighbour);
            }
        }
        return neighbours;
    }
    public PathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

}
