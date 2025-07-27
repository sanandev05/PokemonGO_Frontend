using System.Threading.Tasks;
using UnityEngine.Networking;
using System;

public static class UnityWebRequestHelper
{
    public static Task<UnityWebRequest> SendWebRequestAsync(this UnityWebRequest request)
    {
        var tcs = new TaskCompletionSource<UnityWebRequest>();
        request.SendWebRequest().completed += operation =>
        {
            if (request.result == UnityWebRequest.Result.ConnectionError 
            || request.result == UnityWebRequest.Result.ProtocolError 
            || request.result == UnityWebRequest.Result.DataProcessingError)
            {
                tcs.SetException(new Exception(request.error));
            }
            else
            {
                tcs.SetResult(request);
            }
        };
        return tcs.Task;
    }
}