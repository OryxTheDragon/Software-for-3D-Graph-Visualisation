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
            renderer.material.color =
                new Color(
                    Abs(pos.x) / maximumPosition,
                    Abs(pos.y) / maximumPosition,
                    Abs(pos.z) / maximumPosition
                );
        }
    }
}
