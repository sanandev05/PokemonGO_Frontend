using UnityEngine;
using UnityEngine.UI;

public class CatchingController : MonoBehaviour
{
    public Text pokemonNameText;
    public Slider hpBar;

    private void Start()
    {
        string name = PlayerPrefs.GetString("PokemonName", "Unknown");
        int hp = PlayerPrefs.GetInt("PokemonHP", 50);

        if (pokemonNameText != null)
            pokemonNameText.text = "Wild " + name + " appeared!";

        if (hpBar != null)
        {
            hpBar.maxValue = hp;
            hpBar.value = hp;
        }
    }

    // Tutma tamamlananda burdan GameManager.Instance.targetPokemon üzərindən backendə sorğu göndərə bilərsən
}
