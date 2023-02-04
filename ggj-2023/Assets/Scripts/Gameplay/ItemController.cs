using UnityEngine;

public class ItemController : MonoBehaviour
{
  public Interactable Interactable => _interactable;

  [SerializeField]
  private Interactable _interactable = null;
}