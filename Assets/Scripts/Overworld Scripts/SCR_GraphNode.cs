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
        const int C_GraphNodeRaycastLength = 3;
        [Header("GRAPH NODE PROPERTIES")]
        [SerializeField] protected bool calculateAdjacentNodes = true;
        [SerializeField] protected List<GraphNode> graphNodes;

        public IReadOnlyList<GraphNode> GetGraphNodes => graphNodes;

        /// <summary>
        /// Virtual method that allows the player to move to any conditional nodes if this condition is met.
        /// This can be overriden in inherited classes.
        /// </summary>
        /// <returns>Whether thhe player can move to the adjacent node or not</returns>
        public virtual bool ConditionalNode()
        {
            return false;
        }
        
        /// <summary>
        /// Initialises all graph nodes by dynamically scanning for nodes in all valid directions.
        /// </summary>
        private void InitialiseGraphNodes()
        {
            Func<GraphNode.Direction, Vector2> getDirection = (direction) =>
            {
                return direction switch
                {
                    GraphNode.Direction.NORTH => Vector2.up,
                    GraphNode.Direction.SOUTH => Vector2.down,
                    GraphNode.Direction.EAST => Vector2.right,
                    GraphNode.Direction.WEST => Vector2.left,
                    _ => Vector2.zero,
                };
            };

            for (int i = 0; i < 4; i++)
            {
                GraphNode.Direction currentDirection = (GraphNode.Direction)i;
                RaycastHit2D[] graphNodeRaycast = Physics2D.RaycastAll(transform.position, getDirection(currentDirection), C_GraphNodeRaycastLength, LayerMask.GetMask("Graph Nodes"));
                foreach (RaycastHit2D raycastHit in graphNodeRaycast)
                {
                    bool validGraphNode = raycastHit.collider != null && raycastHit.collider.name != name;
                    
                    if (!validGraphNode) { continue; }
                    GraphNode existingGraphNode = graphNodes.Find(gn => gn.ValidDirection == currentDirection);
                    if (existingGraphNode != null && existingGraphNode.AdjacentNode == null) {
                        existingGraphNode.AdjacentNode = raycastHit.collider.GetComponent<SCR_GraphNode>();
                        continue;
                    }

                    GraphNode graphNode = new GraphNode
                    {
                        AdjacentNode = raycastHit.collider.GetComponent<SCR_GraphNode>(),
                        ValidDirection = currentDirection,
                        ConditionalNode = false
                    };

                    graphNodes.Add(graphNode);
                }
                
            }
        }
        void Awake()
        {
            if (!calculateAdjacentNodes) { return; }
            if (graphNodes.Count == 0) { graphNodes = new List<GraphNode>(); }
            InitialiseGraphNodes();
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