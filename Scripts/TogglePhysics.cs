using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglePhysics : MonoBehaviour
{
    private bool physicsEnabled = false;

    public void togglePhysics()
    {
        physicsEnabled = !physicsEnabled;
        Physics.autoSimulation = physicsEnabled;
    }
}
