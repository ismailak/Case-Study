using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettingsScriptableObject")]
public class GameSettingsScriptableObject : ScriptableObject
{
    [Header("Ground")] 
    [Min(0)] public float _GroundWidth = 6;
    [Min(0)] public float _GroundHeight = 8;
    
    [Header("Player")] 
    [Min(0)] public float _ForwardSpeed = 5;
    [Min(0)] public float _PathWidth = 6;
    [Min(0)] public float _BorderDistance = .5f;
    
    [Header("Input")] 
    [Min(0)] public float _InputSensitivity = 1;

    [Header("Cube Materials")] 
    public Material _BlueMaterial;
    public Material _OrangeMaterial;
    public Material _PurpleMaterial;
    
    [Header("Cube Animations")]
    [Min(1)] public float _AddedCubeScaleBounce = 1.5f;
    [Min(0)] public float _CubesJumpMaxHeight = 1f;
    [Min(.2f)] public float _AddedCubePlaceDuration = .5f;
    
    [Header("Ramp")]
    [Min(0)] public float _RampDuration = 2f;
    
    [Header("Booster")]
    [Min(1)] public float _BoostSpeedMultiplier = 2.5f;
    [Min(0)] public float _BoostDuration = 3f;
    
    [Header("Ramp")]
    [Min(0)] public float _BridgeDuration = 3f;
}
