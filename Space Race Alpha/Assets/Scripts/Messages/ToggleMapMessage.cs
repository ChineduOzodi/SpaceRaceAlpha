using UnityEngine;
using System.Collections;
using CodeControl;

public class SetCameraView : Message {

    public CameraView cameraView;
    public double distanceModifier;
    public SolarBodyModel reference;
}
