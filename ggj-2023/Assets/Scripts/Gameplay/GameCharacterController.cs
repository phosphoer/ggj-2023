using UnityEngine;

public class GameCharacterController : MonoBehaviour
{
  public Transform CameraRoot => _cameraRoot;

  [Range(-1, 1)]
  public float MoveAxis = 0.0f;

  [Range(-1, 1)]
  public float StrafeAxis = 0.0f;

  [Range(-1, 1)]
  public float LookHorizontalAxis = 0.0f;

  [Range(-1, 1)]
  public float LookVerticalAxis = 0.0f;

  [SerializeField]
  private Transform _cameraRoot = null;

  [SerializeField]
  private LayerMask _groundLayer = default(LayerMask);

  [SerializeField]
  private LayerMask _obstacleLayer = default(LayerMask);

  [SerializeField]
  private float _groundRaycastRadius = 0.4f;

  [SerializeField]
  private float _obstacleRaycastRadius = 0.7f;

  [SerializeField]
  private float _minDistToObstacle = 0.8f;

  [SerializeField]
  private float _raycastUpStartOffset = 1.0f;

  [SerializeField]
  private float _terrainAlignmentSpeed = 3.0f;

  [SerializeField]
  private float _moveSpeed = 1.0f;

  [SerializeField]
  private float _strafeSpeed = 1.0f;

  [SerializeField]
  private float _turnSpeed = 90.0f;

  [SerializeField]
  private float _gravity = 5;

  private RaycastHit _groundRaycast;
  private RaycastHit _obstacleRaycast;
  private Vector3 _lastGroundPos;

  private Vector3 _raycastStartPos => transform.position + transform.up * _raycastUpStartOffset;

  private void Update()
  {
    // Calculate next position based on movement
    Vector3 newPosition = transform.position + transform.forward * MoveAxis * _moveSpeed * Time.deltaTime;
    newPosition += transform.right.WithY(0) * StrafeAxis * _strafeSpeed * Time.deltaTime;

    // Snap and align to ground
    Vector3 raycastDir = -transform.up + (transform.forward * MoveAxis + transform.right * StrafeAxis) * 0.5f;
    if (Physics.SphereCast(_raycastStartPos, _groundRaycastRadius, raycastDir, out _groundRaycast, 3.0f, _groundLayer))
    {
      _lastGroundPos = _groundRaycast.point;

      Vector3 toGroundPoint = _groundRaycast.point - newPosition;
      newPosition += Vector3.ClampMagnitude(toGroundPoint, 1f) * Time.deltaTime * _gravity;

      Quaternion desiredRot = Quaternion.FromToRotation(transform.up, _groundRaycast.normal) * transform.rotation;
      transform.rotation = Mathfx.Damp(transform.rotation, desiredRot, 0.25f, Time.deltaTime * _terrainAlignmentSpeed);
    }
    // If no ground, go towards where it was last
    else
    {
      Vector3 fallDir = (_lastGroundPos - newPosition).normalized;
      Quaternion desiredRot = Quaternion.FromToRotation(transform.up, -fallDir) * transform.rotation;

      newPosition += fallDir * Time.deltaTime * _gravity;
      transform.rotation = Mathfx.Damp(transform.rotation, desiredRot, 0.25f, Time.deltaTime * _terrainAlignmentSpeed);
    }

    // Collide with obstacles
    Vector3 moveVec = newPosition - transform.position;
    if (Physics.SphereCast(transform.position, _obstacleRaycastRadius, moveVec.normalized, out _obstacleRaycast, _minDistToObstacle + 1, _obstacleLayer))
    {
      // Find the plane representing the point + normal we hit
      Plane hitPlane = new Plane(_obstacleRaycast.normal, _obstacleRaycast.point);

      // Now project our position onto that plane and use the vector from
      // the projected point to our pos as the adjusted normal
      Vector3 closestPoint = hitPlane.ClosestPointOnPlane(newPosition);
      Vector3 closestPointToPos = newPosition - closestPoint;

      // "Clamp" our distance from the plane to a min distance
      float planeDist = closestPointToPos.magnitude;
      float adjustedDist = Mathf.Max(planeDist, _minDistToObstacle);
      newPosition = closestPoint + closestPointToPos.normalized * adjustedDist;
    }

    // Apply movement
    transform.position = newPosition;
    transform.Rotate(Vector3.up, LookHorizontalAxis * _turnSpeed * Time.deltaTime, Space.Self);
    _cameraRoot.Rotate(Vector3.right, -LookVerticalAxis * _turnSpeed * Time.deltaTime, Space.Self);
  }
}