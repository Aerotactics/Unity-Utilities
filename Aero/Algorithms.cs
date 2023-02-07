using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aero
{
    public static class Algorithms
    {
        public enum WorkStatus
        {
            Finished,       // We finished the search, exit immediately
            Continue,       // We are still searching, check neighbors
            SkipNeighbors,  // We are still searching, but don't check neighbors
        }

        // I don't remember why I wrote a custom Rect.
        public struct Rect
        {
            public float left;
            public float right;
            public float top;
            public float bottom;

            public Rect(float left, float right, float top, float bottom)
            {
                this.left = left;
                this.right = right;
                this.top = top;
                this.bottom = bottom;
            }

            public Vector2 Center()
            {
                return new Vector2(left + ((right - left) / 2f), bottom + ((top - bottom) / 2f));
            }
        }

        public enum Cube
        {

        }

        // A node which has neighbors
        public class GraphNode<T>
        {
            public T m_value = default(T);
            public List<DiscoverableNode<T>> m_neighbors = new List<DiscoverableNode<T>>();
            public GraphNode(T value) { this.m_value = value; }
        }

        // A graph node that can be discovered
        public class DiscoverableNode<T> : GraphNode<T>
        {
            public bool m_discovered = false;

            public DiscoverableNode(T value) : base(value) { }
        }

        public class BFSNode<T> : DiscoverableNode<T>
        {
            public BFSNode(T value) : base(value) { }
        }

        // Breadth First Search - Searches and works from the root node outwards
        //
        // Inputs:
        //  root - the starting node for our search. Must have neighbors assigned before search
        //  workFunction - the work to do or check on each node as it is handled
        public static void BreadthFirstSearch<T>(BFSNode<T> root, Func<BFSNode<T>, WorkStatus> workFunction)
        {
            Queue<BFSNode<T>> queue = new Queue<BFSNode<T>>();
            root.m_discovered = true;
            queue.Enqueue(root);
            while(queue.Count > 0)
            {
                BFSNode<T> node = queue.Dequeue();

                switch(workFunction(node))
                {
                    case WorkStatus.Finished: return;
                    case WorkStatus.SkipNeighbors: continue;
                }

                int count = node.m_neighbors.Count;
                for (int i = 0; i < count; ++i)
                {
                    BFSNode<T> neighbor = (BFSNode<T>)node.m_neighbors[i];
                    if(neighbor.m_discovered == false)
                    {
                        neighbor.m_discovered = true;
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        public class DFSNode<T> : DiscoverableNode<T>
        {
            public DFSNode(T value) : base(value) { }
        }

        // Depth First Search - Searches from the outermost nodes inwards towards the root
        //
        // Inputs:
        //  root - the starting node for our search. Must have neighbors assigned before search
        //  workFunction - the work to do or check on each node as it is handled
        public static void DepthFirstSearch<T>(DFSNode<T> root, Func<DFSNode<T>, WorkStatus> workFunction)
        {
            Stack<DFSNode<T>> stack = new Stack<DFSNode<T>>();
            root.m_discovered = true;
            stack.Push(root);
            while (stack.Count > 0)
            {
                DFSNode<T> node = stack.Pop();

                WorkStatus workStatus = workFunction(node);

                if (workStatus == WorkStatus.Finished)
                    return;
                else if (workStatus == WorkStatus.SkipNeighbors)
                    continue;

                int count = node.m_neighbors.Count;
                for (int i = 0; i < count; ++i)
                {
                    DFSNode<T> neighbor = (DFSNode<T>)node.m_neighbors[i];
                    if (neighbor.m_discovered == false)
                    {
                        neighbor.m_discovered = true;
                        stack.Push(neighbor);
                    }
                }
            }
        }

        public class DLSNode<T> : DiscoverableNode<T>
        {
            public uint m_depth = 0;

            public DLSNode(T value) : base(value) {}
        }

        // Depth-Limited Search - Searches and works from the root node outwards,
        //                          stopping at a given depth
        // Inputs:
        //  root - the starting node for our search. Must have neighbors assigned before search
        //  workFunction - the work to do or check on each node as it is handled
        //  depthLimit - how many iterations deep we want to go for this search
        public static void DepthLimitedSearch<T>(DLSNode<T> root, Func<DLSNode<T>, WorkStatus> workFunction, uint depthLimit)
        {
            Stack<DLSNode<T>> stack = new Stack<DLSNode<T>>();
            root.m_discovered = true;
            stack.Push(root);
            while(stack.Count > 0)
            {
                DLSNode<T> node = stack.Pop();
                uint currentDepth = node.m_depth;

                WorkStatus workStatus = workFunction(node);

                if (workStatus == WorkStatus.Finished)
                    return;

                if (currentDepth == depthLimit || workStatus == WorkStatus.SkipNeighbors)
                    continue;

                int count = node.m_neighbors.Count;
                for(int i = 0; i < count; ++i)
                {
                    DLSNode<T> neighbor = (DLSNode<T>)node.m_neighbors[i];
                    if (neighbor.m_discovered == false)
                    {
                        neighbor.m_discovered = true;
                        neighbor.m_depth = currentDepth + 1;
                        stack.Push(neighbor);
                    }
                }
            }
        }

        // A node used for tree structures and algorithms
        public class TreeNode
        {
            public TreeNode m_parent = null;
            public List<TreeNode> m_children = new List<TreeNode>();

            public TreeNode(TreeNode parent = null, List<TreeNode> children = null)
            {
                m_parent = parent;
                if(children != null)
                    m_children = children;
            }
        }

        // A node used for Binary Space partitioning in 2D
        public class BSPTreeNode2D : TreeNode
        {
            public Aero.Algorithms.Rect m_rect;     // For now, BSP only works in 2D
            public bool m_splitVertical = false;    // Binary space partitions can only be split horizontal or vertical
            public int m_depth = 0;

            public BSPTreeNode2D(BSPTreeNode2D parent, Aero.Algorithms.Rect rect, int depth = 0) : base(parent)
            {
                m_rect = rect;
                m_depth = depth;
            }

            public void Split(bool splitVertical, float position)
            {
                m_splitVertical = splitVertical;
                if(splitVertical)
                {
                    // These Rects must be labled as our Aero.Algorithms.Rect otherwise Unity hijacks them
                    //  without telling us.
                    m_children.Add(new BSPTreeNode2D(this, new Aero.Algorithms.Rect(m_rect.left, m_rect.right, m_rect.top, position), m_depth + 1));
                    m_children.Add(new BSPTreeNode2D(this, new Aero.Algorithms.Rect(m_rect.left, m_rect.right, position, m_rect.bottom), m_depth + 1));
                }
                else
                {
                    m_children.Add(new BSPTreeNode2D(this, new Aero.Algorithms.Rect(m_rect.left, position, m_rect.top, m_rect.bottom), m_depth + 1));
                    m_children.Add(new BSPTreeNode2D(this, new Aero.Algorithms.Rect(position, m_rect.right, m_rect.top, m_rect.bottom), m_depth + 1));
                }
            }
        }

        // Binary Spatial Partition - Divides a 2D space iteratively until the space cannot be divided
        //                              smaller than the minimum width or height. The partition will be
        //                              divided such that the difference of the partition width with the
        //                              minimum width is compared to the difference of the partition
        //                              difference of the partition height and the minimum height. This
        //                              should prevent oddities such as long, skinny areas.
        // Inputs:
        //  root - A root node containing the entire area to be divided
        //  evenlyDivide - Should the divisions be divided exactly in half? Or should there be some nuance?
        //  minWidth - the minimum width for a partition
        //  minHeight - the minimum height for a partition
        // Returns:
        //  A list of hash sets with each item in the list representing a layer from the graph. 0 is the root,
        //      list size - 1 is the bottom-most leaves.

        // TODO: This function needs refactoring
        public static List<HashSet<BSPTreeNode2D>> BinarySpacePartition(BSPTreeNode2D root, bool evenDividePartitions, float snapSize = 0, float minWidth = 0, float minHeight = 0)
        {
            List<HashSet<BSPTreeNode2D>> graph = new List<HashSet<BSPTreeNode2D>>();
            graph.Add(new HashSet<BSPTreeNode2D>());
            graph[0].Add(root);
            List<float> horizontalSplits = new List<float>();
            List<float> verticalSplits = new List<float>();

            Queue<BSPTreeNode2D> splittableNodes = new Queue<BSPTreeNode2D>();
            splittableNodes.Enqueue(root);

            float halfSnapSize = snapSize / 2f;

            while(splittableNodes.Count > 0)
            {
                BSPTreeNode2D node = splittableNodes.Dequeue();

                int nextDepth = node.m_depth + 1;
                float partitionWidth = node.m_rect.right - node.m_rect.left;
                float partitionHeight = node.m_rect.top - node.m_rect.bottom;

                // if the partition width is more than the height, split horizontally
                if(partitionWidth - minWidth >= partitionHeight - minHeight)
                {
                    // If the width is less than twice the minimum width (meaning we can't
                    //  make 2 partitions without going below the minimum width) we are done
                    //  with this node.
                    if (partitionWidth < minWidth * 2f)
                        continue;

                    float splitPosition = 0;
                    if (!evenDividePartitions)
                        splitPosition = Aero.Random.rng.Range(node.m_rect.left + minWidth, node.m_rect.right - minWidth);
                    else
                        splitPosition = node.m_rect.left + (partitionWidth / 2f);

                    if (snapSize > 0)
                    {
                        foreach (var split in horizontalSplits)
                        {
                            if (Math.Abs(split - splitPosition) < snapSize)
                            {
                                splitPosition = split;
                                break;
                            }
                        }

                        if (!horizontalSplits.Contains(splitPosition))
                            horizontalSplits.Add(splitPosition);
                    }

                    node.Split(false, splitPosition);
                }
                // else split vertically
                else
                {
                    if (partitionHeight < minHeight * 2f)
                        continue;

                    float splitPosition = 0;
                    if (!evenDividePartitions)
                        splitPosition = Aero.Random.rng.Range(node.m_rect.bottom + minHeight, node.m_rect.top - minHeight);
                    else
                        splitPosition = node.m_rect.bottom + (partitionHeight / 2f);

                    if (snapSize > 0)
                    {
                        foreach (var split in verticalSplits)
                        {
                            if (Math.Abs(split - splitPosition) < snapSize)
                            {
                                splitPosition = split;
                                break;
                            }
                        }

                        if (!verticalSplits.Contains(splitPosition))
                            verticalSplits.Add(splitPosition);
                    }

                    node.Split(true, splitPosition);
                }

                if (nextDepth == graph.Count)
                {
                    graph.Add(new HashSet<BSPTreeNode2D>());
                }

                graph[nextDepth].Add((BSPTreeNode2D)node.m_children[0]);
                graph[nextDepth].Add((BSPTreeNode2D)node.m_children[1]);

                splittableNodes.Enqueue((BSPTreeNode2D)node.m_children[0]);
                splittableNodes.Enqueue((BSPTreeNode2D)node.m_children[1]);
            }

            return graph;
        }
    }
}
