using UnityEngine;
using System.Collections;

public enum ItemType
{
  Tooth,
  Food
}

public class ItemController : MonoBehaviour
{
  public event System.Action<ItemController> ToothDestroyed;

  public Interactable Interactable => _interactable;
  public ItemType Type => _type;
  public float ToothHealth => _toothHealth;
  public float ToothChompPower => _toothChompPower;
  public float FoodWeightValue => _foodWeightValue;
  public float FoodChompCount => _foodChompCount;
  public float FoodToothDamage => _foodToothDamage;

  [SerializeField]
  private Interactable _interactable = null;

  [SerializeField]
  private ItemType _type = default;

  [SerializeField]
  private float _toothHealthMax = 1;

  [SerializeField]
  private float _toothChompPower = 1;

  [SerializeField]
  private float _foodWeightValue = 1;

  [SerializeField]
  private float _foodChompCount = 1;

  [SerializeField]
  private float _foodToothDamage = 1;

  private float _toothHealth;

  public void DamageTooth(float damage)
  {
    _toothHealth -= damage;
    StartCoroutine(DamageAnimAsync());

    if (_toothHealth <= 0)
    {
      ToothDestroyed?.Invoke(this);
    }
  }

  private void Awake()
  {
    _toothHealth = _toothHealthMax;
  }

  private IEnumerator DamageAnimAsync()
  {
    yield return Tween.CustomTween(0.75f, t =>
    {
      transform.localEulerAngles += Random.insideUnitSphere * 150 * Time.deltaTime;
    });

    Quaternion startRot = transform.localRotation;
    yield return Tween.CustomTween(0.25f, t =>
    {
      transform.localRotation = Quaternion.Slerp(startRot, Quaternion.identity, t);
    });
  }
}