using AutoFixture;
using FluentAssertions;
using Moq;

namespace AsyncEnumerable.UnitTests;

public class PokemonServiceTests
{
    private readonly Fixture _fixture = new();
    private readonly PokemonService _pokemonService;
    private readonly Mock<IPokemonClient> _pokemonClient = new();

    public PokemonServiceTests()
    {
        _pokemonService = new PokemonService(_pokemonClient.Object);
    }

    [Theory]
    [InlineData(1, 3)]
    [InlineData(8, 9)]
    [InlineData(12, 12)]
    public async Task Makes_Calls_To_Client_Until_It_Has_Requested_Amount(int requestedAmount, int expectedAmount)
    {
        var allUrlPages = _fixture.CreateMany<PokeApiInfoUrl[]>();
        _pokemonClient.Setup(x => x.GetAllPokemonUrls()).Returns(allUrlPages.ToAsyncEnumerable());
        var expectedUrls = allUrlPages.SelectMany(x => x).Take(expectedAmount);
        
        var actualUrls = await _pokemonService.GetPokemonInfoUrls(requestedAmount);

        actualUrls.Should().BeEquivalentTo(expectedUrls);
    }

    // Example of how to create your own async enumerable if you don't want to use ToAsyncEnumerable
    private async IAsyncEnumerable<PokeApiInfoUrl[]> CreateAsyncEnumYield()
    {
        yield return _fixture.Create<PokeApiInfoUrl[]>();
        yield return _fixture.Create<PokeApiInfoUrl[]>();
        yield return _fixture.Create<PokeApiInfoUrl[]>();
        await Task.CompletedTask;
    }
}