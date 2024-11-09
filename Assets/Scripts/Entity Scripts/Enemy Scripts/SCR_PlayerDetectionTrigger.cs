using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Enemies
{
    public class SCR_PlayerDetectionTrigger : MonoBehaviour
    {
        public event System.Action<Player.SCR_PlayerMovement> OnPlayerDetected;
        public void InitializeCollider(float radius)
        {
            CircleCollider2D circleCollider2D = gameObject.AddComponent<CircleCollider2D>();
            circleCollider2D.radius = radius;
            circleCollider2D.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetType(out Player.SCR_PlayerMovement playerMovement) == null) { return; }
            OnPlayerDetected?.Invoke(playerMovement);
        }
    }
}
