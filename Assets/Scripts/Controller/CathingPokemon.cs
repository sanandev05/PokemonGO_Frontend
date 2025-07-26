using UnityEngine;

public class CathingPokemon : MonoBehaviour
{
    private int attemptCount = 0; // Cəhdlərin sayı

    // Bu funksiya Poketop tərəfindən çağırılacaq
    public void AttemptToCatch(WildPokemon pokemon)
    {
        attemptCount++; // Hər cəhddə sayğacı artır

        // Tutma ehtimalını hesablamaq üçün formula
        // Ehtimal = (1 - (MövcudCan / MaksimumCan)) * 0.5f + (CəhdSayı * 0.05f)
        // Bu formula ilə:
        // - Canı azaldıqca tutma şansı artır.
        // - Hər yeni cəhddə şans bir az daha artır.
        float hpFactor = 1.0f - ((float)pokemon.currentHp / (float)pokemon.maxHp);
        float attemptFactor = attemptCount * 0.05f; // Hər cəhd üçün 5% bonus

        // Baza tutma şansı (məsələn 30%) + HP faktoru + Cəhd faktoru
        float catchChance = 0.3f + (hpFactor * 0.5f) + attemptFactor;

        // Ehtimalın 0 ilə 1 arasında olduğundan əmin olmaq
        catchChance = Mathf.Clamp(catchChance, 0.0f, 1.0f);

        // Təsadüfi bir dəyər yaradırıq
        float randomValue = Random.Range(0.0f, 1.0f);

        Debug.Log($"Cəhd #{attemptCount}. Tutma Şansı: {catchChance * 100}%. Təsadüfi Dəyər: {randomValue * 100}%");

        if (randomValue <= catchChance)
        {
            OnCaptureSuccess(pokemon);
        }
        else
        {
            OnCaptureFailure(pokemon);
        }
    }

    private void OnCaptureSuccess(WildPokemon pokemon)
    {
        Debug.Log($"{pokemon.pokemonName} uğurla tutuldu!");
        // Burada Pokémon-u oyunçunun siyahısına əlavə etmək,
        // Pokémon modelini yox etmək və s. kodlar olmalıdır.
        Destroy(pokemon.gameObject);
        
        // Tutduqdan sonra cəhd sayını sıfırla
        attemptCount = 0; 
        
        // Məsələn, döyüş səhnəsini bağla və ya UI göstər
    }

    private void OnCaptureFailure(WildPokemon pokemon)
    {
        Debug.Log($"{pokemon.pokemonName} Poketopdan çıxdı!");
        // Burada Pokémonun topdan çıxma animasiyası və s. ola bilər.
    }

    // Yeni bir Pokémon ilə qarşılaşdıqda cəhd sayını sıfırlamaq üçün
    public void ResetAttempts()
    {
        attemptCount = 0;
    }
}