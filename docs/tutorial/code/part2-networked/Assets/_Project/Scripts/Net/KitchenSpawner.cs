using System.Collections.Generic;
using DumplingKitchen.Game;
using Unity.Netcode;
using UnityEngine;

namespace DumplingKitchen.Net
{
    /// <summary>
    /// Server-only: spawns the chef's avatar for the host and a dumpling for every other
    /// player, once that player has finished loading the kitchen.
    /// Place it in the Kitchen scene on a GameObject with a NetworkObject.
    /// </summary>
    public sealed class KitchenSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkObject chefAvatarPrefab;
        [SerializeField] private NetworkObject dumplingPrefab;
        [SerializeField] private Transform[] dumplingSpawnPoints;

        private readonly Dictionary<ulong, int> _playerIndexByClient = new();

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
                return;

            NetworkManager.SceneManager.OnSynchronizeComplete += HandleClientSynchronized;
            NetworkManager.OnClientDisconnectCallback += HandleClientDisconnected;

            SpawnChefAvatar();

            // Anyone already connected (e.g. after a scene reload) gets a dumpling now.
            foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
            {
                if (clientId != NetworkManager.ServerClientId)
                    SpawnDumpling(clientId);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer || NetworkManager == null)
                return;

            if (NetworkManager.SceneManager != null)
                NetworkManager.SceneManager.OnSynchronizeComplete -= HandleClientSynchronized;
            NetworkManager.OnClientDisconnectCallback -= HandleClientDisconnected;
        }

        private void HandleClientSynchronized(ulong clientId)
        {
            if (clientId != NetworkManager.ServerClientId)
                SpawnDumpling(clientId);
        }

        private void HandleClientDisconnected(ulong clientId)
        {
            // NGO despawns a player's PlayerObject automatically when they leave.
            _playerIndexByClient.Remove(clientId);
            UpdateDumplingCount();
        }

        private void SpawnChefAvatar()
        {
            NetworkObject avatar = Instantiate(chefAvatarPrefab);
            avatar.Spawn(destroyWithScene: true); // owned by the server = the chef
        }

        private void SpawnDumpling(ulong clientId)
        {
            if (_playerIndexByClient.ContainsKey(clientId))
                return;

            int index = LowestFreeIndex();
            _playerIndexByClient[clientId] = index;

            Transform spawn = dumplingSpawnPoints[index % dumplingSpawnPoints.Length];
            NetworkObject dumpling = Instantiate(dumplingPrefab, spawn.position, spawn.rotation);
            dumpling.SpawnAsPlayerObject(clientId, destroyWithScene: true);

            dumpling.GetComponent<NetworkDumpling>().PlayerIndex.Value = index;
            UpdateDumplingCount();
        }

        private int LowestFreeIndex()
        {
            int index = 0;
            while (_playerIndexByClient.ContainsValue(index))
                index++;
            return index;
        }

        private void UpdateDumplingCount()
        {
            if (RoundManager.Instance != null)
                RoundManager.Instance.SetDumplingCount(_playerIndexByClient.Count);
        }
    }
}
