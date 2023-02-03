using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Interactable : MonoBehaviour
{
  public static int InstanceCount => _instances.Count;
  public static IEnumerable<Interactable> Instances => _instances;

  public event System.Action InteractionTriggered;
  public event System.Action PromptShown;
  public event System.Action PromptHidden;

  public float InteractionRadius
  {
    get { return _interactionRadius; }
    set { _interactionRadius = value; }
  }

  public bool IsInteractionEnabled
  {
    get { return _disabledStack.Count == 0 && _enableInteraction && enabled; }
  }

  public bool AutoInteract
  {
    get { return _autoInteract; }
    set { _autoInteract = value; }
  }

  public bool EnableInteraction
  {
    get => _enableInteraction;
    set => _enableInteraction = true;
  }

  public bool RequiresLineOfSight => _requiresLineOfSight;
  public bool IsPromptShown => _uiRoot != null;

  [SerializeField]
  private InteractableUI _interactableUIPrefab = null;

  [SerializeField]
  private Transform _interactionUIAnchor = null;

  [SerializeField]
  private float _interactionUIHeight = 8.0f;

  [SerializeField]
  private string _interactionText = "Interact";

  [SerializeField]
  private float _interactionRadius = 50.0f;

  [SerializeField]
  private bool _requiresLineOfSight = false;

  [SerializeField]
  private bool _autoInteract = false;

  [SerializeField]
  private bool _enableInteraction = true;

  [SerializeField]
  private bool _disableOnInteract = false;

  private InteractableUI _interactableUI;
  private RectTransform _uiRoot;
  private List<string> _disabledStack = new List<string>();

  private static List<Interactable> _instances = new List<Interactable>();

  public static Interactable GetInstance(int instanceIndex)
  {
    return _instances[instanceIndex];
  }

  public static void HideAll()
  {
    foreach (var instance in _instances)
      instance.HidePrompt();
  }

  private void OnEnable()
  {
    _instances.Add(this);

    if (_interactionUIAnchor == null)
    {
      _interactionUIAnchor = transform;
    }
  }

  private void OnDisable()
  {
    _instances.Remove(this);

    HidePrompt();
  }

  public IEnumerator WaitForInteractAsync(bool enableAndDisable = true)
  {
    bool didInteract = false;
    System.Action onInteract = () =>
    {
      didInteract = true;
    };

    InteractionTriggered += onInteract;

    if (enableAndDisable)
      enabled = true;

    while (!didInteract)
      yield return null;

    InteractionTriggered -= onInteract;

    if (enableAndDisable)
      enabled = false;
  }

  public void PushDisabledState()
  {
    _disabledStack.Add(string.Empty);
  }

  public void PopDisabledState()
  {
    if (_disabledStack.Count > 0)
    {
      _disabledStack.RemoveAt(_disabledStack.Count - 1);
      RefreshText();
    }
  }

  public void PushDisabledState(string disabledText)
  {
    _disabledStack.Add(disabledText);
    RefreshText();
  }

  public void PopDisabledState(string disabledText)
  {
    _disabledStack.Remove(disabledText);
    RefreshText();
  }

  public void TriggerInteraction()
  {
    if (IsInteractionEnabled)
    {
      InteractionTriggered?.Invoke();
      HidePrompt();

      if (_disableOnInteract)
        enabled = false;
    }
  }

  public void ShowPrompt()
  {
    if (_uiRoot != null)
    {
      Debug.LogWarning("Skipping show prompt - already open");
      return;
    }

    if (!AutoInteract)
    {
      // _uiRoot = PlayerUI.Instance.WorldAttachedUI.ShowItem(_interactionUIAnchor, Vector3.up * _interactionUIHeight);
      _interactableUI = Instantiate(_interactableUIPrefab, _uiRoot);
    }

    RefreshText();

    PromptShown?.Invoke();
  }

  public void HidePrompt()
  {
    if (_uiRoot != null)
    {
      // PlayerUI.Instance.WorldAttachedUI.HideItem(_uiRoot);
      _uiRoot = null;
      _interactableUI = null;
    }

    PromptHidden?.Invoke();
  }

  private void RefreshText()
  {
    if (_interactableUI != null)
    {
      if (_disabledStack.Count > 0)
      {
        _interactableUI.InteractionText = _disabledStack[_disabledStack.Count - 1];
        _interactableUI.gameObject.SetActive(!string.IsNullOrEmpty(_interactableUI.InteractionText));
      }
      else
      {
        _interactableUI.InteractionText = _interactionText;
        _interactableUI.gameObject.SetActive(true);
      }
    }
  }

#if UNITY_EDITOR
  private void OnDrawGizmosSelected()
  {
    if (_interactionUIAnchor != null)
    {
      Gizmos.color = Color.white;
      Gizmos.DrawWireSphere(transform.position + Vector3.up * _interactionUIHeight, 0.1f);
    }
  }
#endif
}