namespace AsyncEnumerable;

public interface IPokemonService
{
    
}

public class PokemonService : IPokemonService
{
    private readonly IPokemonClient _pokemonClient;

    public PokemonService(IPokemonClient _pokemonClient)
    {
        this._pokemonClient = _pokemonClient;
    }

    public async Task<List<PokeApiInfoUrl>> GetPokemonInfoUrls(int amount)
    {
        var pokemonUrls = new List<PokeApiInfoUrl>();
        
        await foreach (var page in _pokemonClient.GetAllPokemonUrls())
        {
            pokemonUrls.AddRange(page);
            if (pokemonUrls.Count >= amount)
            {
                break;
            }
        }

        return pokemonUrls;
    }
}