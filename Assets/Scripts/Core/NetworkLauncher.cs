using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using KanjiFlipGame.Core;

namespace KanjiFlipGame.Network
{
    /// <summary>
    /// Photon Fusionを使用した接続とマッチングを管理するクラス
    /// </summary>
    public class NetworkLauncher : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private NetworkRunner _runnerPrefab;
        private NetworkRunner _currentRunner;

        public static NetworkLauncher Instance { get; private set; }

        public bool IsMaster => _currentRunner != null && _currentRunner.IsSharedModeMasterClient;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// ランダムマッチを開始
        /// </summary>
        public async void StartRandomMatch(PlayerRole role, string topic = "")
        {
            await StartGame(GameMode.Shared, null, role, topic);
        }

        /// <summary>
        /// フレンドマッチを開始（ルームID指定）
        /// </summary>
        public async void StartFriendMatch(string roomId, PlayerRole role, string topic = "")
        {
            await StartGame(GameMode.Shared, roomId, role, topic);
        }

        private async Task StartGame(GameMode mode, string roomName, PlayerRole role, string topic)
        {
            if (_currentRunner != null)
            {
                await _currentRunner.Shutdown();
            }

            _currentRunner = Instantiate(_runnerPrefab);
            _currentRunner.AddCallbacks(this);

            var result = await _currentRunner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = roomName, // nullの場合はランダムマッチ
                SceneManager = _currentRunner.gameObject.AddComponent<NetworkSceneManagerDefault>()
            });

            if (result.Ok)
            {
                Debug.Log($"接続成功: Room={_currentRunner.SessionInfo.Name}");
                // 入室成功。GameManagerの同期を待つ
            }
            else
            {
                Debug.LogError($"接続失敗: {result.ShutdownReason}");
            }
        }

        #region INetworkRunnerCallbacks (最低限の実装)

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { Debug.Log($"Player Joined: {player}"); }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

        #endregion
    }
}
