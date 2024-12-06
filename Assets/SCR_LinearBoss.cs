using Dialogue;
using Entities.Enemies;
using Entities.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;


namespace Entities.Boss
{
    public class SCR_LinearBoss : SCR_BossEntity
    {
        [Header("LINEAR BOSS PROPERTIES")]
        [SerializeField][Range(1, 3)] int _bossSpeed = 1;
        [SerializeField] float _horizonatalMultipier = 4;
        [SerializeField] float _verticalPeriodMultipler = 2;
        SCR_PlayerMovement _playerMovementReference;
        SCR_EntityShooting _entityShooting;
        CMP_HitboxComponent _hitboxComponent;

        [Header("PHASE PROPERTIES")]
        [SerializeField] bool _inAttackPhase;
        [SerializeField] int _bossPhase;
        [SerializeField] int _localPhasePasses;
        const int BossInterpolationSpeed = 5;


        [Header("FIRST PHASE PROPERTIES")]
        [SerializeField] GameObject _firstPhaseParentObject;
        [SerializeField] QuestionObject[] _questionObjects;
        FirstPhaseUIObjects _firstPhaseUIObjects;
        string _previousQuestionName;
        

        [Header("SECOND PHASE PROPERTIES")]
        [SerializeField] Transform _secondPhaseParentObject;

        List<ColliderZones> _colliderZones;
        Vector3 _defaultPosition;
        float _movementCounter;

        [Header("THIRD PHASE PROPERTIES")]
        [SerializeField] float _alternatingPeriod = 2;
        int[] _angleArray;
        float _cooldownPeriod = 0.125f;

        [Header("CUTSCENE PROPERTIES")]
        [SerializeField] DialogueObject[] _halfHPDialogue;
        bool _atHalfHP;
        bool _cutsceneActive;
        

        [Header("BOSS DEFEATED PROPERTIES")]
        [SerializeField] DialogueObject[] _defeatDialogue;
        bool _bossDefeated;
        public override CMP_HealthComponent BossHealthComponent => _hitboxComponent.HealthComponent;

        protected void Start()
        {
            _playerMovementReference = FindObjectOfType<SCR_PlayerMovement>();
            _entityShooting = GetComponent<SCR_EntityShooting>();
            _hitboxComponent = GetComponentInChildren<CMP_HitboxComponent>();
            _firstPhaseUIObjects = new FirstPhaseUIObjects(_firstPhaseParentObject);

            _hitboxComponent.OnZeroHPEvent += OnZeroHPEvent;
            _hitboxComponent.OnDamageEvent += OnDamageEvent;

            _inAttackPhase = false;
            _bossDefeated = false;
            _localPhasePasses = UnityEngine.Random.Range(2, 5);
            _defaultPosition = transform.position;

            _colliderZones = new List<ColliderZones>();
            _angleArray = new int[3];
            _previousQuestionName = "";
            for (int i = 0; i < _secondPhaseParentObject.childCount; i++){

                ColliderZones colliderZone = new ColliderZones(_secondPhaseParentObject.GetChild(i));
                colliderZone.SetWarningObjectActivity(false);
                colliderZone.SetConstantColliderActivity(false);
                _colliderZones.Add(colliderZone);

                Vector2 relativePosition = _defaultPosition - _secondPhaseParentObject.GetChild(i).transform.position;
                float relativeAngle = Mathf.Atan2(relativePosition.y, relativePosition.x);
                _angleArray[i] = Mathf.RoundToInt(-relativeAngle * Mathf.Rad2Deg);
                
            }
        }

        protected void Update()
        {
            if (_bossDefeated) { return; }
            if (_cutsceneActive) { return; }

            _bossSpeed = _hitboxComponent.HealthComponent.IsHalfHP ? 2 : 1;

            SecondPhaseBossMovement();



            if (_inAttackPhase) { return; }

            switch (_bossPhase)
            {
                case 0:
                    FirstPhase();
                    break;
                case 1:
                    SecondPhase();
                    break;
                case 2:
                    ThirdPhase();
                    break;
            }
            _localPhasePasses -= 1;
        }

        /// <summary>
        /// Called at the end of every local boss phase. Will dynamically set the amount of phase passes depending on the state of the boss.
        /// </summary>
        /// <remarks>Generates a random number depending on the state of the boss.</remarks>
        private void OnLocalPhaseCompleted()
        {
            if (_localPhasePasses <= 0)
            {
                int randomPhase = UnityEngine.Random.Range(0, 3);
                while (randomPhase == _bossPhase) { randomPhase = UnityEngine.Random.Range(0, 3); }

                _bossPhase = randomPhase;
                _localPhasePasses = _atHalfHP ?
                    UnityEngine.Random.Range(5, 10) :
                    UnityEngine.Random.Range(3, 5);
            }

            _inAttackPhase = false;
        }

