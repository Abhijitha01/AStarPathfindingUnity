using PathFinding;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using Utils;

public class HexGrid : MonoBehaviour
{
    [SerializeField] private Transform pfHexTile;
    private GridXZ<PathNode> gridXZ;
    private PathNode lastGridObject;
    // ORDER OF THE LIST EXTREMELY IMP BE CAREFULL
    [SerializeField] List<Material> grassMaterials;
    [SerializeField] List<Material> forestMaterials;
    [SerializeField] List<Material> sandMaterials;
    [SerializeField] List<Material> mountainMaterials;
    [SerializeField] List<Material> waterMaterials;

    public Dictionary<HexTileType, int> tileTravelCosts = new Dictionary<HexTileType, int>()
    {
        { HexTileType.Grass, 1 },
        { HexTileType.Forest, 3 },
        { HexTileType.Sand, 5 },
        { HexTileType.Mountain, 10 },
        { HexTileType.Water, int.MaxValue }, // Infinite cost for water
    };
    PathNode startTile;
    PathNode destinationTile;
    IList<IAStarNode> pathList;
    private class GridObject
    {
        public Transform visualTransform;
        public void Show()
        {
            visualTransform.Find("Hexagon_Model_Selected").gameObject.SetActive(true);
        }
        public void Hide()
        {
            visualTransform.Find("Hexagon_Model_Selected").gameObject.SetActive(false);
        }
    }
    private void Awake()
    {
        int width = 10;
        int height = 6;
        float cellSize = 1f;
        gridXZ = new GridXZ<PathNode>(width, height, cellSize, Vector3.zero, (GridXZ<PathNode> g, int x, int z) => new PathNode(g, x, z));
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                InitializePathNode(x, z);

            }
        }
    }

    private void InitializePathNode(int x, int z)
    {
        Transform visualTransform = Instantiate(pfHexTile, gridXZ.GetworldPosition(x, z), Quaternion.identity);
        PathNode instance = gridXZ.GetGridObject(x, z);
        instance.visualTransform = visualTransform;
        instance.Hide_Selected();
        instance.tileType = GetRandomTileType();
        List<Material> mat = GetMaterialForType(instance.tileType);
        SetMaterial(mat, instance.visualTransform);
        instance.tileCost = tileTravelCosts[instance.tileType];
        
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            SelectTile();
        }
    }

    private void SelectTile()
    {
        PathNode clickedTile = gridXZ.GetGridObject(UtilsClass.MousePosition3D(Camera.main));
        if (clickedTile != null && clickedTile.tileType!= HexTileType.Water)
        {
            if (startTile == null)
            {
                startTile = clickedTile;
                startTile.Show_Selected();

                Debug.Log("Selected starting tile.");
            }
            else if (destinationTile == null)
            {
                destinationTile = clickedTile;
                Debug.Log("Selected destination tile.");
                
                pathList = AStar.GetPath(gridXZ.GetGridObject(startTile.x, startTile.z), gridXZ.GetGridObject(destinationTile.x, destinationTile.z));
                if(pathList == null)
                {
                    Debug.Log("Path not possible");
                    return;
                }
                foreach (IAStarNode node in pathList)
                {
                    PathNode pathNode = node as PathNode;
                    Debug.Log("pathNode: " + pathNode.tileType+" "+"tileCost: "+pathNode.tileCost);
                    pathNode.Show_Path();
                    destinationTile.Show_Selected();
                    startTile.Show_Selected();
                }
            }
            else
            {
                // Both starting and destination tiles are selected, clear the selections
                startTile.Hide_Selected();
                destinationTile.Hide_Selected();
                startTile = null;
                destinationTile = null;
                if(pathList != null)
                {
                    foreach (IAStarNode node in pathList)
                    {
                        PathNode pathNode = node as PathNode;
                        pathNode.Hide_Path();
                        pathNode = null;
                    }
                }
                
                Debug.Log("Cleared selections.");
            }
        }
        else
        {
            Debug.Log("Invalid Tile");
        }
    }

    private HexTileType GetRandomTileType()
    {
        // Generate a random number between 0 and the number of tile types
        int randomIndex = Random.Range(0, System.Enum.GetValues(typeof(HexTileType)).Length);
        // Convert the random index to a HexTileType enum value
        return (HexTileType)randomIndex;
    }
    private List<Material> GetMaterialForType(HexTileType tileType)
    {
        // Implement logic to return the material based on the tile type
        // You can use a switch statement or other logic to map types to materials
        // Make sure you have assigned materials for each tile type in the Unity Editor
        switch (tileType)
        {
            case HexTileType.Grass:
                return grassMaterials; // Example: Load Grass material
            case HexTileType.Forest:
                return forestMaterials; // Example: Load Forest material
            case HexTileType.Sand:
                return sandMaterials;
            case HexTileType.Mountain:
                return mountainMaterials;
            case HexTileType.Water:
                return waterMaterials;
            default:
                return null; // Return null for unsupported types
        }
    }
    public void SetMaterial(List<Material> material,Transform currentNode)
    {
        MeshRenderer meshRenderer_main = currentNode.Find("Hexagon_Model").GetComponent<MeshRenderer>();
        MeshRenderer meshRenderer_selected = currentNode.Find("Hexagon_Model_Selected").GetComponent<MeshRenderer>();
        MeshRenderer meshRenderer_path = currentNode.Find("Hexagon_Model_Path").GetComponent<MeshRenderer>();
        // Apply the material to the tile's mesh renderer here

        if (meshRenderer_main != null && material != null)
        {
            Debug.Log("Here");
            meshRenderer_main.material = material[0];
            meshRenderer_selected.material = material[1];
            meshRenderer_path.material = material[2];
        }
    }
    
}
