using UnityEngine;
using Fusion;

public class NetworkBulletManager : NetworkBehaviour
{
    public static NetworkBulletManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("[EffectManager] Awake: instance assigned.");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /*
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayerEffect(GameObject hitPrefab, Vector3 vector3)
    {
        EffectManager.instance.PlayEffect(hitPrefab, vector3);
    }
    */
}
