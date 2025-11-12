using NesScripts.Controls.PathFind;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// About the Grid
/// The grid internally does not allow negatives. This means all negative world points must be translated to grid points
/// The grid generates a list of nodes or points, and can use A* pathfinding on them
/// This 3D version uses the 2D grid's y axis as the unity world's z axis.

namespace NesScripts.Controls.PathFind
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] Vector2Int GridDimensions = new Vector2Int(100, 100);
        Grid grid;
        bool[,] tilesmap;
        [SerializeField] List<Vector3> finalPath;
        [SerializeField] LayerMask groundMask;

        public Vector3Int startPoint;
        public Vector3Int endPoint;
        public bool findPath;

        void Awake()
        {
            gameObject.tag = "GridManager";

            // Create tile matrix. Add +1 to account for (0,0).
            tilesmap = new bool[GridDimensions.x + 1, GridDimensions.y + 1];

            // Populate tile matrix
            for(int x = 0; x <= GridDimensions.x; x++)
            {
                for(int y = 0; y <= GridDimensions.y; y++)
                {
                    Vector3Int position = GridToWorld(new Vector2Int(x, y));
                    float radius = 0.1f;
                    Collider[] hitColliders = Physics.OverlapSphere(position, radius, groundMask);
                    if (hitColliders.Length > 0)
                    {
                        //Debug.Log(hitColliders[0].transform.name);
                        tilesmap[x, y] = false;
                    }
                    else
                        tilesmap[x, y] = true;
                    
                    // Check for world geometry
                    //tilesmap[x, y] = Physics.OverlapSphere(GridToWorld(new Vector2Int(x, y)), 0.1f, groundMask).Length == 0;
                }
            }

            // Create grid with tile matrix
            grid = new Grid(tilesmap);
        }

        private void Update()
        {
            if(findPath)
            {
                GetPath(startPoint, endPoint);
                findPath = false;
            }
        }

        public List<Vector3> GetPath(Vector3Int start, Vector3Int end)
        {
            Vector2Int start_2d = WorldToGrid(start);
            //Debug.Log("start_2d = " + start_2d);
            Vector2Int end_2d = WorldToGrid(end);
            //Debug.Log("end_2d = " + end_2d);
            finalPath.Clear();

            Point _from = new(start_2d.x, start_2d.y);
            Point _to = new(end_2d.x, end_2d.y);

            List<Point> path = Pathfinding.FindPath(grid, _from, _to, Pathfinding.DistanceType.Manhattan);
            //Debug.Log("path.count = " + path.Count);
            foreach (Point p in path)
                finalPath.Add(GridToWorld(new Vector2Int(p.x, p.y)));
            
            return finalPath;
        }

        // Converts from a world point to a grid point.
        Vector2Int WorldToGrid(Vector3Int worldPoint)
        {
            Vector2Int gridPoint = new Vector2Int(worldPoint.x + GridDimensions.x / 2, worldPoint.z + GridDimensions.y / 2);
            return gridPoint;
        }

        // Converts from a grid point to a world point.
        Vector3Int GridToWorld(Vector2Int gridPoint)
        {
            Vector3Int worldPoint = new Vector3Int(gridPoint.x - GridDimensions.x / 2, 0, gridPoint.y - GridDimensions.y / 2);
            return worldPoint;
        }
    }
}