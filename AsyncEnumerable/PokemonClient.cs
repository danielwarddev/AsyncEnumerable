using System.Net.Http.Json;

namespace AsyncEnumerable;

public interface IPokemonClient
{
    IAsyncEnumerable<PokeApiPokemon[]> GetAllPokemon();
}

public class PokemonClient : IPokemonClient
{
    private const int PageSize = 100;
    private readonly HttpClient _httpClient;

    public PokemonClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async IAsyncEnumerable<PokeApiPokemon[]> GetAllPokemon()
    {
        var itemsInResponse = 0;
        var currentPage = 0;

        do
        {
            var response = (await _httpClient.GetFromJsonAsync<PokeApiResponse<PokeApiPokemon>>($"pokemon?limit={PageSize}&offset={currentPage * PageSize}"))!;
            itemsInResponse = response.Count;

            yield return response.Results;
        }
        while (itemsInResponse == PageSize);
    }
}
