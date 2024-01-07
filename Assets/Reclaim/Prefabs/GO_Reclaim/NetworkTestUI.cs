using Unity.Netcode;
using UnityEngine;

namespace UI.Networking
{
    public class NetworkTestUI : MonoBehaviour
    {
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
            }

            GUILayout.EndArea();
        }

        static void StartButtons()
        {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();

        }

        static void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);

            if(GUILayout.Button("Spawn Monster"))
            {
                if (NetworkManager.Singleton.IsServer)
                    Instantiate(Resources.Load<GameObject>("Goblin"), new Vector3(Random.Range(0, 10), Random.Range(0, 10), 0f), Quaternion.identity)
                        .GetComponent<NetworkObject>().Spawn();
                else
                    Debug.LogError("Attempting to spawn from a client.");
            }

            if(GUILayout.Button("Spawn Item"))
            {
                //...
            }
        }
    }
}