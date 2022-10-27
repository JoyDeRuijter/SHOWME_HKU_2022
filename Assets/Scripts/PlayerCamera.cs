using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] private GameObject followCameraPrefab;
    private CinemachineVirtualCamera followCamera;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        SpawnCameraServerRpc();
    }

    [ServerRpc]
    private void SpawnCameraServerRpc()
    {
        followCamera = Instantiate(followCameraPrefab).GetComponent<CinemachineVirtualCamera>();
        followCamera.gameObject.GetComponent<NetworkObject>().Spawn(true);
        followCamera.Follow = this.transform;
    }
}
