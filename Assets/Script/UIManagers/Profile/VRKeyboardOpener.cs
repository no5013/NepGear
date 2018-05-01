using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VRKeyboardOpener : EventTrigger {
    public override void OnSelect(BaseEventData data)
    {
        Debug.Log("OnSelect called.");
    }
    public override void OnDeselect(BaseEventData data)
    {
        Debug.Log("OnDeselect called.");
    }
}
