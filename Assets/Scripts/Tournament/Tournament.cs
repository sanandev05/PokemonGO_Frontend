using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Microsoft.AspNetCore.SignalR.Client;

public class TournamentManager : MonoBehaviour
{
    [SerializeField] private GameObject battleUI;
    [SerializeField] private string hubUrl = "https://localhost:7156/tournamentHub";

    [SerializeField] private TextMeshProUGUI playerCounts;
    [SerializeField] private TextMeshProUGUI playerNames;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private GameObject joinButton;
    [SerializeField] private GameObject localPlayerGameObject;

    private HashSet<string> playersInTournament = new HashSet<string>();
    private HubConnection hubConnection;
    private string localPlayerName;

    private async void Awake()
    {
        if (localPlayerGameObject == null)
        {
            Debug.LogError("Local player GameObject not assigned!");
            return;
        }
        localPlayerName = localPlayerGameObject.name;

        hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .Build();

        hubConnection.On<List<string>>("ReceiveTournamentPlayers", playerList =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                playersInTournament.Clear();
                foreach (var name in playerList) playersInTournament.Add(name);
                UpdateUI();
            });
        });


        hubConnection.On("StartBattle", () =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                battleUI.SetActive(false);
                SceneManager.LoadScene("BattleScene");
            });
        });

        try
        {
            await hubConnection.StartAsync();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SignalR connection error: {e.Message}");
        }

        UpdateUI();
    }

    private async void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") )
        {
            battleUI.SetActive(true);
            if (hubConnection.State == HubConnectionState.Connected)
            {
                try
                {
                    await hubConnection.InvokeAsync("JoinTournament", localPlayerName);
                }
                catch { }
            }
        }
    }

    private async void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            battleUI.SetActive(false);
            if (hubConnection.State == HubConnectionState.Connected)
            {
                try
                {
                    await hubConnection.InvokeAsync("LeaveTournament", localPlayerName);
                }
                catch { }
            }
        }
    }

    private void UpdateUI()
    {
        playerCounts.text = $"Players: {playersInTournament.Count}/2";

        playerNames.text = string.Join("\n", playersInTournament);

        if (playersInTournament.Count == 2 && playersInTournament.Contains(localPlayerName))
        {
            infoText.text = "Ready to battle!";
            joinButton.SetActive(true);
        }
        else
        {
            infoText.text = "Waiting for players...";
            joinButton.SetActive(false);
        }
    }

    public async void OkClicked()
    {
        if (hubConnection.State == HubConnectionState.Connected)
        {
            try
            {
                await hubConnection.InvokeAsync("StartBattle");
            }
            catch { }
        }
    }

    public void CloseClicked()
    {
        battleUI.SetActive(false);
    }

    private void OnDestroy()
    {
        _ = DisconnectHub();
    }

    private async Task DisconnectHub()
    {
        if (hubConnection != null)
        {
            await hubConnection.StopAsync();
        }
    }
}
