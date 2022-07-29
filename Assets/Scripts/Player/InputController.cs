using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{
    public event Action<Vector3> DidDrag;

    [Header("Development")]
    [SerializeField] private GameSettingsScriptableObject _GameSettings;
    
    private bool _isInputActive;
    private Vector3 _lastMousePosition;
    
    private void Update()
    {
        if (Input.GetMouseButtonUp(0)) _isInputActive = false;
        
        if(EventSystem.current && EventSystem.current.currentSelectedGameObject) return;

        if (Input.GetMouseButtonDown(0))
        {
            _isInputActive = true;
            _lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0) && _isInputActive)
        {
            DidDrag?.Invoke((Input.mousePosition - _lastMousePosition) * Time.deltaTime * _GameSettings._InputSensitivity);
            _lastMousePosition = Input.mousePosition;
        }
    }
}