        /// <summary>
        /// Function called the first time the boss enemy reaches half HP.
        /// </summary>
        /// <param name="obj"></param>
        private void OnDamageEvent(SCR_DamageCollider obj)
        {
            if (_hitboxComponent.HealthComponent.IsHalfHP && !_atHalfHP)
            {
                _atHalfHP = true;
                StopAllCoroutines();

                foreach (ColliderZones colliderZones in _colliderZones)
                {
                    colliderZones.SetParentObjectActivity(false);
                    colliderZones.SetTransitionState(false);
                }
                _movementCounter = MathF.PI / 2;
                _cutsceneActive = true;

                Action OnDialogueFinished = () =>
                {
                    _cutsceneActive = false;
                    _inAttackPhase = false;
                };
                StartCoroutine(HalfHPCoroutine(OnDialogueFinished));
            }

            
            IEnumerator HalfHPCoroutine(Action action)
            {
                while ((transform.position - _defaultPosition).magnitude > 0.25f)
                {
                    transform.position = Vector3.Lerp(transform.position, _defaultPosition, Time.deltaTime * 5);
                    yield return null;
                }

                SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(_halfHPDialogue, action);
            }
        }

        #region FIRST PHASE

        /// <summary>
        /// First phase of the boss rush that gets called within Update() that initializes the values for the first boss rush phase with a questinnaire
        /// </summary>
        private void FirstPhase()
        {
            _inAttackPhase = true;

            StopAllCoroutines();
            _firstPhaseUIObjects.SetQuestionnaireVisibility(true);
            QuestionObject randomQuestion = _questionObjects[UnityEngine.Random.Range(0, QuestionObject.MaximumQuizQuestions)].ShuffleAnswers();
            while (_previousQuestionName == randomQuestion.Question)
            {
                randomQuestion = _questionObjects[UnityEngine.Random.Range(0, QuestionObject.MaximumQuizQuestions)].ShuffleAnswers();
            }
            _firstPhaseUIObjects.DisplayQuestion(randomQuestion);
            _previousQuestionName = randomQuestion.Question;

            StartCoroutine(FirstPhaseCoroutine(randomQuestion));
        }

        /// <summary>
        /// First boss rush coroutine that adds time based events to the questionnaire
        /// </summary>
        /// <param name="randomQuestion"></param>
        /// <returns></returns>
        IEnumerator FirstPhaseCoroutine(QuestionObject randomQuestion)
        {   
            
            _firstPhaseUIObjects.DisableColliders();

            yield return new WaitForSeconds(1 / _bossSpeed);

            _firstPhaseUIObjects.EnableIncorrectColliders(randomQuestion);

            yield return new WaitForSeconds(0.5f / _bossSpeed);

            _inAttackPhase = false;
            _firstPhaseUIObjects.DisableColliders();
            if (_localPhasePasses <= 0)
            {
                _firstPhaseUIObjects.SetQuestionnaireVisibility(false);
            }
            OnLocalPhaseCompleted();
            
        }
        #endregion

        #region SECOND PHASE
        /// <summary>
        /// Function called in Update() to move the Boss in an infinity symbol movement
        /// </summary>
        private void SecondPhaseBossMovement()
        {
            if (_bossPhase != 2)
            {
                _movementCounter += Time.deltaTime * _bossSpeed;
                if (_movementCounter > Mathf.PI * 2) { _movementCounter = 0; }

                transform.position = new Vector3(Mathf.Cos(_movementCounter) * _horizonatalMultipier, 
                    Mathf.Sin(_movementCounter * _verticalPeriodMultipler)) * _bossSpeed + _defaultPosition;
            }

            
        }

        /// <summary>
        /// Second phase of the boss rush that gets called within Update() that initializes the values for the second boss rush phase and executes the coroutine associated
        /// </summary>
        private void SecondPhase()
        {
            _inAttackPhase = true;
            int playerLane = Mathf.Clamp(Mathf.RoundToInt((_playerMovementReference.transform.position.x + 5) / 5), 0, 2);
            StopAllCoroutines();
            StartCoroutine(SecondPhaseCoroutine(playerLane));

            int index = UnityEngine.Random.Range(0, 3);
            while (index == playerLane){
                index = UnityEngine.Random.Range(0, 3);
            }
            StartCoroutine(SecondPhaseCoroutine(index));
        }

