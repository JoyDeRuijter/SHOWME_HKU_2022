using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private Transform spawnedObjectPrefab;
    private Transform spawnedObjectTransform;

    private int userID = 1;
    private string userName = "default_user";

    private NetworkVariable<MyCustomNetworkData> N_userData = new NetworkVariable<MyCustomNetworkData>(
        new MyCustomNetworkData
        {
            _userID = 0,
            _userName = "unknown_user",
            _isActive = false,
        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public struct MyCustomNetworkData : INetworkSerializable
    {
        public int _userID;
        public FixedString32Bytes _userName;
        public bool _isActive;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _userID);
            serializer.SerializeValue(ref _userName);
            serializer.SerializeValue(ref _isActive);
        }
    }

    public override void OnNetworkSpawn()
    {
        N_userData.OnValueChanged += (MyCustomNetworkData _previousValue, MyCustomNetworkData _newValue) =>
        {
            Debug.Log($"{OwnerClientId} has userID: {_newValue._userID}, isActive is: {_newValue._isActive}" +
                $" and has userName: {_newValue._userName}");
            userID = _newValue._userID;
        };
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        if (Input.GetKey(KeyCode.T))
        {
            // SPAWNING NETWORK OBJECTS (ONLY WORKS FROM THE SERVER)
            // IF YOU WANT TO SPAWN IT FROM A CLIENT, USE SERVERRPC TO TELL THE SERVER TO SPAWN THAT OBJECT
            // AND SEND IT OVER TO THE CLIENT
            spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
            spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);

            // USE OF SERVER RPC (SEND SOMETHING FROM THE CLIENT TO THE SERVER)
            // (WHEN YOU DON'T WANT THE CLIENT TO HAVE ANY OWNERSHIP OF THE SERVER)
            //TestServerRpc("Test message", new ServerRpcParams());

            // USE OF CLIENT RPC (SEND SOMETHING FROM THE SERVER TO THE CLIENT
            // (CAN ONLY BE RUN FROM THE SERVER)
            // (CAN SEND SOMETHING TO (A) SPECIFIC CLIENT(S), IN THIS CASE TO THE CLIENT WITH ID 1)
            //TestClientRpc(new ClientRpcParams { 
            //    Send = new ClientRpcSendParams
            //    {
            //        TargetClientIds = new List<ulong> { 1 }
            //    }
            //});

            // USE OF NETWORK VARIABLES, IN THIS CASE A STRUCT (WHEN YOU DO TRUST THE CLIENT WITH THE SERVER)
            //N_userData.Value = new MyCustomNetworkData
            //{
            //    _userID = userID,
            //    _userName = userName,
            //    _isActive = true,
            //};
        }

        if (Input.GetKey(KeyCode.G))
        {
            // DESPAWNING NETWORK OBJECTS
            Destroy(spawnedObjectTransform.gameObject);
        }

        Vector3 moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveDir = Vector3.forward;
        if (Input.GetKey(KeyCode.A)) moveDir = Vector3.left;
        if (Input.GetKey(KeyCode.S)) moveDir = Vector3.back;
        if (Input.GetKey(KeyCode.D)) moveDir = Vector3.right;

        float moveSpeed = 3f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    // Runs only on the server, not on the client
    [ServerRpc]
    private void TestServerRpc(string _message, ServerRpcParams _serverRpcParams)
    {
        Debug.Log($"TestServerRPC OwnderClientID: {OwnerClientId}, SenderClientID: {_serverRpcParams.Receive.SenderClientId}");
    }

    [ClientRpc]
    private void TestClientRpc(ClientRpcParams _clientRpcParams)
    {
        Debug.Log($"TestClientRPC ");
    }
}
