using UnityEngine;
using System.Collections;
using CodeControl;

public class ToggleMessage : Message {

    public string label;
    public string toggleGroupName;
    public bool isToggled;
    internal int labelID;
}
