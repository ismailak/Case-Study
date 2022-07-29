using System;
using System.Collections;
using System.Collections.Generic;
using MatchingCubes.Block;
using MatchingCubes.Obstacle;
using UnityEngine;

namespace MatchingCubes.Player
{
    public class CollisionDetector : MonoBehaviour
    {
        public event Action<CubeController> DidCollidedToCube;
        public event Action<BlockController, GameObject> DidCollidedToBlock;
        public event Action<RegularOrderController> DidCollidedToRegularOrder;
        public event Action<RandomOrderController> DidCollidedToRandomOrder;
        public event Action DidCollidedToBooster;
        public event Action<RampController> DidCollidedToRamp;
        public event Action<BridgeController> DidCollidedToBridge;
        public event Action<bool> DidFinished;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ObstacleAbstract obstacleAbstract))
            {
                switch (obstacleAbstract.GetObstacleType)
                {
                    case ObstacleType.Cube:
                        DidCollidedToCube?.Invoke(obstacleAbstract as CubeController);
                        break;
                    case ObstacleType.RegularOrder:
                        DidCollidedToRegularOrder?.Invoke(obstacleAbstract as RegularOrderController);
                        break;
                    case ObstacleType.RandomOrder:
                        DidCollidedToRandomOrder?.Invoke(obstacleAbstract as RandomOrderController);
                        break;
                    case ObstacleType.Booster:
                        DidCollidedToBooster?.Invoke();
                        break;
                    case ObstacleType.Ramp:
                        DidCollidedToRamp?.Invoke(obstacleAbstract as RampController);
                        break;
                    case ObstacleType.Bridge:
                        DidCollidedToBridge?.Invoke(obstacleAbstract as BridgeController);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                obstacleAbstract.Hit();
            }
            else if(other.GetComponentInParent<BlockController>())
                DidCollidedToBlock?.Invoke(other.GetComponentInParent<BlockController>(), other.gameObject);
            else if (other.GetComponent<FinishLine>())
                DidFinished?.Invoke(true);
        }
    }
}