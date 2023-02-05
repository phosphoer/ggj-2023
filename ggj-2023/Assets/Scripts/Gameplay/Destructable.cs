using UnityEngine;
using System.Collections;

public class Destructable : MonoBehaviour
{
  [SerializeField]
  private Slappable _slappable = null;

  [SerializeField]
  private GameObject _visualRoot = null;

  [SerializeField]
  private GameObject _destroyedRoot = null;

  private void Awake()
  {
    _visualRoot.SetActive(true);
    _destroyedRoot.SetActive(false);
  }

  private void OnEnable()
  {
    _slappable.Slapped += OnSlapped;
  }

  private void OnDisable()
  {
    _slappable.Slapped -= OnSlapped;
  }

  private void OnSlapped(GameCharacterController fromCharacter)
  {
    _visualRoot.SetActive(false);
    _destroyedRoot.SetActive(true);
    foreach (Transform piece in _destroyedRoot.transform)
    {
      Rigidbody rb = piece.GetComponent<Rigidbody>();
      rb.AddForce(Random.insideUnitSphere * 2, ForceMode.VelocityChange);
      rb.AddTorque(Random.insideUnitSphere * 2, ForceMode.VelocityChange);

      CoroutineRoot.Instance.StartCoroutine(DehydratePieceAsync(piece));
    }

    while (_destroyedRoot.transform.childCount > 0)
      _destroyedRoot.transform.GetChild(0).parent = null;

    Destroy(gameObject);
  }

  private static IEnumerator DehydratePieceAsync(Transform piece)
  {
    yield return Tween.WaitForTime(Random.Range(5f, 8f));
    yield return Tween.HermiteScale(piece, piece.localScale, Vector3.zero, Random.Range(1f, 3f));
    Destroy(piece.gameObject);
  }
}