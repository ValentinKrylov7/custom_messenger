using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SwipeController : MonoBehaviour
{
    private List<SwipePanel> _swipePanels = new List<SwipePanel>();
    [SerializeField] private Transform _logInPosition;
    [SerializeField] private Transform _signUpPosition;
    
    private void Awake()
    {
        var panels = GetComponentsInChildren<SwipePanel>();

        if (panels != null)
        {
            foreach (var panel in panels)
            {
                _swipePanels.Add(panel);
            }
        }
    }
    
    public void Move(Positions position)
    {
        if (position == Positions.LeftPosition)
        {
            _logInPosition.transform.DOMove(_swipePanels[1].PanelPosition.position, 0.5f);
            _signUpPosition.transform.DOMove(_swipePanels[0].PanelPosition.position, 0.5f);
        }
        
        if (position == Positions.RightPosition)
        {
            _signUpPosition.transform.DOMove(_swipePanels[1].PanelPosition.position, 0.5f);
            _logInPosition.transform.DOMove(_swipePanels[2].PanelPosition.position, 0.5f);
        }
       
    }
}
