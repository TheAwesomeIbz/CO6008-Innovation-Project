using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public class SCR_ParallaxLayer : MonoBehaviour
    {
        Camera currentCamera;
        [Header("PARALLAX PROPERTIES")]
        [SerializeField] [Range(0, 5)] float parallaxFactor;
        [SerializeField] int verticalParallaxQuotient = 16;
        Vector2 originalPosition;
        void Start()
        {
            originalPosition = transform.position;
            currentCamera = Camera.main;
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            transform.position = originalPosition + CalculateParallaxPosition();
        }

        private Vector2 CalculateParallaxPosition()
        {
            float parallax_X = currentCamera.transform.position.x * (1 - parallaxFactor);
            float parallax_Y = currentCamera.transform.position.y * (1 - parallaxFactor / verticalParallaxQuotient);

            return new Vector2(parallax_X, parallax_Y);
        }
    }
}
