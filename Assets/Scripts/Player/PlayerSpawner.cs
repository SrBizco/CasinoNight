using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private float spawnSpacing = 3f;
    [SerializeField] private Vector3 spawnOrigin = Vector3.zero;

    private readonly Dictionary<PlayerRef, NetworkObject> spawnedPlayers = new();
    private int _nextSpawnIndex = 0;

    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log($"PlayerJoined callback received for {player}. Runner is server: {Runner.IsServer}");

        if (Runner == null || Runner.IsServer == false)
        {
            return;
        }

        Vector3 spawnPosition = spawnOrigin + new Vector3(_nextSpawnIndex * spawnSpacing, 0f, 0f);
        _nextSpawnIndex++;

        NetworkObject playerObject = Runner.Spawn(
            playerPrefab,
            spawnPosition,
            Quaternion.identity,
            player
        );

        spawnedPlayers[player] = playerObject;
        Runner.SetPlayerObject(player, playerObject);

        Debug.Log($"Spawned player avatar for {player} at {spawnPosition}");
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (Runner == null || Runner.IsServer == false)
        {
            return;
        }

        if (spawnedPlayers.TryGetValue(player, out NetworkObject playerObject))
        {
            Runner.Despawn(playerObject);
            spawnedPlayers.Remove(player);
            Debug.Log($"Despawned player avatar for {player}");
        }
    }
}