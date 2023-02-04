using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
  [SerializeField]
  private GameObject[] _droptable = default;

  void Start()
  {
    SelectItem();
  }

  public GameObject SelectItem()
  {
    if (_droptable.Length == 0)
      return null;

    int randomIndex = Random.Range(0, _droptable.Length); 
    GameObject itemTemplate = _droptable[randomIndex];
    GameObject item= Instantiate(itemTemplate, transform);

    return item;
  }
}
