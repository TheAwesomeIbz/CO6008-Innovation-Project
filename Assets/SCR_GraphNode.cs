using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class SCR_GraphNode : MonoBehaviour
    {
        [Header("GRAPH NODE PROPERTIES")]
        [SerializeField] private List<GraphNode> graphNodes;

        void Start()
        {
            if (graphNodes == null) { graphNodes = new List<GraphNode>(); }
        }

        public bool HasDirection(GraphNode.Direction direction) => graphNodes.Find(pr => pr.ValidDirection == direction) != null;

        public GraphNode GetNode(GraphNode.Direction direction) => graphNodes.Find(pr => pr.ValidDirection == direction);
        public GraphNode GetFirstNode() => graphNodes[0];

        public virtual void OnPlayerMoved(SCR_PlayerOverworldMovement playerOverworldMovement)
        {

        }

        public virtual void OnPlayerLanded(SCR_PlayerOverworldMovement playerOverworldMovement)
        {

        }



        /// <summary>
        /// Structure used to store information about graph nodes and where the player can move to.
        /// </summary>
        [System.Serializable]
        public class GraphNode
        {
            public enum Direction
            {
                NORTH,
                SOUTH,
                EAST,
                WEST
            }

            public Direction ValidDirection;
            public SCR_GraphNode AdjacentNode;
        }
    }

}