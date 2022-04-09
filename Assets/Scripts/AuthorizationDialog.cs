using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthorizationDialog : MonoBehaviour
{
    [SerializeField] private HeaderAuthorization _header;
    [SerializeField] private SwipePanelsAuthorization _swipePanels;

    private void Awake()
    {
        _header.BarIsMoving += MoveSwipe;
    }
    
    private void MoveSwipe()
    {
        var currentPosition = _header.BarAuthorization.CurrentPosition;
        _swipePanels.MovePanels(currentPosition);
    }
}
