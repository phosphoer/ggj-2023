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

  [SerializeField]
  private GameObject[] _lootSpawnTable = null;

  [SerializeField]
  private RangedInt _lootSpawnCount = default;

  [SerializeField]
  private SoundBank _destroySound = null;

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
    if (_destroySound != null)
    {
      AudioManager.Instance.PlaySound(_destroySound);
    }

    _visualRoot.SetActive(false);
    _destroyedRoot.SetActive(true);
    foreach (Transform piece in _destroyedRoot.transform)
    {
      Rigidbody rb = piece.GetComponent<Rigidbody>();
      rb.AddForce(Random.insideUnitSphere * 4, ForceMode.VelocityChange);
      rb.AddTorque(Random.insideUnitSphere * 4, ForceMode.VelocityChange);

      CoroutineRoot.Instance.StartCoroutine(DehydratePieceAsync(piece));
    }

    while (_destroyedRoot.transform.childCount > 0)
      _destroyedRoot.transform.GetChild(0).parent = null;

    int spawnCount = _lootSpawnCount.RandomValue;
    for (int i = 0; i < spawnCount; ++i)
    {
      GameObject spawnPrefab = _lootSpawnTable[Random.Range(0, _lootSpawnTable.Length)];
      GameObject spawnInstance = Instantiate(spawnPrefab, transform.parent);
      spawnInstance.transform.position = transform.position + Vector3.up * 0.5f + Random.insideUnitSphere * 0.3f;
      spawnInstance.transform.rotation = Random.rotationUniform;
    }

    Destroy(gameObject);
  }

  private static IEnumerator DehydratePieceAsync(Transform piece)
  {
    yield return Tween.WaitForTime(Random.Range(5f, 8f));
    yield return Tween.HermiteScale(piece, piece.localScale, Vector3.zero, Random.Range(1f, 3f));
    Destroy(piece.gameObject);
  }
}