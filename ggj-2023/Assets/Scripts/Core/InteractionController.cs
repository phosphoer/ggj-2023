using UnityEngine;
using System.Collections.Generic;

public class InteractionController : MonoBehaviour
{
  public event System.Action<Interactable> InteractionTriggered;

  public Transform TrackedTransform
  {
    get { return _trackedTransform; }
    set { _trackedTransform = value; }
  }

  public PlayerUI PlayerUI;

  public Interactable ClosestInteractable => _closestInteractable;

  [SerializeField]
  private Transform _trackedTransform = null;

  private int _lazyUpdateIndex;
  private Interactable _closestInteractable;
  private InteractableUI _interactableUI;

  //$HACK -- Game specific kludge
  private PlayerCharacterController _ownerPlayer;
  //$HACK -- Game specific kludge

  public void TriggerInteraction()
  {
    if (_closestInteractable != null)
    {
      _closestInteractable.TriggerInteraction(this);
    }
  }

  private void OnDisable()
  {
    SetClosestInteractable(null);
  }

  private void Start()
  {
    //$HACK -- Game specific kludge
    _ownerPlayer= gameObject.GetComponent<PlayerCharacterController>();
    //$HACK -- Game specific kludge
  }

  private void Update()
  {
    if (_closestInteractable != null && (!_closestInteractable.enabled || !_closestInteractable.gameObject.activeInHierarchy))
    {
      SetClosestInteractable(null);
    }

    if (_lazyUpdateIndex < Interactable.Instances.Count)
    {
      // Check if the current interactable is still in range
      float distToClosest = Mathf.Infinity;
      if (_closestInteractable != null)
      {
        distToClosest = Vector3.Distance(_trackedTransform.position, _closestInteractable.transform.position);
        bool isInLightOfSight = IsInLineOfSight(_closestInteractable);
        if (distToClosest >= _closestInteractable.InteractionRadius || !isInLightOfSight || !_closestInteractable.enabled)
        {
          SetClosestInteractable(null);
          distToClosest = Mathf.Infinity;
        }
      }

      // If the interactable does not have interaction enabled, any other interactable in range should 
      // take priority
      if (_closestInteractable != null && !_closestInteractable.IsInteractionEnabled)
      {
        distToClosest = Mathf.Infinity;
      }

      // Get the distance to the next potential interactable
      Interactable interactable = Interactable.Instances[_lazyUpdateIndex];
      Vector3 toInteractable = interactable.transform.position - _trackedTransform.position;
      float distToInteractable = toInteractable.magnitude;

      // Decide if this interactable is more contextual than the current one
      bool isInteractableMoreContextual = distToInteractable < distToClosest;
      isInteractableMoreContextual &= distToInteractable < interactable.InteractionRadius;
      isInteractableMoreContextual &= interactable != _closestInteractable;
      if (_closestInteractable != null && !interactable.IsInteractionEnabled)
      {
        isInteractableMoreContextual = false;
      }

      // If the new interactable is more contextual than the previous, make it the highlighted one
      if (isInteractableMoreContextual)
      {
        // Make this interactable the current one, if it was in line of sight
        if (CanInteractWith(interactable) && IsInLineOfSight(interactable))
        {
          SetClosestInteractable(interactable);
        }
      }
    }
    else
    {
      SetClosestInteractable(null);
    }

    if (Interactable.Instances.Count > 0)
    {
      _lazyUpdateIndex = (_lazyUpdateIndex + 1) % Interactable.Instances.Count;
    }
  }

  //$HACK -- Game specific kludge
  private bool CanInteractWith(Interactable interactable)
  {
    // See if the interaction being considered is a pirate
    PirateController pirate= interactable.gameObject.GetComponent<PirateController>();
    if (pirate != null)
    {
      // only allow if the pirate is owned by the same player that owns this interaction controller
      return pirate.AssignedPlayerController == _ownerPlayer;
    }

    return true;
  }
  //$HACK -- Game specific kludge

  private void OnInteractionTriggered(InteractionController _)
  {
    Debug.Log($"OnInteractionTriggered - {_closestInteractable.name}");
    InteractionTriggered?.Invoke(_closestInteractable);
  }

  private void SetClosestInteractable(Interactable interactable)
  {
    if (_closestInteractable != null)
    {
      _closestInteractable.InteractionTriggered -= OnInteractionTriggered;
      HidePrompt();
      _closestInteractable = null;
    }

    if (interactable != null)
    {
      _closestInteractable = interactable;
      _closestInteractable.InteractionTriggered += OnInteractionTriggered;
      ShowPrompt(_closestInteractable);
    }
  }

  private void ShowPrompt(Interactable interactable)
  {
    if (PlayerUI != null)
    {
      var uiRoot = PlayerUI.OnScreenUI.ShowItem(interactable.InteractionUIAnchor, Vector3.up * interactable.InteractionUIHeight);
      _interactableUI = Instantiate(interactable.InteractableUIPrefab, uiRoot);
      _interactableUI.transform.SetIdentityTransformLocal();
      _interactableUI.InteractionText = interactable.InteractionText;
    }
  }

  private void HidePrompt()
  {
    if (PlayerUI != null)
    {
      if (_interactableUI != null)
        PlayerUI.OnScreenUI.HideItem(_interactableUI.transform.parent as RectTransform);

      _interactableUI = null;
    }
  }

  private bool IsInLineOfSight(Interactable interactable)
  {
    if (!interactable.RequiresLineOfSight)
      return true;

    // If the interactable requires line of sight to be interacted with, cast a ray and mark it not interactable if we 
    // hit something other than ourselves
    bool inLightOfSight = true;
    RaycastHit hitInfo = default(RaycastHit);
    Vector3 fromPos = _trackedTransform.position.WithY(interactable.transform.position.y);
    Vector3 visibilityRay = interactable.transform.position - fromPos;
    if (Physics.Raycast(fromPos, visibilityRay.normalized, out hitInfo, visibilityRay.magnitude))
    {
      if (!hitInfo.collider.transform.IsChildOf(_trackedTransform) && !hitInfo.collider.transform.IsChildOf(interactable.transform))
        inLightOfSight = false;
    }

    return inLightOfSight;
  }
}