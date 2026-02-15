using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DataStructure
{
    public class RBTree
    {
        public static RBTree DeepCopy(RBTree tree)
        {
            RBTree copyTree = new RBTree();
            
            var root = tree.Root;
            copyTree.Root = new RBNode(root.Parent, root.Key, root.IsRed);
            
            Queue<RBNode> originalQ = new Queue<RBNode>();
            Queue<RBNode> copyQ = new Queue<RBNode>();
            
            originalQ.Enqueue(root);
            copyQ.Enqueue(copyTree.Root);

            while (originalQ.Count > 0)
            {
                RBNode original = originalQ.Dequeue();
                RBNode copy = copyQ.Dequeue();

                if (original == null)
                {
                    continue;
                }
                
                var left = original.Left;
                var right = original.Right;
                
                copy.Left = new RBNode(copy, left);
                copy.Right = new RBNode(copy, right);
                
                originalQ.Enqueue(copy.Left);
                originalQ.Enqueue(copy.Right);
                
                copyQ.Enqueue(copy.Left);
                copyQ.Enqueue(copy.Right);
            }
            
            copyTree.Size = tree.Size;
            copyTree._deletedCount = tree._deletedCount;
            
            return copyTree;
        }
        
        public int Size { get; private set; }
        public RBNode Root {get; private set; }
        private int _deletedCount;
        
        
        public void Insert(int key)
        {
            if (Root == null)
            {
                Root = new RBNode(null, key, false);
                Size++;
                return;
            }

            RBNode inserted = null;
            RBNode current = Root;

            while (current != null)
            {
                if (key < current.Key)
                {
                    inserted = current;
                    current = current.Left;
                }
                else if (key > current.Key)
                {
                    inserted = current;
                    current = current.Right;
                }
                else
                {
                    current.IsNil = false;
                    _deletedCount--;
                    return;
                }
            }

            if (inserted == null)
            {
                return;
            }

            if (key < inserted.Key)
            {
                inserted.Left = new RBNode(inserted, key);
                inserted = inserted.Left;
            }
            else if (key > inserted.Key)
            {
                inserted.Right = new RBNode(inserted, key);
                inserted = inserted.Right;
            }
            
            FixTree(inserted);
            Size++;
        }

        public void LazyDelete(int key)
        {
            if (!TryGetKey(key, out var target))
            {
                return;
            }
            
            target.IsNil = true;
            _deletedCount++;

            if (_deletedCount * 2 > Size)
            {
                RebuildTree();
                _deletedCount = 0;
            }
        }

        private void RebuildTree()
        {
            // Deconstruct
            List<RBNode> sortedNodes = new List<RBNode>();
            ToSortedList(sortedNodes, Root);
            Size = 0;
            BuildRedBlackTree(sortedNodes);
        }

        private void BuildRedBlackTree(List<RBNode> sortedNodes)
        {
            for (int i = sortedNodes.Count - 1; i >= 0; i--)
            {
                if (sortedNodes[i].IsNil)
                {
                    sortedNodes.RemoveAt(i);
                }
            }
            
            int mid = sortedNodes.Count / 2;
            Root = sortedNodes[mid];
            Size++;
            Root.Left = BuildSubTree(sortedNodes, Root, 0, mid - 1);
            Root.Right = BuildSubTree(sortedNodes, Root, mid + 1, sortedNodes.Count - 1);

            Root.IsRed = false;
        }

        private RBNode BuildSubTree(List<RBNode> sortedNodes, RBNode parent, int start, int end)
        {
            if (start > end)
            {
                return null;
            }
            
            if (start == end)
            {
                sortedNodes[start].Left = null;
                sortedNodes[start].Right = null;
                sortedNodes[start].Parent = parent;
                sortedNodes[start].IsRed = true;
                Size++;
                return sortedNodes[start];
            }
            int mid = start + (end - start) / 2;
            
            var current = sortedNodes[mid];
            Size++;
            current.IsRed = false;
            
            current.Left = BuildSubTree(sortedNodes, current, start, mid - 1);
            current.Right = BuildSubTree(sortedNodes, current, mid + 1, end);
            
            //link back to parent

            current.Parent = parent;
            
            return current;
        }

        private void ToSortedList(List<RBNode> result, RBNode node)
        {
            if (node.Left == null && node.Right == null)
            {
                result.Add(node);
                return;
            }

            if (node.Left != null)
            {
                ToSortedList(result, node.Left);
            }
            
            result.Add(node);

            if (node.Right != null)
            {
                ToSortedList(result, node.Right);
            }
        }

        public bool TryGetKey(int key, out RBNode result)
        {
            var current = Root;
            
            while (current != null && key != current.Key)
            {
                current = key < current.Key ? current.Left : current.Right;
            }

            result = current;

            return result != null;
        }

        private void FixTree(RBNode node)
        {
            while (node.Parent is { IsRed: true })
            {
                if (node.Parent == node.Parent.Parent.Left)
                {
                    var uncle = node.Parent.Parent.Right;

                    if (uncle is {IsRed: true})
                    {
                        uncle.IsRed = false;
                        node.Parent.IsRed = false;
                        node.Parent.Parent.IsRed = true;
                        node = node.Parent.Parent;
                    }
                    else
                    {
                        if (node == node.Parent.Right)
                        {
                            node = node.Parent;
                            RotateLeft(node);
                        }

                        node.Parent.IsRed = false;
                        node.Parent.Parent.IsRed = true;
                        RotateRight(node.Parent.Parent);
                    }
                }
                else
                {
                    var uncle = node.Parent.Parent.Left;

                    if (uncle is { IsRed: true })
                    {
                        uncle.IsRed = false;
                        node.Parent.IsRed = false;
                        node.Parent.Parent.IsRed = true;
                        node = node.Parent.Parent;
                    }
                    else
                    {
                        if (node == node.Parent.Left)
                        {
                            node = node.Parent;
                            RotateRight(node);
                        }

                        node.Parent.IsRed = false;
                        node.Parent.Parent.IsRed = true;
                        RotateLeft(node.Parent.Parent);
                        
                    }
                }
            }

            Root.IsRed = false;
        }

        private void RotateRight(RBNode node)
        {
            //Left Node becomes parent
            var parent = node.Parent;
            var newRoot = node.Left;
            var replaceBranch = node.Left.Right;
            
            

            if (parent == null)
            {
                Root = newRoot;
            }
            else if (newRoot.Key > parent.Key)
            {
                parent.Right = newRoot;
            }
            else
            {
                parent.Left = newRoot;
            }
            newRoot.Parent = parent;
            
            newRoot.Right = node;
            node.Parent = newRoot;
            
            node.Left = replaceBranch;
            if (replaceBranch != null)
            {
                replaceBranch.Parent = node;
            }
        }

        private void RotateLeft(RBNode node)
        {
            var parent = node.Parent;
            var newRoot = node.Right;
            var replaceBranch = node.Right.Left;
            
            if (parent == null)
            {
                Root = newRoot;
            }
            else if (newRoot.Key > parent.Key)
            {
                parent.Right = newRoot;
            }
            else
            {
                parent.Left = newRoot;
            }
            newRoot.Parent = parent;
            
            newRoot.Left = node;
            node.Parent = newRoot;
            
            node.Right = replaceBranch;

            if (replaceBranch != null)
            {
                replaceBranch.Parent = node;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            List<RBNode> sortedNodes = new List<RBNode>();
            ToSortedList(sortedNodes, Root);
            sb.Append("[");
            foreach (var node in sortedNodes)
            {
                if (node.IsNil)
                    continue;
                
                sb.Append(node);
                sb.Append(", ");
            }
            sb.Append("]");
            return sb.ToString();
        }

        public int GetMaxDepth()
        {
            return GetDepth(Root);
        }
        
        private int GetDepth(RBNode node)
        {
            if (node == null)
            {
                return 0;
            }
            
            return 1 + Mathf.Max(GetDepth(node.Left), GetDepth(node.Right));
        }
    }
    
    
}
