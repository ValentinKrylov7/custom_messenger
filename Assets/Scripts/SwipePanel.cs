using UnityEngine;

public class SwipePanel : MonoBehaviour
{
    public Transform PanelPosition { get; private set; }

    private void OnEnable()
    {
        PanelPosition = transform;
    }
}
