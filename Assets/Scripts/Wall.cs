using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public class Wall : MonoBehaviour
    {
        private void Start()
        {
            int2 myGridPosition = transform.position.GetGridPosition();
            GameManager.Instance.level.MakeUnwalkable(myGridPosition);
            transform.position = myGridPosition.GetWorldPosition();
        }
    }
}