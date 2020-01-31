using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform followTransform;
    [SerializeField] private float followSpeed;

    private float _cameraDistanceZ = -10.0f;
    
    private void Awake()
    {
        transform.position = new Vector3(followTransform.position.x, followTransform.position.y, _cameraDistanceZ);
    }

    private void Update()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        float interpolation = followSpeed * Time.deltaTime;
        Vector3 position = transform.position;
        position.y = Mathf.Lerp(transform.position.y, followTransform.transform.position.y, interpolation);
        position.x = Mathf.Lerp(transform.position.x, followTransform.transform.position.x, interpolation);
        position.z = _cameraDistanceZ;
        transform.position = position;
    }
}
