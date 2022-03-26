using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipePanel : MonoBehaviour
{
    //[SerializeField] private Transform _panelPosition;
    public Transform PanelPosition { get; private set; }

    private void Awake()
    {
        PanelPosition = transform;
    }
}
