using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }
                
    public void PlayEffect(GameObject effectPrefab, Vector3 position)
    {
        GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
        Destroy(effect, 1);
    }

    public void PlayEffecting(GameObject effectPrefab, Transform target)
    {
        if (effectPrefab == null || target == null) return;

        PlayEffect(effectPrefab, target.position);
    }
}
