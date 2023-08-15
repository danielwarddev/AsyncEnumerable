using Moq;
using System.Linq;
using System.Net;
using AutoFixture;
using FluentAssertions;

namespace AsyncEnumerable.UnitTests;

public class PokemonClientTests
{
    private readonly Fixture _fixture = new();
    private readonly PokemonClient _pokemonClient;
    private readonly Mock<HttpMessageHandler> _httpMessageHandler = new();
    private readonly string _baseAddress = "https://www.google.com";

    public PokemonClientTests()
    {
        var httpClient = new HttpClient(_httpMessageHandler.Object)
        {
            BaseAddress = new Uri(_baseAddress)
        };
        _pokemonClient = new PokemonClient(httpClient);
    }

    [Fact]
    public async Task Returns_Pages_Until_Results_Is_Greater_Than_Or_Equal_To_The_Total_Count()
    {
        var endpoint = $"{_baseAddress}/pokemon";
        var totalPages = 3;
        var resultsPerPage = 3;
        
        var responses = SetupSuccessfulPagedResponses<PokeApiInfoUrl>(endpoint, totalPages, resultsPerPage);
        var expectedResults = responses.Select(x => x.Results).ToArray();

        int i = 0;
        await foreach (var actualResults in _pokemonClient.GetAllPokemonUrls())
        {
            expectedResults[i].Should().BeEquivalentTo(actualResults);
            i++;
        }
    }

    private List<PokeApiResponse<T>> SetupSuccessfulPagedResponses<T>(string endpoint, int totalPages, int resultsPerPage)
    {
        var responses = new List<PokeApiResponse<T>>();

        for (int i = 0; i < totalPages; i++)
        {
            var response = SetupSuccessfulResponse<T>(endpoint, resultsPerPage, totalPages * resultsPerPage, PokemonClient.PageSize,
                i * PokemonClient.PageSize);
            responses.Add(response);
        }

        return responses;
    }

    private PokeApiResponse<T> SetupSuccessfulResponse<T>(string endpoint, int resultsPerPage, int countInResponse, int limit, int offset)
    {
        var results = _fixture.CreateMany<T>(resultsPerPage).ToArray();
        var response = new PokeApiResponse<T>(countInResponse, null, null, results);
        
        _httpMessageHandler
            .SetupSendAsync(HttpMethod.Get, $"{endpoint}?limit={limit}&offset={offset}")
            .ReturnsHttpResponseAsync(response, HttpStatusCode.OK);

        return response;
    }
}