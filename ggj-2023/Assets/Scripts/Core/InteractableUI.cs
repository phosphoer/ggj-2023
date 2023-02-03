using UnityEngine;
using UnityEngine.UI;

public class InteractableUI : MonoBehaviour
{
  public string InteractionText
  {
    get { return _interactionTextUI.text; }
    set { _interactionTextUI.text = value; }
  }

  [SerializeField]
  private Text _interactionTextUI = null;
}