        /// <summary>
        /// Coroutine used for warning the player where the damage colliders will be at, and enabling them for a brief period of tmie
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IEnumerator SecondPhaseCoroutine(int index)
        {
            _colliderZones[index].SetTransitionState(true);
            _colliderZones[index].SetWarningObjectActivity(true);

            yield return new WaitForSeconds(0.125f / _bossSpeed);
            
            for (int i = 0; i < 10; i++)
            {
                _colliderZones[index].SetWarningObjectActivity(true);
                yield return new WaitForSeconds(0.0625f / _bossSpeed);
                _colliderZones[index].SetWarningObjectActivity(false);
                yield return new WaitForSeconds(0.0625f / _bossSpeed);
            }

            _colliderZones[index].SetConstantColliderActivity(true);

            yield return new WaitForSeconds(1 / (float)_bossSpeed);
            _colliderZones[index].SetConstantColliderActivity(false);
            _colliderZones[index].SetTransitionState(false);

            OnLocalPhaseCompleted();
        }

        #endregion

        #region THIRD PHASE

        /// <summary>
        /// Third phase of the boss rush that gets called within Update() that initializes the values for the third boss rush phase and executes the coroutine associated
        /// </summary>
        private void ThirdPhase()
        {
            _inAttackPhase = true;

            int firstIndex = Mathf.Clamp(Mathf.RoundToInt((_playerMovementReference.transform.position.x + 5) / 5), 0, 2);
            int secondIndex = UnityEngine.Random.Range(0, 3);
            while (secondIndex == firstIndex)
            {
                secondIndex = UnityEngine.Random.Range(0, 3);
            }
            _entityShooting.SetLinearShootingAngles(new int[] { _angleArray[firstIndex], _angleArray[secondIndex] });
            _entityShooting.SetCooldownPeriod(_cooldownPeriod / _bossSpeed);

            StopAllCoroutines();
            StartCoroutine(ThirdPhaseCoroutine());
        }

        /// <summary>
        /// Coroutine within the Third phase that interpolates the boss to the default position, and shoots at the angles in the angle array
        /// </summary>
        /// <returns></returns>
        IEnumerator ThirdPhaseCoroutine()
        {
            while ((transform.position - _defaultPosition).magnitude > 0.25f)
            {
                transform.position = Vector3.Lerp(transform.position, _defaultPosition, Time.deltaTime * BossInterpolationSpeed * _bossSpeed);
                yield return null;
            }
            _movementCounter = Mathf.PI / 2;
            transform.position = _defaultPosition;

            float counter = 0;
            while (counter < _alternatingPeriod)
            {
                _entityShooting.enabled = true;
                _entityShooting.EntityShootingUpdate(_playerMovementReference.transform);
                counter += Time.deltaTime * _bossSpeed;
                yield return null;
            }
            
            _entityShooting.enabled = false;
            yield return new WaitForSeconds(_alternatingPeriod / (_bossSpeed * 4));

            OnLocalPhaseCompleted();
        }

        #endregion

        #region BOSS DEFEATED
        /// <summary>
        /// Function called when the boss hits 0 HP
        /// </summary>
        /// <param name="obj"></param>
        private void OnZeroHPEvent(SCR_DamageCollider obj)
        {
            _bossDefeated = true;
            StopAllCoroutines();
            StartCoroutine(OnBossDefeatCoroutine());
        }
        IEnumerator OnBossDefeatCoroutine()
        {
            _firstPhaseUIObjects.SetQuestionnaireVisibility(false);
            while ((transform.position - _defaultPosition).magnitude > 0.25f)
            {
                transform.position = Vector3.Lerp(transform.position, _defaultPosition, Time.deltaTime * BossInterpolationSpeed * _bossSpeed);
                yield return null;
            }

            SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(_defeatDialogue, OnDialogueEnd);
        }

        private void OnDialogueEnd()
        {
            print("TODO : REWARD ITEMS AND TRANSITION TO HUB AREA");
        }

        #endregion

        private void OnDisable()
        {
            _hitboxComponent.OnZeroHPEvent -= OnZeroHPEvent;
            _hitboxComponent.OnDamageEvent -= OnDamageEvent;
        }



