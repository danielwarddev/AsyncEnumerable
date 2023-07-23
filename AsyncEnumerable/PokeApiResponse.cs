namespace AsyncEnumerable;
public record PokeApiResponse<T>(int Count, string? Next, string? Previous, T[] Results);

public record PokeApiPokemon(string Name, string Url);