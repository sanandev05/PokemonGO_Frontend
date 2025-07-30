using UnityEngine;

// Bu skripti səhnədəki BÜTÜN vəhşi Pokémon-lara əlavə et.
public class WildPokemon : MonoBehaviour
{
    public int PokemonId;
    public int Level;
    public string pokemonName;
    public int maxHP = 50;
    public int currentHP = 50;

    private void OnMouseDown()
    {
        if (GameManager.Instance != null && !GameManager.Instance.inCatchSequence)
        {
            // Tutulacaq pokemon haqqında məlumatı PlayerPrefs ilə saxla
            PlayerPrefs.SetString("TargetPokemon_Name", pokemonName);
            PlayerPrefs.SetInt("TargetPokemon_HP", currentHP);

            // Əgər prefabları adla yükləmək istəyirsənsə:
            PlayerPrefs.SetString("TargetPokemon_PrefabName", pokemonName); // Prefab adını da saxla

            PlayerPrefs.Save();

            GameManager.Instance.StartCatchSequence(this); // Səhnəni dəyiş
        }
    }

}