        [Serializable] class QuestionObject
        {
            public const int MaximumQuizQuestions = 3;
            public QuestionObject(string question, string[] answers, string correctAnswer)
            {
                Question = question;
                Answers = answers;
                CorrectAnswer = correctAnswer;

                if (!new List<string>(answers).Contains(correctAnswer)){
                    Debug.LogError("CORRECT ANSWER DOES NOT EXIST WITHIN THE ARRAY.");
                }
            }

            [field : SerializeField] [field: TextArea(2, 2)] public string Question { get; private set; }
            [field: SerializeField] public string[] Answers { get; private set; }
            [field: SerializeField] public string CorrectAnswer { get; private set; }

            public void SetAnswers(string[] answer) => Answers = answer;

            public QuestionObject ShuffleAnswers()
            {
                List<string> answers = new List<string>();
                List<int> randomIndexes = new List<int>();
                int randomNumber = UnityEngine.Random.Range(0, MaximumQuizQuestions);

                for (int i = 0; i < 3; i++)
                {
                    while (randomIndexes.Contains(randomNumber))
                    {
                        randomNumber = UnityEngine.Random.Range(0, MaximumQuizQuestions);
                    }
                    randomIndexes.Add(randomNumber);
                    answers.Add(Answers[randomNumber]);
                }
                return new QuestionObject(Question, answers.ToArray(), CorrectAnswer);
            }

        }
        [Serializable] class FirstPhaseUIObjects
        {
            private GameObject _quizDamageColliderParent;
            private GameObject[] _quizDamageColliders;

            private GameObject _questionParent;
            private TextMeshProUGUI _quizQuestionObject;
            private TextMeshProUGUI[] _quizAnswersObjects;

            public FirstPhaseUIObjects(GameObject zerothPhaseObject)
            {
                //INITIALIZE QUIZ DAMAGE COLLIDERS
                _quizDamageColliderParent = zerothPhaseObject.transform.GetChild(1).gameObject;
                _quizDamageColliders = new GameObject[_quizDamageColliderParent.transform.childCount];
                for (int i = 0; i < _quizDamageColliderParent.transform.childCount; i++)
                {
                    _quizDamageColliders[i] = _quizDamageColliderParent.transform.GetChild(i).gameObject;
                }

                //INITIALIZE QUIZ QUESTIONS AND TEXT
                _questionParent = zerothPhaseObject.transform.GetChild(0).gameObject;
                _quizQuestionObject = _questionParent.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                _quizAnswersObjects = new TextMeshProUGUI[QuestionObject.MaximumQuizQuestions];
                for (int i = 1; i < _questionParent.transform.childCount; i++)
                {
                    _quizAnswersObjects[i - 1] = _questionParent.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
                }
            }

            /// <summary>
            /// Dynamically displays the questionnaire to the objects in the scene
            /// </summary>
            /// <param name="questionObject"></param>
            public void DisplayQuestion(QuestionObject questionObject)
            {
                _questionParent.gameObject.SetActive(true);
                _quizDamageColliderParent.gameObject.SetActive(true);

                _quizQuestionObject.text = questionObject.Question;
                for (int i = 0; i < QuestionObject.MaximumQuizQuestions; i++)
                {
                    _quizAnswersObjects[i].text = questionObject.Answers[i];
                }
            }

            /// <summary>
            /// Dynamically enables the colliders that align with the incorrect answers to the questionnaire
            /// </summary>
            /// <param name="questionObject"></param>
            public void EnableIncorrectColliders(QuestionObject questionObject)
            {
                int correctAnswer = Array.IndexOf(questionObject.Answers, questionObject.CorrectAnswer);
                for (int i = 0; i < _quizDamageColliders.Length; i++)
                {
                    _quizDamageColliders[i].SetActive(i != correctAnswer);
                }
            }

            /// <summary>
            /// Disable all damage colliders
            /// </summary>
            public void DisableColliders()
            {
                foreach (GameObject collider in _quizDamageColliders) { collider.SetActive(false); }
            }

            /// <summary>
            /// Set the visibility of the questionnaire
            /// </summary>
            /// <param name="state"></param>
            public void SetQuestionnaireVisibility(bool state)
            {
                _quizDamageColliderParent.SetActive(state);
                _questionParent.SetActive(state);
            }
        }
        [Serializable] class ColliderZones
        {
            private Transform _parentObject;
            [field: SerializeField] public bool BeganTransition { get; private set; }
            [field : SerializeField] public Transform WarningObject { get; private set; }
            [field: SerializeField] public Transform ConstantColliderZone { get; private set; }


            public void SetParentObjectActivity(bool state) {
                WarningObject.gameObject.SetActive(state);
                ConstantColliderZone.gameObject.SetActive(state);
            }
            

            public void SetWarningObjectActivity(bool state) => WarningObject.gameObject.SetActive(state);
            public void SetConstantColliderActivity(bool state) => ConstantColliderZone.gameObject.SetActive(state);

            public void SetTransitionState(bool state) => BeganTransition = state;

            public ColliderZones(Transform parentObject)
            {
                _parentObject = parentObject;
                WarningObject = _parentObject.GetChild(0);
                ConstantColliderZone = _parentObject.GetChild(1);
                BeganTransition = false;
            }

        }
    }

    public abstract class SCR_BossEntity : MonoBehaviour
    {
        public abstract CMP_HealthComponent BossHealthComponent { get; }
    }



}

