using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeaderController : MonoBehaviour
{
    [SerializeField] private Button _logInButton;
    [SerializeField] private Button _signUpButton;

    [SerializeField] private SelectionBarTween _selectionBarTween;
    
    private void OnEnable()
    {
        _logInButton.onClick.AddListener(ChangeAuthorizationPanel);
        _signUpButton.onClick.AddListener(ChangeAuthorizationPanel);
    }
    
    private void OnDisable()
    {
        _logInButton.onClick.RemoveListener(ChangeAuthorizationPanel);
        _signUpButton.onClick.RemoveListener(ChangeAuthorizationPanel);
    }

    private void ChangeAuthorizationPanel()
    {
        _selectionBarTween.Move();
    }
}
