using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld
{
    public class SCR_PlayerOverworldMovement : MonoBehaviour
    {
        [Header("PLAYER MOVEMENT PROPERTIES")]
        [SerializeField] [Range(1, 20)] float _movementSpeed = 10f;
        [SerializeField] bool _currentlyMoving = false;
        
        SCR_GraphNode _graphNode;
        SCR_PlayerInputManager _playerInputManager;
        public SCR_GraphNode GraphNode => _graphNode;
        public void SetGraphNode(SCR_GraphNode graphNode) => _graphNode = graphNode;
        public void Start()
        {
            _playerInputManager = SCR_GeneralManager.PlayerInputManager;
            Collider2D[] allColliders = Physics2D.OverlapPointAll(transform.position);
            foreach (var collider in allColliders)
            {
                SCR_GraphNode graphNode = collider.GetComponent<SCR_GraphNode>();
                if (graphNode != null) { _graphNode = graphNode; break; }
            }

            if (_graphNode == null)
            {
                _graphNode = FindFirstObjectByType<SCR_GraphNode>();
            }
        }

        public void InitialiseCurrentNode(SCR_GraphNode graphNode) => this._graphNode = graphNode;
        // Update is called once per frame
        void Update()
        {
            if (_currentlyMoving) { return; }
            iInteractable interactable = _graphNode?.GetComponent<iInteractable>() ?? null;
            if (_playerInputManager.Submit.PressedThisFrame() && interactable != null)
            {
                interactable.Interact(this);
            }

            if (!_playerInputManager.Axis2D.IsPressed()) { return; }

            //If player pressed right button
            if (_playerInputManager.Axis2D.AxisValue.x > 0 && _graphNode.HasDirection(SCR_GraphNode.GraphNode.Direction.EAST))
            {
                StartCoroutine(MovementCoroutine(SCR_GraphNode.GraphNode.Direction.EAST));
            }
            //If player pressed left button
            if (_playerInputManager.Axis2D.AxisValue.x < 0 && _graphNode.HasDirection(SCR_GraphNode.GraphNode.Direction.WEST))
            {
                StartCoroutine(MovementCoroutine(SCR_GraphNode.GraphNode.Direction.WEST));
            }
            //If player pressed up button
            if (_playerInputManager.Axis2D.AxisValue.y > 0 && _graphNode.HasDirection(SCR_GraphNode.GraphNode.Direction.NORTH))
            {
                StartCoroutine(MovementCoroutine(SCR_GraphNode.GraphNode.Direction.NORTH));
            }
            //If player pressed down button
            if (_playerInputManager.Axis2D.AxisValue.y < 0 && _graphNode.HasDirection(SCR_GraphNode.GraphNode.Direction.SOUTH))
            {
                StartCoroutine(MovementCoroutine(SCR_GraphNode.GraphNode.Direction.SOUTH));
            }
        }

        IEnumerator MovementCoroutine(SCR_GraphNode.GraphNode.Direction direction)
        {
            _currentlyMoving = true;
            yield return new WaitForSeconds(0.25f);
            
            SCR_GraphNode.GraphNode graphNode = _graphNode.GetNode(direction);
            SCR_GraphNode adjacentGraphNode = graphNode.AdjacentNode;

            if (adjacentGraphNode == null)
            {
                Debug.LogWarning("THERE IS NO ADJACENT NODE PRESENT!");
                _currentlyMoving = false;
                yield break;
            }

            if (graphNode.ConditionalNode && !_graphNode.ConditionalNode())
            {
                _currentlyMoving = false;
                yield break;
            }
            _graphNode.OnPlayerMoved(this);
            _graphNode = adjacentGraphNode;

 
            while ((transform.position - _graphNode.transform.position).sqrMagnitude > 0.25f)
            {
                transform.position = Vector3.Lerp(transform.position, _graphNode.transform.position, Time.deltaTime * _movementSpeed);
                yield return null;

            }
            transform.position = _graphNode.transform.position;
            _currentlyMoving = false;
            _graphNode.OnPlayerLanded(this);
            yield break;
        }
    }

}