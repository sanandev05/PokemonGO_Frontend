using UnityEngine;
using System.Threading.Tasks;

public class DataManager : MonoBehaviour
{
    private GenericApiService<TrainerDTO> _trainerService;
    private GenericApiService<PokemonDTO> _pokemonService;

    void Start()
    {
        // Servisləri öz endpoint URL-ləri ilə yaradırıq
        _trainerService = new GenericApiService<TrainerDTO>(ConstDatas.TrainerApiUrl);
        _pokemonService = new GenericApiService<PokemonDTO>(ConstDatas.PokemonApiUrl);

        // Nümunə əməliyyatları çağırırıq
        TestApiOperations();
    }

    private async void TestApiOperations()
    {
        try
        {
            // Nümunə 1: Bütün Trainer-ləri gətir
            Debug.Log("Fetching all trainers...");
            var allTrainers = await _trainerService.GetAllAsync();
            Debug.Log($"Found {allTrainers.Count} trainers.");

            // Nümunə 2: ID-si 1 olan Trainer-i gətir
            Debug.Log("Fetching trainer with ID 1...");
            var singleTrainer = await _trainerService.GetByIdAsync("1"); // ID string olmalıdır
            Debug.Log($"Found trainer: {singleTrainer.Name}");

            // Nümunə 3: Yeni bir Trainer yarat
            Debug.Log("Creating a new trainer...");
            var newTrainerData = new TrainerDTO { Name = "Ash Ketchum" };
            var createdTrainer = await _trainerService.CreateAsync(newTrainerData);
            Debug.Log($"Created trainer '{createdTrainer.Name}' with ID {createdTrainer.Id}");

            // Nümunə 4: Trainer-i yenilə
            createdTrainer.Name = "Ash Ketchum (Updated)";
            var updatedTrainer = await _trainerService.UpdateAsync(createdTrainer,createdTrainer.Id);
            Debug.Log($"Updated trainer name to: {updatedTrainer.Name}");

            // Nümunə 5: Trainer-i sil
            Debug.Log($"Deleting trainer with ID {createdTrainer.Id}...");
            await _trainerService.DeleteAsync(createdTrainer.Id.ToString());
            Debug.Log("Trainer deleted successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("An API error occurred: " + ex.Message);
        }


    }
}