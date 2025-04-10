using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public class SCR_EntityMovement : MonoBehaviour
    {
        [SerializeField] private Vector3 minimumPosition, maximumPosition;
        [SerializeField] private float movementSpeed = 3.0f;
        [SerializeField] private PositionState positionState;
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            switch (positionState)
            {
                case PositionState.TRAVEL_TO_MINIMUM:
                    transform.position = Vector2.Lerp(transform.position, minimumPosition, movementSpeed * Time.deltaTime);
                    if ((transform.position - minimumPosition).sqrMagnitude < 2)
                    {
                        positionState = PositionState.TRAVEL_TO_MAXIMUM;
                    }
                    break;
                case PositionState.TRAVEL_TO_MAXIMUM:
                    transform.position = Vector2.Lerp(transform.position, maximumPosition, movementSpeed * Time.deltaTime);
                    if ((transform.position - maximumPosition).sqrMagnitude < 2)
                    {
                        positionState = PositionState.TRAVEL_TO_MINIMUM;
                    }
                    break;
            }
            
        }

        enum PositionState
        {
            TRAVEL_TO_MINIMUM,
            TRAVEL_TO_MAXIMUM,
        }
    }
}
