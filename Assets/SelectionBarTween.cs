using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SelectionBarTween : MonoBehaviour
{
    [SerializeField] private Transform _bar;
    [SerializeField] private Transform _logInBarPosition;
    [SerializeField] private Transform _signUpBarPosition;
    [SerializeField] private SwipeController _swipeController;
    

    private Positions _currentPosition;
    
    private void OnEnable()
    {
        _currentPosition = Positions.LeftPosition;
        SetStartPosition();
    }

    private void SetStartPosition()
    {
        _bar.position = _logInBarPosition.position;
    }

    public void Move()
    {
        if (_currentPosition == Positions.LeftPosition)
        {
            _bar.transform.DOMove(_signUpBarPosition.position, 0.5f);
            _currentPosition = Positions.RightPosition;
            _swipeController.Move(_currentPosition);
        }
        
        else
        {
            _bar.transform.DOMove(_logInBarPosition.position, 0.5f);
            _currentPosition = Positions.LeftPosition;
            _swipeController.Move(_currentPosition);
        }
    }
}

public enum Positions
{
    RightPosition,
    LeftPosition
}
