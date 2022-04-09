using System;
using UnityEngine;
using UnityEngine.UI;

public class HeaderAuthorization : MonoBehaviour
{
    [SerializeField] private Button _logInButton;
    [SerializeField] private Button _signUpButton;
    [SerializeField] private BarAuthorization barAuthorization;

    public BarAuthorization BarAuthorization => barAuthorization;
    
    public Action BarIsMoving;

    private void OnEnable()
    {
        _logInButton.onClick.AddListener(ChangeBarPosition);
        _signUpButton.onClick.AddListener(ChangeBarPosition);
    }
    
    private void OnDisable()
    {
        _logInButton.onClick.RemoveListener(ChangeBarPosition);
        _signUpButton.onClick.RemoveListener(ChangeBarPosition);
    }

    public void ChangeBarPosition()
    {
       barAuthorization.MoveBar();
       BarIsMoving?.Invoke();
    }
}
