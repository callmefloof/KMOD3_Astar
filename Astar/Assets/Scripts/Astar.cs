﻿using System.Collections;
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
                
            };

            foreach(Vector2Int direction in directions)
            {
                Vector2Int neighborPos = currentPos + direction;
                if(neighborPos.x > grid.GetLength(0) 
                    || neighborPos.y > grid.GetLength(1)
                    || neighborPos.x < 0
                    || neighborPos.y < 0)
                {
                    continue;
                }
                
                if(closedSet.Any(node => node.position == neighborPos))
                {
                    continue;
                }
                Node neighbor = openSet.Find(n => n.position == neighborPos);

                float distCtoN = Vector2Int.Distance(current.position, neighborPos) *10;
                int tentativeGScore = (int)(current.GScore + distCtoN);
                if(neighbor == null || tentativeGScore < neighbor.GScore)
                {
                    if(IsCollidingWithWall(grid[currentPos.x, currentPos.y], direction))
                    {
                        continue;
                    }
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

    private bool IsCollidingWithWall(Cell cell, Vector2Int direction)
    {
        if((cell.walls & Wall.LEFT ) != 0 && direction == Vector2Int.left)
        {
            return true;
        }
        if((cell.walls & Wall.RIGHT ) != 0 && direction == Vector2Int.right)
        {
            return true;
        }
        if((cell.walls & Wall.UP ) != 0 && direction == Vector2Int.up)
        {
            return true;
        }
        if((cell.walls & Wall.DOWN ) != 0 && direction == Vector2Int.down)
        {
            return true;
        }
        return false;
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
