using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld
{
    /// <summary>
    /// Base monobehaviour that all overworld nodes would inherit from, providing functionality for the player to travel from one node to another
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public class SCR_GraphNode : MonoBehaviour
    {
        [Header("GRAPH NODE PROPERTIES")]
        [SerializeField] protected List<GraphNode> graphNodes;

        /// <summary>
        /// Virtual method that allows the player to move to any conditional nodes if this condition is met.
        /// This can be overriden in inherited classes.
        /// </summary>
        /// <returns>Whether thhe player can move to the adjacent node or not</returns>
        public virtual bool ConditionalNode()
        {
            return false;
        }
        
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
            /// <summary>
            /// Direction in which to travel to
            /// </summary>
            public Direction ValidDirection;
            public SCR_GraphNode AdjacentNode;

            /// <summary>
            /// Whether travelling to the adjacent node requires a condition to be met or not
            /// </summary>
            public bool ConditionalNode;

        }
    }

}