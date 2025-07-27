using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class GenericApiService<T> where T : class
{
    public readonly string _baseUrl;
    public GenericApiService(string baseUrl)
    {
        _baseUrl = baseUrl;
    }
    public async Task<List<T>> GetAllAsync()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(_baseUrl))
        {
            var result = await request.SendWebRequestAsync();
            return JsonConvert.DeserializeObject<List<T>>(result.downloadHandler.text);
        }
    }

    public async Task<T> GetByIdAsync(string id)
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{_baseUrl}/{id}"))
        {
            var result = await request.SendWebRequestAsync();
            return JsonConvert.DeserializeObject<T>(result.downloadHandler.text);
        }
    }

    public async Task<T> CreateAsync(T data)
    {
        string jsonData= JsonConvert.SerializeObject(data);
        byte[] bodyRaw =Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(_baseUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var result = await request.SendWebRequestAsync();
            return JsonConvert.DeserializeObject<T>(result.downloadHandler.text);
        }
    } 

    public async Task<T> UpdateAsync(T data,int id)
    {
        string json = JsonConvert.SerializeObject(data);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = UnityWebRequest.Put($"{_baseUrl}/{id}", bodyRaw))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var result = await request.SendWebRequestAsync();
            return JsonConvert.DeserializeObject<T>(result.downloadHandler.text);
        }
    }

    public async Task DeleteAsync(string id)
    {
        using (UnityWebRequest request = UnityWebRequest.Delete($"{_baseUrl}/{id}"))
        {
            await request.SendWebRequestAsync();
        }
    }
}
