using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Enemies
{
    public class SCR_PlayerDetectionTrigger : MonoBehaviour
    {

        public event System.Action<Player.SCR_PlayerMovement, bool> OnPlayerDetected;
        public void InitializeCollider(float radius)
        {
            CircleCollider2D circleCollider2D = gameObject.AddComponent<CircleCollider2D>();
            circleCollider2D.radius = radius;
            circleCollider2D.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetType(out Player.SCR_PlayerMovement playerMovement) == null) { return; }

            GetComponent<CircleCollider2D>().radius *= 2;
            OnPlayerDetected?.Invoke(playerMovement, true);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetType(out Player.SCR_PlayerMovement playerMovement) == null) { return; }

            GetComponent<CircleCollider2D>().radius /= 2;
            OnPlayerDetected?.Invoke(playerMovement, false);
        }
    }
}
