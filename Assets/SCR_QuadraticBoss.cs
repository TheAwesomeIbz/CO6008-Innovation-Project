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
        [SerializeField] float _quadraticSpeed = 12.5f;

        [Header("FOURTH PHASE COROUTINE")]
        [SerializeField] float _bossShootingCooldown = 1.5f;
        [SerializeField] float maximumTimeForWave = 1f;

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
            StopAllCoroutines();
            StartCoroutine(ThirdPhaseCoroutine());
            StartCoroutine(QuadraticMovement());
        }

        /// <summary>
        /// Movement coroutine that moves the boss in a quadratic graph fashion and stops when the attacking phase is over.
        /// </summary>
        /// <returns></returns>
        private IEnumerator QuadraticMovement()
        {
            yield return MoveToPosition(new Vector3(Mathf.Cos(_movementCounter * _bossSpeed) * _horizonatalMultipier * 2,
                    -Mathf.Sin(_movementCounter * _bossSpeed) * 3) + _defaultPosition);

            bool strictlyIncreasing = true;
            float initialValue = 0;
            _movementCounter = initialValue;
            while (_inAttackPhase)
            {
                //determine the state of strictlyIncreasion as a flag
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

            yield return MoveToPosition(_defaultPosition);
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

        /// <summary>
        /// Fourth phase of attacking that shoots out projectiles in a circular fashion
        /// </summary>
        private void FourthPhase()
        {
           
            _inAttackPhase = true;

            _entityShooting.SetShootingStyle(SCR_EntityShooting.ShootingVariant.CIRCULAR);
            _entityShooting.SetCooldownPeriod(_bossShootingCooldown / _bossSpeed);

            StopAllCoroutines();
            StartCoroutine(FourthPhaseCoroutine());
            StartCoroutine(QuadraticMovement());
        }

        /// <summary>
        /// Coroutine that handles shooting the projectiles from the boss for a fixed period of time, relative to the boss state
        /// </summary>
        /// <returns></returns>
        IEnumerator FourthPhaseCoroutine()
        {

            float counter = 0;
            while (counter < maximumTimeForWave / _bossSpeed)
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
