using UnityEngine;
public class DynamicScale : MonoBehaviour
{
    public Transform startNode;
    public Transform endNode;

    void Update()
    {
        Vector3 startNodePosition = startNode.position;
        Vector3 endNodePosition = endNode.position;  
        Vector3 direction = startNodePosition - endNodePosition;

        float length = direction.magnitude/2;
        transform.localScale = new(transform.localScale.x, length, transform.localScale.z);
        transform.SetPositionAndRotation(((startNodePosition + endNodePosition) / 2f), Quaternion.FromToRotation(
            Vector3.up, direction));
    }
}
