using UnityEngine;

public class CameraFollowX:MonoBehaviour
{
    public Transform Target;
    public float OffsetX=3f;

    void LateUpdate()
    {
        float targetX=Target.position.x+OffsetX;
        if(targetX>transform.position.x)
        {
            transform.position=new Vector3(
                targetX,
                transform.position.y,
                transform.position.z
            );
        }
    }

    public void SnapToTarget()
    {
        transform.position=new Vector3(
            Target.position.x+OffsetX,
            transform.position.y,
            transform.position.z
        );
    }
}
