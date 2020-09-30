using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    public float _dampTime = 0.2f;
    public float _screenEdgeBuffer = 4f;
    public float _minSize = 6.5f;

    public Transform[] _tanksTrans;
    public TankManager _tankManager;

    protected Camera mCamera;
    protected Vector3 mAveragePos;
    protected float mZoomSpeed;
    protected Vector3 mMoveVelocity;

    private void Awake()
    {
        mCamera = GetComponentInChildren<Camera>();
    }
    private void Start()
    {
        _tanksTrans = _tankManager.GetTanksTransform();
    }

    private void FixedUpdate()
    {
        Move();
        Zoom();
    }

    protected void Move ()
    {
        // Find the average position of the targets.
        CalculateAveragePosition();

        // Smoothly transition to that position.
        transform.position = Vector3.SmoothDamp(transform.position, mAveragePos, ref mMoveVelocity, _dampTime);
    }
    private void CalculateAveragePosition ()
    {
        Vector3 sumPos = new Vector3();
        int activeTanksCount = 0;

        // Loop to sum up all active tanks' position
        for (int i = 0; i < _tanksTrans.Length; i++)
        {
            // Skip non-active object(s)
            if (!_tanksTrans[i].gameObject.activeSelf)
                continue;

            sumPos += _tanksTrans[i].position;
            activeTanksCount++;
        }

        // Get the average by dividing (only if number of active tanks are not zero)
        if (activeTanksCount > 0)
            mAveragePos = sumPos / activeTanksCount;

        // Retain the Y position
        mAveragePos.y = transform.position.y;
    }

    protected void Zoom()
    {
        mCamera.orthographicSize = Mathf.SmoothDamp(mCamera.orthographicSize, GetRequiredSize(), ref mZoomSpeed, _dampTime);
    }
    protected float GetRequiredSize()
    {
        // Size cannot be smaller than the minimum size
        float size = _minSize;

        // Convert average position to local position of camera rig
        Vector3 localAveragePos = transform.InverseTransformPoint(mAveragePos);

        // Loop through all tanks and check which one is closest to the edge of the screen
        foreach(Transform target in _tanksTrans)
        {
            // Skip any tanks that are not active
            if (!target.gameObject.activeSelf)
                continue;

            // Get the local position of the target relative to the camera
            Vector3 targetLocalPos = transform.InverseTransformPoint(target.position);
            // Calculate the size require for this tank
            Vector3 tempSize = targetLocalPos - localAveragePos;

            // Compare size on both axis and retain the bigger one
            // y-axis
            size = Mathf.Max(size, Mathf.Abs(tempSize.y));
            // x-axis
            size = Mathf.Max(size, Mathf.Abs(tempSize.x) / mCamera.aspect);
        }

        size += _screenEdgeBuffer;

        return size;
    }
}
