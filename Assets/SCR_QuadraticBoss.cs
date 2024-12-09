using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Boss
{
    public class SCR_QuadraticBoss : SCR_LinearBoss
    {
        [Header("OVERRIDEN THIRD PHASE PROPERTIES")]
        [SerializeField] Transform equationParentObject;
        SCR_EquationRenderer _equationRenderer;
        [SerializeField] float _quadraticSpeed = 7;

        [Header("FOURTH PHASE COROUTINE")]
        [SerializeField] float maximumTimeForWave = 20;

        protected override void Start()
        {
            base.Start();
            _equationRenderer = equationParentObject.GetComponentInChildren<SCR_EquationRenderer>();
            _equationRenderer.ParentObject.gameObject.SetActive(false);

            _bossPhaseActions.Add(FourthPhase);
            
        }
        protected override void ThirdPhase()
        {
            _inAttackPhase = true;
            _equationRenderer.SetGlobalPosition(Vector3.zero);
            StartCoroutine(ThirdPhaseCoroutine());
            StartCoroutine(QuadraticMovement());
        }

        private IEnumerator QuadraticMovement()
        {
            bool strictlyIncreasing = true;
            float initialValue = 0;
            _movementCounter = initialValue;
            while (_inAttackPhase)
            {

                _movementCounter += (strictlyIncreasing ? 1 : -1) * Time.deltaTime * _bossSpeed;
                if (_movementCounter > Mathf.PI / _bossSpeed)
                {
                    strictlyIncreasing = false;
                }
                if (_movementCounter < initialValue)
                {
                    strictlyIncreasing = true;
                }
                transform.position = new Vector3(Mathf.Cos(_movementCounter * _bossSpeed) * _horizonatalMultipier * 2,
                    -Mathf.Sin(_movementCounter * _bossSpeed) * 3) + _defaultPosition;

                yield return null;
            }
        }
        protected override IEnumerator ThirdPhaseCoroutine()
        {
            float initialValue = 0;
            _movementCounter = initialValue;

            _equationRenderer.ParentObject.gameObject.SetActive(true);
            _equationRenderer.SetPolynomialCoefficients(new float[] { 0.5f, 0, 0 });

            yield return _equationRenderer.DisplayGraphCoroutine(1 * _bossSpeed);

            
            bool moveRight = Random.Range(0, 20) % 2 == 0;

            while (equationParentObject.transform.position.y < 2)
            {
                equationParentObject.transform.position += new Vector3(moveRight ? (Time.deltaTime * -_quadraticSpeed / 8) : (Time.deltaTime * _quadraticSpeed / 8), Time.deltaTime * _bossSpeed);
                yield return null;
            }

            if (!moveRight)
            {
                while (equationParentObject.transform.position.x < 23)
                {
                    equationParentObject.transform.position += new Vector3(Time.deltaTime * _quadraticSpeed, Time.deltaTime * -_quadraticSpeed * _bossSpeed) ;
                    yield return null;
                }
            }
            else
            {
                while (equationParentObject.transform.position.x > -23)
                {
                    equationParentObject.transform.position -= new Vector3(Time.deltaTime * _quadraticSpeed, Time.deltaTime * _quadraticSpeed * _bossSpeed);
                    yield return null;
                }
            }
            

            _localPhasePasses = 0;
            _equationRenderer.ParentObject.gameObject.SetActive(false);
            OnLocalPhaseCompleted();
        }

        private void FourthPhase()
        {
            _inAttackPhase = true;

            _entityShooting.SetShootingStyle(SCR_EntityShooting.ShootingVariant.CIRCULAR);
            _entityShooting.SetCooldownPeriod(1 / _bossSpeed);

            StopAllCoroutines();
            StartCoroutine(FourthPhaseCoroutine());
            StartCoroutine(QuadraticMovement());
        }

        IEnumerator FourthPhaseCoroutine()
        {
            float counter = 0;
            while (counter < maximumTimeForWave)
            {
                counter += Time.deltaTime;
                _entityShooting.EntityShootingUpdate(_playerMovementReference.transform);
                yield return null;
            }

            _localPhasePasses = 0;
            OnLocalPhaseCompleted();
        }
    }
}
