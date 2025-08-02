using Microsoft.AspNetCore.SignalR.Client;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignalRClient : MonoBehaviour
{
    [NonSerialized]
    private HubConnection connection;
    public TextMeshProUGUI messageText;
    public Button _sendBtn;
    public TMP_InputField userInput;
    private string latestMessage = "";

    public GenericApiService<TrainerDTO> _trainerService;
    async void Start()
    {
        _trainerService= new GenericApiService<TrainerDTO>(ConstDatas.TrainerApiUrl);
        connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7156/chatHub") // replace with your actual URL
            .WithAutomaticReconnect()
            .Build();

        connection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            latestMessage = $"{user}: {message}";
        });

        try
        {
            await connection.StartAsync();
            latestMessage = "Connected to Server!";
        }
        catch (System.Exception ex)
        {
            latestMessage = $"❌ Error: {ex.Message}";
        }

        _sendBtn.onClick.AddListener(() => SendMessageToHub("Sanan", "Deneme"));
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(latestMessage) && messageText != null)
        {
            messageText.text += "\n" + latestMessage;
            latestMessage = ""; // clear after displaying
        }
    }

    public async void SendMessageToHub(string user, string message)
    {
        user = _trainerService.GetLocalCurrentTrainerDto().Name;
        message= userInput.text;
        if (connection != null && connection.State == HubConnectionState.Connected)
        {
            try
            {
                await connection.InvokeAsync("SendMessage", user, message);
            }
            catch (System.Exception ex)
            {
                latestMessage = $"❌ Send Error: {ex.Message}";
            }
        }
        else
        {
            latestMessage = "❌ Not connected to SignalR!";
        }
    }
}
