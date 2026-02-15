using System;
using System.Collections.Generic;
using DataStructure;
using UnityEngine;

public class RedBlackRenderer : MonoBehaviour
{
    [SerializeField] private RBGhostNode ghostPrefab;
    [SerializeField] private RBVisualNode nodePrefab;
    [SerializeField] private float spacingY = 1f;
    [SerializeField] private float spacingX = 1f;
    [SerializeField] private float size = 1f;
    [SerializeField] private bool rerender = false;

    [SerializeField] private int testRenderDepth = 5;

    [SerializeField] private RBGhostNode ghostRoot;
    [SerializeField] private int currentDepth;
    
    private readonly RBTree _tree = new();
    
    private List<RBGhostNode> _ghostNodes = new();
    private List<RBVisualNode> _nodes = new();

    private void OnValidate()
    {
        #if UNITY_EDITOR
        if (rerender)
        {
            Render();
            rerender = false;
        }
        #endif
    }

    private void Awake()
    {
        currentDepth = Int32.MinValue;
    }

    public void Insert(int key)
    {
        _tree.Insert(key);
        Debug.Log(_tree);
        Render();
    }

    public void Delete(int key)
    {
        _tree.LazyDelete(key);
        Debug.Log(_tree);
        Render();
    }


    public void Render()
    {
        
        int depth = _tree.GetMaxDepth();
        RenderEmptyTreeOfDepth(depth);
        
        Queue<RBNode> queue = new Queue<RBNode>();
        Queue<RBGhostNode> positionQueue = new Queue<RBGhostNode>();
        
        queue.Enqueue(_tree.Root);
        positionQueue.Enqueue(ghostRoot);
        
        while (queue.Count > 0)
        {
            RBNode node = queue.Dequeue();
            RBGhostNode position = positionQueue.Dequeue();

            if (node == null)
            {
                continue;
            }
            
            RBVisualNode visual = FindNode(node.Key);

            if (!visual)
            {
                visual = CreateNode(position, node);
                _nodes.Add(visual);
            }
            else
            {
                visual.SetColor(node.IsRed);
                visual.GoTo(position.transform.position);
                Vector3 parentPosition = position.Parent ? position.Parent.transform.position : position.transform.position;
                visual.SetParentPos(parentPosition);
                visual.SetNil(node.IsNil);
            }
            
            
            queue.Enqueue(node.Left);
            queue.Enqueue(node.Right);
            positionQueue.Enqueue(position.Left);
            positionQueue.Enqueue(position.Right);
        }

        if (_tree.Size < _nodes.Count)
        {
            for (int i = _nodes.Count - 1; i >= 0; i--)
            {
                if (_nodes[i].IsNil)
                {
                    Destroy(_nodes[i].gameObject);
                    _nodes.RemoveAt(i);
                }
            }
        }
    }

    private void RenderEmptyTreeOfDepth(int depth)
    {
        if (depth == currentDepth)
        {
            return;
        }
        foreach (var node in _ghostNodes)
        {
            Destroy(node.gameObject);
        }
        _ghostNodes.Clear();
        ghostRoot = null;
        
        List<RBGhostNode> lastLayer = new();
        int numOfNodes = Mathf.RoundToInt(Mathf.Pow(2, depth - 1));
        
        float initialX = - (numOfNodes - 1) * spacingX / 2f;
        float initialY = - (depth - 1) * spacingY / 2f;
        Vector2 pos = new Vector2(initialX, initialY);
        
        
        
        for (int i = 0; i < numOfNodes; i++)
        {
            RBGhostNode ghostNode = CreateGhostNode(pos, $"layer {0} # {i}");
            _ghostNodes.Add(ghostNode);
            lastLayer.Add(ghostNode);
            pos.x += spacingX;
        }

        
        for (int d = 1; d < depth; d++)
        {
            float yPos = initialY + spacingY * d;
            List<RBGhostNode> layer = new();
            for (int i = 0; i < lastLayer.Count; i += 2)
            {
                RBGhostNode ghostNodeLeft = lastLayer[i];
                RBGhostNode ghostNodeRight = lastLayer[i + 1];
                
                float deltaX = ghostNodeRight.transform.position.x - ghostNodeLeft.transform.position.x;
                float xPos = ghostNodeLeft.transform.position.x + deltaX/2;
                
                pos = new Vector2(xPos, yPos);
                RBGhostNode ghostNode = CreateGhostNode(pos, $"layer {d} # {i}");
                //link nodes
                ghostNode.Left = ghostNodeLeft;
                ghostNode.Right = ghostNodeRight;
                ghostNodeLeft.Parent = ghostNode;
                ghostNodeRight.Parent = ghostNode;
                
                _ghostNodes.Add(ghostNode);
                layer.Add(ghostNode);
            }
            lastLayer = layer;
        }

        ghostRoot = _ghostNodes.Count == 0 ? null : _ghostNodes[^1];
        currentDepth = depth;
    }

    public RBVisualNode CreateNode(RBGhostNode ghostNode, RBNode node)
    {
        RBVisualNode visualNode = Instantiate(nodePrefab, ghostNode.transform.position, Quaternion.identity);
        
        Vector3 parentPosition = ghostNode.Parent ? ghostNode.Parent.transform.position : ghostNode.transform.position;
        visualNode.Init(node.IsRed, node.Key, size, this);
        visualNode.SetParentPos(parentPosition);
        
        return visualNode;
    }
    
    
    public RBGhostNode CreateGhostNode(Vector3 position, string id)
    {
        RBGhostNode ghostNode = Instantiate(ghostPrefab, position, Quaternion.identity);
        ghostNode.name = id;
        
        return ghostNode;
    }

    public RBVisualNode FindNode(int key)
    {
        foreach (var node in _nodes)
        {
            if (node.Key == key)
            {
                return node;
            }
        }
        
        return null;
    }
}
