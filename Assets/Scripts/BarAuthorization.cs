using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BarAuthorization : MonoBehaviour
{
    [SerializeField] private Transform _bar;
    [SerializeField] private Transform _logInBarPosition;
    [SerializeField] private Transform _signUpBarPosition;
    
    public Positions CurrentPosition { get; private set; }

    private void OnEnable()
    {
        CurrentPosition = Positions.LeftPosition;
    }
    
    // Привязано положение панелей к положению бара
    public void MoveBar()
    {
        if (CurrentPosition == Positions.LeftPosition)
        {
            _bar.transform.DOMove(_signUpBarPosition.position, 0.5f);
            CurrentPosition = Positions.RightPosition;
        }
        
        else
        {
            _bar.transform.DOMove(_logInBarPosition.position, 0.5f);
            CurrentPosition = Positions.LeftPosition;
        }
    }
}

public enum Positions
{
    RightPosition,
    LeftPosition
}
