using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : ObstacleAbstract, IHittable
{
    [SerializeField] private CubeType _CubeType;
    [SerializeField] private MeshRenderer _MeshRenderer;
    [SerializeField] private GameSettingsScriptableObject _GameSettings;

    public CubeType GetCubeType => _CubeType;

    private void OnValidate()
    {
        _MeshRenderer.material = _CubeType switch
        {
            CubeType.Blue => _GameSettings._BlueMaterial,
            CubeType.Orange => _GameSettings._OrangeMaterial,
            CubeType.Purple => _GameSettings._PurpleMaterial,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void Hit()
    {
        
    }
}
