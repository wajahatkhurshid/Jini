using System;

namespace Gyldendal.Jini.Services.Contracts
{
    public class GpmNode : IEquatable<GpmNode>
    {
        public int NodeId { get; set; }

        public string Name { get; set; }

        public bool Equals(GpmNode node)
        {
            if (node == null)
            {
                return false;
            }

            return NodeId == node.NodeId && Name.ToLower().Equals(node.Name.ToLower());
        }

        public override int GetHashCode()
        {
            var hashNodeId = NodeId.GetHashCode();
            var hashName = Name == null ? 0 : Name.GetHashCode();

            return hashNodeId ^ hashName;
        }
    }
}
