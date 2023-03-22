using UnityEngine;
using static UnityEngine.Mathf;
namespace Assets.Scripts
{
    public class ChangeColorByPosition : MonoBehaviour
    {
        float maximumPosition = 500;
        void Start()
        {
            Renderer renderer = GetComponent<Renderer>();
            Vector3 pos = transform.position;
            renderer.material.color = new Color(Mathf.Abs(pos.x) / maximumPosition, Mathf.Abs(pos.y) / maximumPosition, Mathf.Abs(pos.z) / maximumPosition);
        }
    }
}
