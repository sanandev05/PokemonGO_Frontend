using Microsoft.AspNetCore.SignalR.Client;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerClient : MonoBehaviour
{
    [SerializeField] private string hubUrl = "https://localhost:7156/gameHub";
    private HubConnection hubConnection;

    [SerializeField] private GameObject otherPlayerPrefab;
    public GenericApiService<TrainerDTO> _trainerService;

    private float syncInterval = 0.1f; // 100ms interval
    private Coroutine sendStateCoroutine;

    // For simple client-side prediction: store last sent position & rotation
    private Vector3 lastSentPosition;
    private Vector3 lastSentRotation;

    // Example for local input to simulate movement (you can replace this with your actual input system)
    [SerializeField] private float moveSpeed = 5f;

    private async void Start()
    {
        _trainerService = new GenericApiService<TrainerDTO>(ConstDatas.TrainerApiUrl);

        hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .Build();

        hubConnection.On<string, float, float, float, float, float, float, string>(
            "ReceivePlayerState",
            (userName, posX, posY, posZ, rotX, rotY, rotZ, animationState) =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    GameObject player = GameObject.Find(userName);
                    if (player == null)
                    {
                        Vector3 spawnPos = new Vector3(posX, posY, posZ);
                        Quaternion spawnRot = Quaternion.Euler(rotX, rotY, rotZ);
                        player = Instantiate(otherPlayerPrefab, spawnPos, spawnRot);
                        player.name = userName;
                        player.AddComponent<RemotePlayer>();
                    }

                    var remotePlayer = player.GetComponent<RemotePlayer>();
                    if (remotePlayer != null)
                    {
                        remotePlayer.UpdateTarget(
                            new Vector3(posX, posY, posZ),
                            Quaternion.Euler(rotX, rotY, rotZ),
                            animationState);
                    }
                });
            });

        try
        {
            await hubConnection.StartAsync();
            Debug.Log("SignalR Connection Started Successfully!");

            sendStateCoroutine = StartCoroutine(SendPlayerStateRoutine());
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error starting SignalR connection: {ex.Message}");
        }
    }

    private void Update()
    {
        // Simple client-side prediction for local player movement example:
        // Process basic WASD keys and move immediately

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 moveDelta = new Vector3(moveX, 0, moveZ).normalized * moveSpeed * Time.deltaTime;
        if (moveDelta.sqrMagnitude > 0f)
        {
            transform.position += moveDelta;
            // Optionally update rotation facing movement direction
            transform.rotation = Quaternion.LookRotation(moveDelta);
        }
        // Note: You might need to adapt this for your actual player controls.

        // Client-side prediction here is: player moves immediately locally, 
        // and the server will eventually confirm that position.
    }

    private IEnumerator SendPlayerStateRoutine()
    {
        var wait = new WaitForSeconds(syncInterval);

        while (hubConnection != null && hubConnection.State == HubConnectionState.Connected)
        {
            var trainer = _trainerService.GetLocalCurrentTrainerDto();
            if (trainer != null)
            {
                Vector3 position = transform.position;
                Vector3 rotation = transform.rotation.eulerAngles;
                string animState = GetCurrentAnimationState();

                // Optional: send only if moved/rotated enough to reduce unnecessary traffic
                if ((position - lastSentPosition).sqrMagnitude > 0.0001f ||
                    (rotation - lastSentRotation).sqrMagnitude > 0.1f)
                {
                    SendPlayerState(trainer.Name, position, rotation, animState);
                    lastSentPosition = position;
                    lastSentRotation = rotation;
                }
            }

            yield return wait;
        }
    }

    private async void OnDestroy()
    {
        if (sendStateCoroutine != null)
        {
            StopCoroutine(sendStateCoroutine);
        }

        if (hubConnection != null)
        {
            try
            {
                await hubConnection.StopAsync();
                Debug.Log("SignalR Connection Stopped.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error stopping SignalR connection: {ex.Message}");
            }
        }
    }

    public async void SendPlayerState(string userName, Vector3 position, Vector3 rotation, string animationState)
    {
        userName = _trainerService.GetLocalCurrentTrainerDto().Name;

        try
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
        catch (System.Exception ex)
        {
            Debug.LogError($"Error sending player state: {ex.Message}");
        }
    }

    private string GetCurrentAnimationState()
    {
        Animator animator = GetComponent<Animator>();
        if (animator == null) return "";

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Walk")) return "Walk";
        if (stateInfo.IsName("Idle")) return "Idle";
        if (stateInfo.IsName("Attack")) return "Attack";
        return "Idle"; // default fallback
    }
}
