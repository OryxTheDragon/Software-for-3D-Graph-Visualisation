using UnityEngine;
using static UnityEngine.Mathf;
namespace Assets.Scripts
{
    public class ChangeColorByPosition : MonoBehaviour
    {
        void Start()
        {
            ObjectID maximumPosition = GetComponent<ObjectID>();
            Renderer renderer = GetComponent<Renderer>();
            Vector3 pos = transform.position;
            renderer.material.color =
                new Color(
                    Abs(pos.x) / maximumPosition.scale,
                    Abs(pos.y) / maximumPosition.scale,
                    Abs(pos.z) / maximumPosition.scale
                );
        }
    }
}
