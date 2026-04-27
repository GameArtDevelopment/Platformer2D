using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public bool freezerVertical, freezeHorizontal;
    private Vector3 positionStore;

    public bool clampPosition;
    public Transform clampMin, clampMax;
    private float halfWidth, halfHeight;
    public Camera theCam;

    private void Start()
    {
        positionStore = transform.position;

        clampMin.SetParent(null);
        clampMax.SetParent(null);

        halfHeight = theCam.orthographicSize;
        halfWidth = theCam.orthographicSize * theCam.aspect;
    }

    void LateUpdate()
    {
        transform.position = new Vector3(target.position.x, target.position.y, positionStore.z);

        if (freezerVertical == true)
        {
            transform.position = new Vector3(transform.position.x, positionStore.y, transform.position.z);
        }
        if (freezeHorizontal == true)
        {
            transform.position = new Vector3(positionStore.x, transform.position.y, transform.position.z);
        }

        if (clampMin == true)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, clampMin.position.x + halfWidth, Mathf.Infinity), Mathf.Clamp(transform.position.y, clampMin.position.y + halfHeight, Mathf.Infinity), transform.position.z);
        }
        //  Parallax background info
    }

    private void OnDrawGizmos()
    {
        if (clampMin == true && clampMax == true)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(clampMin.position.x + halfWidth, clampMin.position.y + halfHeight, 0f), new Vector3(clampMax.position.x - halfWidth, clampMin.position.y + halfHeight, 0f));
            Gizmos.DrawLine(new Vector3(clampMin.position.x + halfWidth, clampMin.position.y + halfHeight, 0f), new Vector3(clampMin.position.x + halfWidth, clampMax.position.y - halfHeight, 0f));
            Gizmos.DrawLine(new Vector3(clampMax.position.x - halfWidth, clampMin.position.y + halfHeight, 0f), new Vector3(clampMax.position.x - halfWidth, clampMax.position.y - halfHeight, 0f));
            Gizmos.DrawLine(new Vector3(clampMin.position.x + halfWidth, clampMax.position.y - halfHeight, 0f), new Vector3(clampMax.position.x - halfWidth, clampMax.position.y - halfHeight, 0f));
        }
    }
}
