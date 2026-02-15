namespace DataStructure
{
    public class RBNode
    {
        public RBNode Parent { get; set; }
        public RBNode Left { get; set; }
        public RBNode Right { get; set; }

        public int Key { get; set; }
        public bool IsRed { get; set; }
        public bool IsNil { get; set; }

        public RBNode (RBNode parent, int key, bool isRed = true)
        {
            Parent = parent;
            IsRed = isRed;
            Key = key;
        }
        
        public RBNode (RBNode parent, RBNode copy)
        {
            Parent = parent;
            
            IsRed = copy.IsRed;
            Key = copy.Key;
            IsNil = copy.IsNil;
        }

        public override string ToString()
        {
            return Key.ToString();
        }
    }
}
