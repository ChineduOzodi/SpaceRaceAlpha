﻿using UnityEngine;
using System.Collections;
using CodeControl;

public class InfoPanelMessage : Message {

    public BaseModel model;

    public InfoPanelMessage() { }
    public InfoPanelMessage(BaseModel m)
    {
        model = m;
    }
}
