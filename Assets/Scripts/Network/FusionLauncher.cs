using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class FusionLauncher : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner runnerPrefab;
    [SerializeField] private string sessionName = "PokerRoom01";
    [SerializeField] private string gameplayScenePath = "Assets/Scenes/CasinoRoom.unity";

    private NetworkRunner _runner;

    public async void StartHost()
    {
        await StartGame(GameMode.Host);
    }

    public async void StartClient()
    {
        await StartGame(GameMode.Client);
    }

    private async Task StartGame(GameMode mode)
    {
        if (_runner != null)
        {
            Debug.LogWarning("A NetworkRunner is already running.");
            return;
        }

        _runner = Instantiate(runnerPrefab);
        _runner.name = "NetworkRunner";

        _runner.ProvideInput = true;
        _runner.AddCallbacks(this);

        NetworkSceneManagerDefault sceneManager = _runner.GetComponent<NetworkSceneManagerDefault>();
        if (sceneManager == null)
        {
            sceneManager = _runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        StartGameArgs startGameArgs = new StartGameArgs
        {
            GameMode = mode,
            SessionName = sessionName,
            SceneManager = sceneManager
        };

        StartGameResult result = await _runner.StartGame(startGameArgs);

        if (!result.Ok)
        {
            Debug.LogError($"Failed to start game: {result.ShutdownReason}");
            return;
        }

        Debug.Log($"Fusion started successfully as {mode}.");

        if (mode == GameMode.Host)
        {
            int sceneIndex = SceneUtility.GetBuildIndexByScenePath(gameplayScenePath);

            if (sceneIndex < 0)
            {
                Debug.LogError($"Scene not found in Build Settings: {gameplayScenePath}");
                return;
            }

            Debug.Log($"Host loading gameplay scene: {gameplayScenePath}");
            await _runner.LoadScene(SceneRef.FromIndex(sceneIndex), LoadSceneMode.Single);
        }
    }

    public void ShutdownRunner()
    {
        if (_runner == null)
        {
            return;
        }

        _runner.Shutdown();
        _runner = null;
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player joined: {player}");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player left: {player}");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        PlayerNetworkInput playerInput = new PlayerNetworkInput();

        Vector2 move = Vector2.zero;

        Keyboard keyboard = Keyboard.current;

        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed)
            {
                move.y += 1f;
            }

            if (keyboard.sKey.isPressed)
            {
                move.y -= 1f;
            }

            if (keyboard.dKey.isPressed)
            {
                move.x += 1f;
            }

            if (keyboard.aKey.isPressed)
            {
                move.x -= 1f;
            }
        }

        playerInput.Move = move;
        input.Set(playerInput);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"Runner shutdown: {shutdownReason}");

        if (_runner == runner)
        {
            _runner = null;
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server.");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.LogWarning($"Disconnected from server: {reason}");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.LogError($"Connect failed: {reason}");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("Scene load start.");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("Scene load done.");
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }
}