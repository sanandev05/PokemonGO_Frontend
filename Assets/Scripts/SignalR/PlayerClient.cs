using Microsoft.AspNetCore.SignalR.Client;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerClient : MonoBehaviour
{
    // The URL of your SignalR Hub. Replace with your actual backend URL.
    // Example: "http://localhost:5000/gamehub"
    [SerializeField]
    private string hubUrl = "https://localhost:7156/gameHub";

    // The HubConnection object to manage the connection to the SignalR hub.
    private HubConnection hubConnection;

    // Use async void for the Start method to allow for asynchronous operations.
    // You could also call a separate async method from Start().
    private async void Start()
    {
        // Build the connection to the SignalR hub.
        hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .Build();

        // This is where we define the method to catch the "ReceivePlayerState" message.
        // The method signature here must match the parameters sent from the server.
        // The order and type of the parameters are crucial.
        hubConnection.On<string, float, float, float, float, float, float, string>("ReceivePlayerState", (userName, posX, posY, posZ, rotX, rotY, rotZ, animationState) =>
        {
            // This code block will execute every time the "ReceivePlayerState" message is received.
            // You can use this data to update the state of another player's object in your scene.
            Debug.Log($"Received Player State from: {userName}");
            Debug.Log($"Position: ({posX}, {posY}, {posZ})");
            Debug.Log($"Rotation: ({rotX}, {rotY}, {rotZ})");
            Debug.Log($"Animation: {animationState}");

            // Example of what you might do with the received data:
            // Find the other player's GameObject by name (or some other identifier).
            // GameObject player = GameObject.Find(userName);
            // if (player != null)
            // {
            //     // Update the player's position, rotation, and animation.
            //     player.transform.position = new Vector3(posX, posY, posZ);
            //     player.transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
            //     // Example animation update (you would need a reference to the Animator component)
            //     // player.GetComponent<Animator>().SetTrigger(animationState);
            // }
        });

        // Start the connection to the hub.
        try
        {
            await hubConnection.StartAsync();
            Debug.Log("SignalR Connection Started Successfully!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error starting SignalR connection: {ex.Message}");
        }
    }

    // It's good practice to stop the connection when the GameObject is destroyed.
    private async void OnDestroy()
    {
        if (hubConnection != null)
        {
            await hubConnection.StopAsync();
            Debug.Log("SignalR Connection Stopped.");
        }
    }

    // You can also create a public method to send data to the hub.
    // For example, to send your player's state back to the server.
    public async Task SendPlayerState(string userName, Vector3 position, Vector3 rotation, string animationState)
    {
        if (hubConnection != null && hubConnection.State == HubConnectionState.Connected)
        {
            await hubConnection.InvokeAsync("UpdatePlayerState",
                userName,
                position.x, position.y, position.z,
                rotation.x, rotation.y, rotation.z,
                animationState);
        }
    }
}
