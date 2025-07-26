using UnityEngine;

public class EncounterTrigger : MonoBehaviour
{
    // Drag your wild Pokémon object here in the Inspector
    public CathingPokemon wildPokemon;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered is the Player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the catch zone. Pokémon is now catchable.");
            wildPokemon.isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object that exited is the Player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left the catch zone.");
            wildPokemon.isPlayerInRange = false;
        }
    }
}