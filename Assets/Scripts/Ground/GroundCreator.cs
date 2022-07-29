using System;
using System.Collections;
using System.Collections.Generic;
using MatchingCubes.Obstacle;
using UnityEditor;
using UnityEngine;


namespace MatchingCubes.Ground
{
    public class GroundCreator : MonoBehaviour
    {
        [Header("Development")] [SerializeField]
        private GameSettingsScriptableObject _GameSettings;
        [SerializeField] private List<Transform> _GroundPieces;
        [SerializeField] private Transform _FinishLine;

        [Header("Design")] 
        [SerializeField, Min(0)] private float _PathLength;
        [SerializeField] private List<RampController> _Ramps;

#if UNITY_EDITOR
        private void OnValidate()
        {
            EditorApplication.delayCall += () =>
            {
                var startPosition = Vector3.back * 4;
                Vector3 endPosition;
                float groundLength;

                for (var i = 0; i < _Ramps.Count; i++)
                {
                    if (_GroundPieces.Count < i + i)
                        _GroundPieces.Add(Instantiate(_GroundPieces[0], transform));

                    endPosition = Vector3.forward * _Ramps[i].GetCurveCreator.GetControlPoints[0].transform.position.z;
                    groundLength = endPosition.z - startPosition.z;

                    _GroundPieces[i].localScale =
                        new Vector3(_GameSettings._GroundWidth, _GameSettings._GroundHeight, groundLength);
                    _GroundPieces[0].position = new Vector3(0, -_GameSettings._GroundHeight / 2 - 1,
                        startPosition.z + groundLength / 2);

                    startPosition = new Vector3(0, 0, _Ramps[i].GetCurveCreator.GetControlPoints[3].transform.position.z);
                }

                if (_GroundPieces.Count == _Ramps.Count)
                    _GroundPieces.Add(Instantiate(_GroundPieces[0], transform));

                endPosition = Vector3.forward * _PathLength;
                groundLength = endPosition.z - startPosition.z;

                _GroundPieces[_Ramps.Count].localScale =
                    new Vector3(_GameSettings._GroundWidth, _GameSettings._GroundHeight, groundLength);
                _GroundPieces[_Ramps.Count].position = new Vector3(0, -_GameSettings._GroundHeight / 2 - 1,
                    startPosition.z + groundLength / 2);

                for (var i = _GroundPieces.Count - 1; i > _Ramps.Count; i--)
                {
                    var ground = _GroundPieces[i].gameObject;
                    _GroundPieces.RemoveAt(i);
                    DestroyImmediate(ground);
                }
                
                _FinishLine.position = new Vector3(0, _FinishLine.position.y, _PathLength);
                // _GroundPieces[_Ramps.Count].localScale =
                //     new Vector3(_GameSettings._GroundWidth, _GameSettings._GroundHeight, _PathLength + 4);
                // _GroundPieces[_Ramps.Count].position = new Vector3(0, -_GameSettings._GroundHeight / 2 - 1, _PathLength / 2 - 4);
            };
        }
#endif
        
    }
}
