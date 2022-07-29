using System.Collections;
using System.Collections.Generic;
using MatchingCubes.Curve;
using UnityEngine;

namespace MatchingCubes.Obstacle
{
    public class BridgeController : ObstacleAbstract
    {
        [SerializeField] private CurveCreator _CurveCreator;

        public CurveCreator GetCurveCreator => _CurveCreator;
    }
}