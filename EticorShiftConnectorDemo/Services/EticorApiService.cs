using EticorShiftConnectorDemo.Models;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EticorShiftConnectorDemo.Services;

internal static class Endpoints
{
    public const string EmployeeByPersonnellNumber = "employees/personellNumber";
    public const string Delegations = "delegations";
    public const string OrgUnits = "orgUnits";
    public const string Tasks = "tasks";
    public const string Laws = "laws";
    public const string Documents = "documents";
    public const string Inspections = "inspections";
}

internal class EticorApiService
{
    public EticorApiService(IConfigurationRoot config, JsonSerializerOptions jsonSerializerOptions, ILogger<EticorApiService> log)
    {
        _config = config;
        _jsonSerializerOptions = jsonSerializerOptions;
        _log = log;
    }
    public async Task<EmployeeModel> GetEmployeeByPersonnellNumberAsync(string personnellNumber)
    {
        _log.LogInformation($"Get employee by personnell number {personnellNumber}");
        var requestPath = $"{Endpoints.EmployeeByPersonnellNumber}/{personnellNumber}";
        var employee = await GetAsync<EmployeeModel>(requestPath);

        return employee!;
    }

    public async Task<PageResult<DelegationModel>> GetDelegationsAsync(DelegationsRequestModel request)
    {
        _log.LogInformation($"Get delegations");
        var requestPath = $"{Endpoints.Delegations}?{request.ToQueryParameters()}";
        var delegations = await GetAsync<PageResult<DelegationModel>>(requestPath);

        return delegations!;
    }

    public async Task<PageResult<OrgUnitModel>> GetOrgUnitsAsync(OrgUnitRequestModel request)
    {
        _log.LogInformation($"Get orgUnits");
        var requestPath = $"{Endpoints.OrgUnits}?{request.ToQueryParameters()}";
        var orgUnits = await GetAsync<PageResult<OrgUnitModel>>(requestPath);
        return orgUnits!;
    }

    public async Task<List<DocumentModel>> GetDocumentsForDelegationAsync(int delegationId)
    {
        _log.LogInformation($"Get documents for delegation {delegationId}");
        var requestPath = $"{Endpoints.Delegations}/{delegationId}/{Endpoints.Documents}";
        var documents = await GetAsync<List<DocumentModel>>(requestPath);
        return documents!;
    }

    public async Task<PageResult<DocumentModel>> GetDocumentsForTaskAsync(int taskId)
    {
        _log.LogInformation($"Get documents for task {taskId}");
        var requestPath = $"{Endpoints.Tasks}/{taskId}/{Endpoints.Documents}";
        var documents = await GetAsync<PageResult<DocumentModel>>(requestPath);
        return documents!;
    }

    public async Task<PageResult<DocumentModel>> GetDocumentsForLawAsync(int lawId)
    {
        _log.LogInformation($"Get documents for law {lawId}");
        var requestPath = $"{Endpoints.Laws}/{lawId}/{Endpoints.Documents}";
        var documents = await GetAsync<PageResult<DocumentModel>>(requestPath);
        return documents!;
    }

    public async Task<DocumentModel> GetDocumentByIdAsync(int documentId)
    {
        _log.LogInformation($"Get document by id {documentId}");
        var requestPath = $"{Endpoints.Documents}/{documentId}";
        var document = await GetAsync<DocumentModel>(requestPath);
        return document!;
    }

    public async Task SaveDocumentAsync(DocumentModel doc)
    {
        var path = Path.Join(Environment.CurrentDirectory, doc.FileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        await File.WriteAllBytesAsync(path, doc.Bytes);
        _log.LogInformation($"Document saved to {path}");
    }

    public async Task<InspectionModel> CreateInspectionAsync(CreateInspectionModel model)
    {
        using var httpClient = await CreateHttpClientAsync();
        var requestPath = $"{Endpoints.Inspections}";

        var json = JsonSerializer.Serialize(model, _jsonSerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _log.LogInformation($"POST {requestPath}");
        var response = await httpClient.PostAsync(requestPath, content);

        if (!response.IsSuccessStatusCode)
        {
            var message = $"Error creating inspection: {response.ReasonPhrase}";
            _log.LogError(message);
            throw new HttpRequestException(message);
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var inspection = Deserialize<InspectionModel>(responseContent);

        return inspection!;
    }

    #region private methods
    private string _accessToken = string.Empty;
    private readonly IConfigurationRoot _config;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly ILogger<EticorApiService> _log;

    private async Task<HttpClient> CreateHttpClientAsync()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("Accept-Language", "de");
        client.DefaultRequestHeaders.Add("ClientID", _config["CustomerId"]);
        client.BaseAddress = new Uri(_config["ApiRoot"]!);

        await CheckTokenValidity();
        client.SetBearerToken(_accessToken);

        return client;
    }

    private async Task CheckTokenValidity()
    {
        // If the token is not set or is expired, get a new one
        if (_accessToken is null)
        {
            await RefreshToken();
        }

        // If the token is expiring, get a new one
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(_accessToken);
        var validTo = token.ValidTo;
        if (validTo < DateTime.UtcNow.AddMinutes(1))
        {
            await RefreshToken();
        }
    }

    private async Task RefreshToken()
    {
        using var tokenClient = new HttpClient();
        var disco = await tokenClient.GetDiscoveryDocumentAsync(_config["Authority"]);
        if (disco.IsError)
        {
            var message = $"Error retrieving discovery document: {disco.Error}";
            _log.LogError(message);
            throw new HttpRequestException(message);
        }

        var clientCredentials = new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = _config["ClientId"]!,
            ClientSecret = _config["ClientSecret"]
        };

        var tokenResponse = await tokenClient.RequestClientCredentialsTokenAsync(clientCredentials);
        if (tokenResponse.IsError)
        {
            var message = $"Error retrieving token: {tokenResponse.Error}";
            _log.LogError(message);
            throw new HttpRequestException(message);
        }

        _accessToken = tokenResponse.AccessToken!;
    }

    private T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions)!;
    }

    private async Task<T> GetAsync<T>(string requestPath)
    {
        _log.LogInformation($"GET {_config["ApiRoot"]}{requestPath}");
        using var client = await CreateHttpClientAsync();
        var response = await client.GetAsync(requestPath);

        if (!response.IsSuccessStatusCode)
        {
            var message = $"Error retrieving {typeof(T).Name}: {response.ReasonPhrase}";
            _log.LogError(message);
            throw new HttpRequestException(message);
        }
        var content = await response.Content.ReadAsStringAsync();
        var obj = Deserialize<T>(content);

        return obj!;
    } 
}
    #endregion
