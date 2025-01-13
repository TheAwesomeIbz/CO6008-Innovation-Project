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

        /// <summary>
        /// Determines whether a node contains a valid direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool HasDirection(GraphNode.Direction direction) => graphNodes.Find(pr => pr.ValidDirection == direction) != null;

        /// <summary>
        /// Find first node based off of direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns>A node whether it exists in the graphNode array</returns>
        public GraphNode GetNode(GraphNode.Direction direction) => graphNodes.Find(pr => pr.ValidDirection == direction);

        /// <summary>
        /// Called when the player moved from this graph node
        /// </summary>
        /// <remarks>
        /// Virtual function that can be overriden in inherited classes
        /// </remarks>
        /// <param name="playerOverworldMovement">Player overworld movement reference</param>
        public virtual void OnPlayerMoved(SCR_PlayerOverworldMovement playerOverworldMovement)
        {

        }

        /// <summary>
        /// Called when the player moved onto this graph node
        /// </summary>
        /// <remarks>
        /// Virtual function that can be overriden in inherited classes
        /// </remarks>
        /// <param name="playerOverworldMovement">Player overworld movement reference</param>
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