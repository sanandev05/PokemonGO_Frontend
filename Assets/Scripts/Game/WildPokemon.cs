using UnityEngine;

// Bu skripti səhnədəki BÜTÜN vəhşi Pokémon-lara əlavə et.
public class WildPokemon : MonoBehaviour
{
    public string pokemonName;
    public int maxHP = 50;
    public int currentHP = 50;

    private void OnMouseDown()
    {
        if (GameManager.Instance != null)
        {
            // Yalnız oyun normal vəziyyətdədirsə (tutma prosesində deyilsə) işləsin
            if (!GameManager.Instance.inCatchSequence)
            {
                GameManager.Instance.StartCatchSequence(this); // Bu Pokemonu hədəf olaraq təyin et və tutma prosesini başlat
            }
        }
        else
        {
            Debug.LogError("GameManager instance not found! Make sure GameManager GameObject is in the scene and has the GameManager script.");
        }
    }
}