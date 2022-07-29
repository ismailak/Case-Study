using System.Collections;
using System.Collections.Generic;
using MatchingCubes.Curve;
using UnityEngine;


namespace MatchingCubes.Obstacle
{
    public class RampController : ObstacleAbstract
    {
        [SerializeField] private CurveCreator _CurveCreator;

        public CurveCreator GetCurveCreator => _CurveCreator;
    }
}