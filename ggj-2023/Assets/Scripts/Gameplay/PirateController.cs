using UnityEngine;
using System.Collections.Generic;

public class PirateController : MonoBehaviour
{
  [SerializeField]
  private Transform[] _teethRoots = null;

  private List<ItemController> _teeth = new List<ItemController>();

  public void AddTooth(ItemController toothItem)
  {
    Transform emptyTooth = GetEmptyTooth();
    if (emptyTooth != null)
    {
      _teeth.Add(toothItem);
      toothItem.transform.parent = emptyTooth;
      toothItem.transform.SetIdentityTransformLocal();
      toothItem.Interactable.enabled = false;

      UIHydrate.Hydrate(toothItem.transform);
    }
  }

  private Transform GetEmptyTooth()
  {
    for (int i = 0; i < _teethRoots.Length; ++i)
    {
      var toothRoot = _teethRoots[i];
      if (toothRoot.childCount == 0)
        return toothRoot;
    }

    return null;
  }
}