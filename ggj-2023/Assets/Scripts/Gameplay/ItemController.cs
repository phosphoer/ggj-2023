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
  private Rigidbody _rigidbody = null;

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

  public void PlayDamageAnim()
  {
    StartCoroutine(DamageAnimAsync());
  }

  public void DamageTooth(float damage)
  {
    _toothHealth -= damage;
    PlayDamageAnim();

    if (_toothHealth <= 0)
    {
      ToothDestroyed?.Invoke(this);
    }
  }

  public void SetInteractable(bool isInteractable)
  {
    _interactable.enabled = isInteractable;
    _rigidbody.isKinematic = !isInteractable;
  }

  private void Awake()
  {
    _toothHealth = _toothHealthMax;
  }

  private IEnumerator DamageAnimAsync()
  {
    yield return Tween.CustomTween(0.75f, t =>
    {
      transform.localPosition += Random.insideUnitSphere * Time.deltaTime;
      transform.localPosition = Mathfx.Damp(transform.localPosition, Vector3.zero, 0.25f, Time.deltaTime);
    });

    Vector3 startPos = transform.localPosition;
    yield return Tween.CustomTween(0.25f, t =>
    {
      transform.localPosition = Vector3.Slerp(startPos, Vector3.zero, t);
    });
  }
}