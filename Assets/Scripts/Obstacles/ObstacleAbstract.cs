using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObstacleAbstract : MonoBehaviour
{
    [SerializeField] private ObstacleType _ObstacleType;
    [SerializeField] private BoxCollider _BoxCollider;

    public ObstacleType GetObstacleType => _ObstacleType;
    public BoxCollider GetBoxCollider => _BoxCollider;

    public void Hit()
    {
        _BoxCollider.enabled = false;
    }
}
