using UnityEngine;

// Bu skripti səhnədəki BÜTÜN vəhşi Pokémon-lara əlavə et.
public class WildPokemon : MonoBehaviour
{
    public string pokemonName;
    public int maxHP = 50;
    public int currentHP = 50;

    private void OnMouseDown()
    {
        // GameManager-ə MƏHZ BU Pokémon ilə tutma prosesini başlatmasını söylə
        GameManager.Instance.StartCatchSequence(this);
    }
}