using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PirateController : MonoBehaviour
{
  public event System.Action<PlayerCharacterController> PirateFull;

  public float FoodWeight => _foodWeight;
  public float FullPercent => Mathf.Clamp01(_foodWeight / _desiredFoodWeight);
  public float EatPercent => _currentFood != null ? Mathf.Clamp01(_chompCount / _currentFood.FoodChompCount) : 0;

  [SerializeField]
  private Transform[] _teethRoots = null;

  [SerializeField]
  private ParticleSystem _fxChomp = null;

  [SerializeField]
  private float _desiredFoodWeight = 10;

  private float _foodWeight;
  private float _chompCount;
  private float _chompTimer;
  private ItemController _currentFood;
  private PlayerCharacterController _assignedPlayer;

  private List<ItemController> _teeth = new List<ItemController>();

  public void AssignPlayer(PlayerCharacterController player)
  {
    _assignedPlayer = player;
  }

  public void NotifyPirateFull()
  {
    PirateFull?.Invoke(_assignedPlayer);
  }

  public void AddFood(ItemController foodItem)
  {
    if (_currentFood == null)
    {
      if (foodItem.Type == ItemType.Food)
      {
        Debug.Log($"{name} is eating {foodItem.name}");
        _currentFood = foodItem;
        _chompCount = 0;
      }
      else
      {
        Debug.LogWarning($"Can't eat {foodItem.name}, it is a tooth!");
      }
    }
    else
    {
      Debug.LogWarning($"Can't eat {foodItem.name}, still eating {_currentFood.name}");
    }
  }

  public void AddTooth(ItemController toothItem)
  {
    if (toothItem.Type == ItemType.Tooth)
    {
      Transform emptyTooth = GetEmptyTooth();
      if (emptyTooth != null)
      {
        _teeth.Add(toothItem);
        toothItem.transform.parent = emptyTooth;
        toothItem.transform.SetIdentityTransformLocal();
        toothItem.Interactable.enabled = false;
        toothItem.ToothDestroyed += OnToothDestroyed;

        UIHydrate.Hydrate(toothItem.transform);
      }
    }
    else
    {
      Debug.LogWarning($"Can't add {toothItem.name} as a tooth, it is food!");
    }
  }

  private void Update()
  {
    if (_currentFood != null && _teeth.Count > 0)
    {
      _chompTimer += Time.deltaTime;
      if (_chompTimer >= 0.5f)
      {
        Chomp();
        _chompTimer = 0;
      }
    }
  }

  private void OnToothDestroyed(ItemController tooth)
  {
    tooth.ToothDestroyed -= OnToothDestroyed;
    _teeth.Remove(tooth);
    Destroy(tooth.gameObject);
  }

  private void Chomp()
  {
    // Apply tooth chomp power
    foreach (var tooth in _teeth)
    {
      _chompCount += tooth.ToothChompPower;
    }

    Debug.Log($"Chomp count: {_chompCount}");

    // Damage random tooth
    var randomTooth = _teeth[Random.Range(0, _teeth.Count)];
    randomTooth.DamageTooth(_currentFood.FoodToothDamage);

    _fxChomp.Play();

    // Finish eating food if chomping is done
    if (_chompCount >= _currentFood.FoodChompCount)
    {
      SwallowFood();
    }
  }

  private void SwallowFood()
  {
    Debug.Log($"{name} has finished eating {_currentFood.name}");

    _foodWeight += _currentFood.FoodWeightValue;
    _currentFood = null;
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