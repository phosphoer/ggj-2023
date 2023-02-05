using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
  [SerializeField]
  private GameObject[] _droptable = default;

  [SerializeField]
  private RangedFloat _respawnTimer = new RangedFloat(10, 15);

  private GameObject _spawnedObject;
  private float _spawnTimer;

  private void Update()
  {
    _spawnTimer -= Time.deltaTime;
    if (_spawnTimer < 0)
    {
      _spawnTimer = _respawnTimer.RandomValue;
      if (_spawnedObject == null)
      {
        _spawnedObject = SpawnItem();
      }
    }
  }

  public GameObject SpawnItem()
  {
    if (_droptable.Length == 0)
      return null;

    int randomIndex = Random.Range(0, _droptable.Length);
    GameObject itemTemplate = _droptable[randomIndex];
    GameObject item = Instantiate(itemTemplate, transform);

    return item;
  }
}
