using UnityEngine;
using UnityEngine.UI;

public class TogglePhysics : MonoBehaviour
{
    private bool physicsEnabled = false;

    public void togglePhysics()
    {
        physicsEnabled = !physicsEnabled;
        Physics.autoSimulation = physicsEnabled;
        if (physicsEnabled)
        {
            transform.parent.GetChild(0).GetComponent<Button>().GetComponent<Image>().color = new Color(0.9f,0.5f,0.5f,1.0f);
        }
        else
        {
            transform.parent.GetChild(0).GetComponent<Button>().GetComponent<Image>().color = new Color(0.7f,0.9f,1.0f,1.0f);
        }
    }
}
