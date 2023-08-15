namespace AsyncEnumerable;
public record PokeApiResponse<T>(int Count, string? Next, string? Previous, T[] Results);

public record PokeApiInfoUrl(string Name, string Url);