using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level.Tutorial
{
    public class SCR_ShootingPractice : MonoBehaviour
    {
        [SerializeField] PolygonCollider2D cameraView;
        Camera mainCamera;
        void Start()
        {
            mainCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {

        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            
        }
    }

}