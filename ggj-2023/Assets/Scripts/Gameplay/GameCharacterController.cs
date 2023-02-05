using UnityEngine;

public class GameCharacterController : MonoBehaviour
{
  public Transform CameraRoot => _cameraRoot;
  public Transform HeldItemRoot => _heldItemRoot;
  public ItemController HeldItem => _heldItem;
  public InteractionController InteractionController => _interactionController;

  [Range(-1, 1)]
  public float MoveAxis = 0.0f;

  [Range(-1, 1)]
  public float StrafeAxis = 0.0f;

  [Range(-1, 1)]
  public float LookHorizontalAxis = 0.0f;

  [Range(-1, 1)]
  public float LookVerticalAxis = 0.0f;

  public RangedFloat LookVerticalRange = new RangedFloat(-45, 45);

  [SerializeField]
  private InteractionController _interactionController = null;

  [SerializeField]
  private Slappable _slappable = null;

  [SerializeField]
  private Animator _animator = null;

  [SerializeField]
  private AnimatorCallbacks _animatorCallback = null;

  [SerializeField]
  private Transform _cameraRoot = null;

  [SerializeField]
  private Transform _heldItemRoot = null;

  [SerializeField]
  private Transform _attackPos = null;

  [SerializeField]
  private LayerMask _groundLayer = default(LayerMask);

  [SerializeField]
  private LayerMask _obstacleLayer = default(LayerMask);

  [SerializeField]
  private LayerMask _attackLayer = default(LayerMask);

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
  private float _turnSpeed = 90.0f;

  [SerializeField]
  private float _gravity = 5;

  private RaycastHit _groundRaycast;
  private RaycastHit _obstacleRaycast;
  private Vector3 _lastGroundPos;
  private ItemController _heldItem;
  private float _holdItemBlend;
  private Collider[] _overlapColliders = new Collider[10];

  private Vector3 _raycastStartPos => transform.position + transform.up * _raycastUpStartOffset;

  private static int kAnimMoveSpeed = Animator.StringToHash("MoveSpeed");
  private static int kAnimHoldItemBlend = Animator.StringToHash("HoldItemBlend");
  private static int kAnimIsMoving = Animator.StringToHash("IsMoving");
  private static int kAnimIsStunned = Animator.StringToHash("IsStunned");
  private static int kAnimAttack = Animator.StringToHash("Attack");
  private static int kAnimRecoil = Animator.StringToHash("Recoil");

  public void Interact()
  {
    if (_interactionController.ClosestInteractable != null)
    {
      _interactionController.TriggerInteraction();
    }
    else if (_heldItem != null)
    {
      DropItem();
    }
    else
    {
      Attack();
    }
  }

  public void Attack()
  {
    _animator.SetTrigger(kAnimAttack);
  }

  private void OnEnable()
  {
    _interactionController.InteractionTriggered += OnInteractionTriggered;
    _slappable.Slapped += OnSlapped;
    _animatorCallback.AddCallback("OnAttackFrame", OnAttackFrame);
  }

  private void OnDisable()
  {
    _interactionController.InteractionTriggered -= OnInteractionTriggered;
    _slappable.Slapped -= OnSlapped;
    _animatorCallback.RemoveCallback("OnAttackFrame", OnAttackFrame);
  }

  private void Update()
  {
    // Calculate next position based on movement
    float moveAxisTotal = Mathf.Clamp01(Mathf.Abs(MoveAxis) + Mathf.Abs(StrafeAxis));
    Vector3 moveVec = (transform.forward * MoveAxis + transform.right.WithY(0) * StrafeAxis).NormalizedSafe() * moveAxisTotal;
    Vector3 newPosition = transform.position + moveVec * _moveSpeed * Time.deltaTime;

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
    Vector3 velocity = newPosition - transform.position;
    if (Physics.SphereCast(transform.position, _obstacleRaycastRadius, velocity.NormalizedSafe(), out _obstacleRaycast, _minDistToObstacle + 1, _obstacleLayer))
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

    // Update animation
    float moveDir = Mathf.Sign(MoveAxis);
    _animator.SetFloat(kAnimMoveSpeed, moveAxisTotal * moveDir);
    _animator.SetBool(kAnimIsMoving, moveAxisTotal > 0);

    float holdItemBlendTarget = _heldItem != null ? 1 : 0;
    _holdItemBlend = Mathfx.Damp(_holdItemBlend, holdItemBlendTarget, 0.25f, Time.deltaTime * 3);
    _animator.SetFloat(kAnimHoldItemBlend, _holdItemBlend);

    // Apply movement
    transform.position = newPosition;
    transform.Rotate(Vector3.up, LookHorizontalAxis * _turnSpeed * Time.deltaTime, Space.Self);
    _cameraRoot.Rotate(Vector3.right, -LookVerticalAxis * _turnSpeed * Time.deltaTime, Space.Self);

    // Clamp camera look
    float verticalAngle = Vector3.SignedAngle(transform.forward, _cameraRoot.forward, transform.right);
    float delta = 0;
    if (verticalAngle < LookVerticalRange.MinValue)
      delta = verticalAngle - LookVerticalRange.MinValue;
    else if (verticalAngle > LookVerticalRange.MaxValue)
      delta = verticalAngle - LookVerticalRange.MaxValue;

    _cameraRoot.Rotate(Vector3.right, -delta, Space.Self);
  }

  private void OnSlapped(GameCharacterController fromCharacter)
  {
    _animator.SetTrigger(kAnimRecoil);
    DropItem();
  }

  private void OnAttackFrame()
  {
    int overlapCount = Physics.OverlapSphereNonAlloc(_attackPos.position, 0.25f, _overlapColliders, _attackLayer, QueryTriggerInteraction.Collide);
    for (int i = 0; i < overlapCount; ++i)
    {
      Collider c = _overlapColliders[i];
      Slappable slappable = c.GetComponentInParent<Slappable>();
      if (slappable != null && slappable != _slappable)
      {
        slappable.ReceiveSlap(fromCharacter: this);
      }
    }
  }

  private void PickupItem(ItemController item)
  {
    DropItem();
    item.transform.parent = _heldItemRoot;
    item.transform.SetIdentityTransformLocal();
    item.SetInteractable(false);
    _heldItem = item;
  }

  private void DropItem()
  {
    if (_heldItem != null)
    {
      _heldItem.SetInteractable(true);
      _heldItem.transform.parent = null;
      _heldItem.transform.localScale = Vector3.one;
      _heldItem = null;
    }
  }

  private void OnInteractionTriggered(Interactable interactable)
  {
    Debug.Log($"{name} interacted with {interactable.name}");
    ItemController item = interactable.GetComponent<ItemController>();
    if (item != null)
    {
      PickupItem(item);
    }

    PirateController pirate = interactable.GetComponent<PirateController>();
    if (pirate != null)
    {
      if (_heldItem != null)
      {
        ItemController giveItem = _heldItem;
        DropItem();

        if (giveItem.Type == ItemType.Tooth)
          pirate.AddTooth(giveItem);
        else if (giveItem.Type == ItemType.Food)
          pirate.AddFood(giveItem);
      }
    }
  }
}