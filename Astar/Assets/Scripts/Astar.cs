using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.Experimental.GraphView;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path from the startPos to the endPos
    /// Note that you will probably need to add some helper functions
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        
        
        List<Node> openSet = new List<Node>();
        List<Node> closedSet = new List<Node>();

        float distStartToEnd = Vector2Int.Distance(startPos, endPos);
        int hScoreStart = Mathf.RoundToInt(distStartToEnd*10);
        Node startNode = new Node(startPos, null, 0, hScoreStart);
        openSet.Add(startNode);
        
        
        while(openSet.Count != 0)
        {
            Node current = openSet.OrderBy(node => node.FScore).First();
            if(current.position == endPos)
            {
                return ReconstructPath(current);
            }
            Vector2Int currentPos = current.position;
            openSet.Remove(current);
            closedSet.Add(current);
            
            List<Vector2Int> directions = new List<Vector2Int>
            {
                Vector2Int.left,
                Vector2Int.right,
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left + Vector2Int.up,
                Vector2Int.right + Vector2Int.up,
                Vector2Int.left + Vector2Int.down,
                Vector2Int.right + Vector2Int.down
            };

            foreach(Vector2Int direction in directions)
            {
                Vector2Int neighborPos = currentPos + direction;
                if(closedSet.Any(node => node.position == neighborPos))
                {
                    continue;
                }
                Node neighbor = openSet.Find(n => n.position == neighborPos);

                float distCtoN = Vector2Int.Distance(current.position, neighborPos) *10;
                int tentativeGScore = (int)(current.GScore + distCtoN);
                if(neighbor == null || tentativeGScore < neighbor.GScore)
                {
                    
                    if(neighbor == null)
                    {
                        int distNtoE = (int)Vector2Int.Distance(current.position, neighborPos) *10;
                        neighbor = new Node(neighborPos, current, tentativeGScore,distNtoE);
                    }
                    else
                    {
                        neighbor.GScore = tentativeGScore;
                        neighbor.parent = current;
                    }
                    if(!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
                
            }
            
            
        }

        return null;
    }


    private List<Vector2Int> ReconstructPath(Node lastNode)
    {
        List<Vector2Int> totalPath = new List<Vector2Int>();
        Node currentNode = lastNode;
        while(currentNode != null)
        {
            if(totalPath.Count == 0)
            {
                totalPath.Add(currentNode.position);
            }
            else
            {
                totalPath = totalPath.Prepend(currentNode.position).ToList();
            }
            currentNode = currentNode.parent;

        }
        return totalPath;
    }

    // private Dictionary<Vector2Int, Node> GenerateNodes(Cell[,] grid)
    // {
        
    //     Dictionary<Vector2Int,Node> nodes = new Dictionary<Vector2Int, Node>();
    //     for(int x = 0; x < grid.GetLength(0); x++)
    //     {
    //         for(int y = 0; y < grid.GetLength(1); y++)
    //         {
    //             int gCost = 0;     
    
    //             Node node = new Node(grid[x,y].gridPosition, null, gCost, 0);
    //             nodes.Add(grid[x,y].gridPosition, node);
    //         }
    //     }
    //     return nodes;
    // }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
