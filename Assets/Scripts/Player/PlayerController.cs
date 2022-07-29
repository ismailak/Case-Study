using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using MatchingCubes.Block;
using MatchingCubes.Obstacle;


namespace MatchingCubes.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Development")] 
        [SerializeField] private GameSettingsScriptableObject _GameSettings;
        [SerializeField] private InputController _InputController;
        [SerializeField] private CollisionDetector _CollisionDetector;
        [SerializeField] private Transform _BodyTransform;

        private PlayerState _playerState;
        private List<CubeController> _cubeControllers;
        private float _speedMultiplier;


        private void Start()
        {
            _cubeControllers = new List<CubeController>();
            _playerState = PlayerState.Run;
            _speedMultiplier = 1;
            
            _InputController.DidDrag += Swerve;
            _CollisionDetector.DidCollidedToCube += OnCollidedToCube;
            _CollisionDetector.DidCollidedToBlock += OnCollidedToBlock;
            _CollisionDetector.DidCollidedToRegularOrder += OnCollidedToRegularOrder;
            _CollisionDetector.DidCollidedToRandomOrder += OnCollidedToRandomOrder;
            _CollisionDetector.DidCollidedToBooster += OnCollidedBooster;
            _CollisionDetector.DidCollidedToRamp += OnCollidedRamp;
            _CollisionDetector.DidCollidedToBridge += OnCollideBridge;
            _CollisionDetector.DidFinished += FinishGame;
        }


        private void Update()
        {
            if (_playerState is PlayerState.Run or PlayerState.Boosted)
                transform.Translate(Vector3.forward * _GameSettings._ForwardSpeed * Time.deltaTime * _speedMultiplier,
                    Space.World);
        }


        private void Swerve(Vector3 swerveVector)
        {
            var pathHalfLength = _GameSettings._PathWidth / 2 - _GameSettings._BorderDistance;
            var targetPosition = transform.position;
            targetPosition.x = Mathf.Clamp(targetPosition.x + swerveVector.x, -pathHalfLength, pathHalfLength);

            transform.position = targetPosition;
        }
        

        private void CheckCubes()
        {
            var sameColorCubeCount = 0;
            var previousCubeType = CubeType.None;

            for (var i = _cubeControllers.Count - 1; i >= 0; i--)
            {
                var currentCubeType = _cubeControllers[i].GetCubeType;
                if (currentCubeType == previousCubeType)
                {
                    sameColorCubeCount++;
                    if (sameColorCubeCount == 3)
                    {
                        DestroyThreeCube(i);
                        CheckCubes();
                        return;
                    }
                }
                else
                {
                    previousCubeType = currentCubeType;
                    sameColorCubeCount = 1;
                }
            }
        }


        private void FinishGame(bool isSuccess)
        {
            Debug.Log("Game Finished (" + isSuccess + ")");
            _playerState = PlayerState.Finish;
        }


        #region Destroy

        private void DestroyThreeCube(int startIndex)
        {
            for (var i = startIndex + 2; i >= startIndex; i--)
            {
                var cube = _cubeControllers[i];
                _cubeControllers.RemoveAt(i);
                Destroy(cube.gameObject);
            }

            OrderCubesAfterDestroy();
        }


        private void OrderCubesAfterDestroy()
        {
            for (var i = 0; i < _cubeControllers.Count; i++)
                _cubeControllers[_cubeControllers.Count - 1 - i].transform.localPosition = new Vector3(0, i - .5f, 0);

            _BodyTransform.transform.localPosition = Vector3.up * _cubeControllers.Count;
        }

        #endregion


        #region Cube Animations

        private void Jump(float duration, float turnBackDelay = 0)
        {
            duration /= _speedMultiplier;
            
            for (var i = 0; i < _cubeControllers.Count; i++)
            {
                var cubeJumpHeight = _GameSettings._CubesJumpMaxHeight / (_cubeControllers.Count - i + 1);
                DOTween.Sequence()
                    .Append(_cubeControllers[_cubeControllers.Count - 1 - i].transform
                        .DOLocalMove(new Vector3(0, i - .5f + cubeJumpHeight, 0), .6f * duration)).Append(
                        _cubeControllers[_cubeControllers.Count - 1 - i].transform
                            .DOLocalMove(new Vector3(0, i - .5f, 0), .4f * duration).SetDelay(turnBackDelay));
            }

            DOTween.Sequence()
                .Append(_BodyTransform.transform
                    .DOLocalMove(Vector3.up * (_cubeControllers.Count + _GameSettings._CubesJumpMaxHeight), .6f * duration)
                    .OnComplete(CheckCubes)).Append(_BodyTransform.transform
                    .DOLocalMove(Vector3.up * _cubeControllers.Count, .4f * duration)
                    .SetDelay(turnBackDelay));
        }

        #endregion


        #region OnCollided Methods

        private void OnCollidedToCube(CubeController cubeController)
        {
            cubeController.GetBoxCollider.enabled = false;
            cubeController.transform.SetParent(transform);
            _cubeControllers.Add(cubeController);
            var originalScale = cubeController.transform.localScale;

            var duration = _GameSettings._AddedCubePlaceDuration / _speedMultiplier;
            
            Jump(_GameSettings._AddedCubePlaceDuration);
            
            DOTween.Sequence()
                .Append(cubeController.transform
                    .DOScale(originalScale * _GameSettings._AddedCubeScaleBounce, 0.4f * duration)
                    .SetEase(Ease.Linear))
                .Append(cubeController.transform.DOScale(originalScale, 0.6f * duration).SetEase(Ease.Linear).OnComplete(CheckCubes));
        }
        
        
        private void OnCollidedRamp(RampController rampController)
        {
            var curvePoints = rampController.GetCurveCreator.GetControlPoints;

            StartCoroutine(FollowCurveRoutine());

            IEnumerator FollowCurveRoutine()
            {
                _playerState = PlayerState.Ramp;
                var time = 0f;
                var rampDuration = _GameSettings._RampDuration / _speedMultiplier;
                
                Jump(_GameSettings._RampDuration / 2, _GameSettings._RampDuration / 2);

                while (time < 1)
                {
                    time += Time.deltaTime / rampDuration;

                    var nextPosition = Mathf.Pow(1 - time, 3) * curvePoints[0].position +
                                         3 * Mathf.Pow(1 - time, 2) * time * curvePoints[1].position +
                                         3 * (1 - time) * Mathf.Pow(time, 2) * curvePoints[2].position +
                                         Mathf.Pow(time, 3) * curvePoints[3].position;
                    
                    transform.position = new Vector3(transform.position.x, nextPosition.y, nextPosition.z);
                    
                    yield return null;
                }

                _playerState = PlayerState.Run;
            }
        }
        
        
        private void OnCollideBridge(BridgeController bridgeController)
        {
            var curvePoints = bridgeController.GetCurveCreator.GetControlPoints;

            StartCoroutine(FollowCurveRoutine());

            IEnumerator FollowCurveRoutine()
            {
                _playerState = PlayerState.Bridge;
                var time = 0f;
                var bridgeDuration = _GameSettings._BridgeDuration / _speedMultiplier;

                while (time < 1)
                {
                    time += Time.deltaTime / bridgeDuration;

                    var nextPosition = Mathf.Pow(1 - time, 3) * curvePoints[0].position +
                                       3 * Mathf.Pow(1 - time, 2) * time * curvePoints[1].position +
                                       3 * (1 - time) * Mathf.Pow(time, 2) * curvePoints[2].position +
                                       Mathf.Pow(time, 3) * curvePoints[3].position;

                    transform.position = nextPosition;
                    
                    yield return null;
                }

                _playerState = PlayerState.Run;
            }
        }
        
        
        private void OnCollidedBooster()
        {
            _speedMultiplier = _GameSettings._BoostSpeedMultiplier;
            _playerState = PlayerState.Boosted;
            StartCoroutine(CancelBoostRoutine());

            IEnumerator CancelBoostRoutine()
            {
                yield return new WaitForSeconds(_GameSettings._BoostDuration);
                _speedMultiplier = 1;
                _playerState = PlayerState.Run;
            }
        }
        
        
        private void OnCollidedToRandomOrder(RandomOrderController randomOrderController)
        {
            _cubeControllers = _cubeControllers.OrderBy(i => Guid.NewGuid()).ToList();

            for (var i = 0; i < _cubeControllers.Count; i++)
                _cubeControllers[_cubeControllers.Count - 1 - i].transform.localPosition = new Vector3(0, i - .5f, 0);

            CheckCubes();
        }
        
        
        private void OnCollidedToRegularOrder(RegularOrderController regularOrderController)
        {
            _cubeControllers = _cubeControllers.OrderBy(cubeController => cubeController.GetCubeType).ToList();

            for (var i = 0; i < _cubeControllers.Count; i++)
                _cubeControllers[_cubeControllers.Count - 1 - i].transform.localPosition = new Vector3(0, i - .5f, 0);

            CheckCubes();
        }
        
        
        private void OnCollidedToBlock(BlockController blockController, GameObject hitCube)
        {
            if (!blockController.IsActive) return;
            if (_playerState == PlayerState.Boosted)
            {
                blockController.DestroyBlocks(transform.position);
                return;
            }

            var height = blockController.CalculateHeight(hitCube);
            var remainingCubeCount = _cubeControllers.Count - height;

            if (remainingCubeCount < 0)
            {
                FinishGame(false);
                return;
            }

            for (var i = _cubeControllers.Count - 1; i >= remainingCubeCount; i--)
            {
                var cube = _cubeControllers[i];
                _cubeControllers.RemoveAt(i);
                Destroy(cube.gameObject);
            }

            OrderCubesAfterDestroy();
        }
        
        #endregion
    }
}