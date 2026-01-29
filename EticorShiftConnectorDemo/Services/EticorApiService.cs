using EticorShiftConnectorDemo.Models;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EticorShiftConnectorDemo.Services
{
    internal static class Endpoints
    {
        public const string EmployeeByPersonnelNumber = "public/employees";
        public const string Delegations = "public/delegations";
        public const string OrgUnits = "public/orgUnits";
        public const string Tasks = "public/tasks";
        public const string Laws = "public/laws";
        public const string Documents = "public/documents";
        public const string Inspections = "public/inspections";
    }

    internal class EticorApiService(IConfigurationRoot config, JsonSerializerOptions jsonSerializerOptions, ILogger<EticorApiService> log)
    {
        public async Task<EmployeeModel> GetEmployeeByPersonnelNumberAsync(string personnelNumber)
        {
            _log.LogInformation($"Get employee by personnel number {personnelNumber}");
            string requestPath = $"{Endpoints.EmployeeByPersonnelNumber}/{personnelNumber}";
            EmployeeModel employee = await GetAsync<EmployeeModel>(requestPath);

            return employee!;
        }

        public async Task<PageResult<DelegationModel>> GetDelegationsAsync(DelegationListRequestModel request)
        {
            _log.LogInformation($"Get delegations");
            string requestPath = $"{Endpoints.Delegations}?{request.ToQueryParameters()}";
            PageResult<DelegationModel> delegations = await GetAsync<PageResult<DelegationModel>>(requestPath);

            return delegations!;
        }

        public async Task<PageResult<OrgUnitModel>> GetOrgUnitsAsync(OrgUnitRequestModel request)
        {
            _log.LogInformation($"Get orgUnits");
            string requestPath = $"{Endpoints.OrgUnits}?{request.ToQueryParameters()}";
            PageResult<OrgUnitModel> orgUnits = await GetAsync<PageResult<OrgUnitModel>>(requestPath);
            return orgUnits!;
        }

        public async Task<List<DocumentModel>> GetDocumentsForDelegationAsync(int delegationId)
        {
            _log.LogInformation($"Get documents for delegation {delegationId}");
            string requestPath = $"{Endpoints.Delegations}/{delegationId}/documents";
            List<DocumentModel> documents = await GetAsync<List<DocumentModel>>(requestPath);
            return documents!;
        }

        public async Task<PageResult<DocumentModel>> GetDocumentsForTaskAsync(int taskId)
        {
            _log.LogInformation($"Get documents for task {taskId}");
            string requestPath = $"{Endpoints.Tasks}/{taskId}/documents";
            PageResult<DocumentModel> documents = await GetAsync<PageResult<DocumentModel>>(requestPath);
            return documents!;
        }

        public async Task<PageResult<DocumentModel>> GetDocumentsForLawAsync(int lawId)
        {
            _log.LogInformation($"Get documents for law {lawId}");
            string requestPath = $"{Endpoints.Laws}/{lawId}/documents";
            PageResult<DocumentModel> documents = await GetAsync<PageResult<DocumentModel>>(requestPath);
            return documents!;
        }

        public async Task<DocumentModel> GetDocumentByIdAsync(int documentId)
        {
            _log.LogInformation($"Get document by id {documentId}");
            string requestPath = $"{Endpoints.Documents}/{documentId}";
            DocumentModel document = await GetAsync<DocumentModel>(requestPath);
            return document!;
        }

        public async Task SaveDocumentAsync(DocumentModel doc)
        {
            string path = Path.Join(Environment.CurrentDirectory, doc.FileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            await File.WriteAllBytesAsync(path, doc.Bytes);
            _log.LogInformation($"Document saved to {path}");
        }

        public async Task<InspectionModel> CreateInspectionAsync(CreateInspectionModel model)
        {
            using HttpClient httpClient = await CreateHttpClientAsync();
            string requestPath = $"{Endpoints.Inspections}";

            string json = JsonSerializer.Serialize(model, _jsonSerializerOptions);
            StringContent content = new(json, Encoding.UTF8, "application/json");

            _log.LogInformation($"POST {requestPath}");
            HttpResponseMessage response = await httpClient.PostAsync(requestPath, content);

            if (!response.IsSuccessStatusCode)
            {
                string message = $"Error creating inspection: {response.ReasonPhrase}";
                _log.LogError(message);
                throw new HttpRequestException(message);
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            InspectionModel inspection = Deserialize<InspectionModel>(responseContent);

            return inspection!;
        }

        public async Task<OrgUnitModel> GetOrgUnitByIdAsync(int id)
        {
            string requestPath = $"{Endpoints.OrgUnits}/{id}";
            _log.LogInformation($"GET {requestPath}");
            OrgUnitModel orgUnit = await GetAsync<OrgUnitModel>(requestPath);
            return orgUnit!;
        }

        #region private methods
        private string _accessToken = string.Empty;
        private readonly IConfigurationRoot _config = config;
        private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions;
        private readonly ILogger<EticorApiService> _log = log;

        private async Task<HttpClient> CreateHttpClientAsync()
        {
            HttpClient client = new();
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
            if (string.IsNullOrEmpty(_accessToken))
            {
                await RefreshToken();
            }

            // If the token is expiring, get a new one
            JwtSecurityTokenHandler handler = new();
            JwtSecurityToken token = handler.ReadJwtToken(_accessToken);
            DateTime validTo = token.ValidTo;
            if (validTo < DateTime.UtcNow.AddMinutes(1))
            {
                await RefreshToken();
            }
        }

        private async Task RefreshToken()
        {
            using HttpClient tokenClient = new();
            DiscoveryDocumentResponse disco = await tokenClient.GetDiscoveryDocumentAsync(_config["Authority"]);
            if (disco.IsError)
            {
                string message = $"Error retrieving discovery document: {disco.Error}";
                _log.LogError(message);
                throw new HttpRequestException(message);
            }

            ClientCredentialsTokenRequest clientCredentials = new()
            {
                Address = disco.TokenEndpoint,
                ClientId = _config["ClientId"]!,
                ClientSecret = _config["ClientSecret"]
            };

            TokenResponse tokenResponse = await tokenClient.RequestClientCredentialsTokenAsync(clientCredentials);
            if (tokenResponse.IsError)
            {
                string message = $"Error retrieving token: {tokenResponse.Error}";
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
            using HttpClient client = await CreateHttpClientAsync();
            HttpResponseMessage response = await client.GetAsync(requestPath);

            if (!response.IsSuccessStatusCode)
            {
                string message = $"Error retrieving {typeof(T).Name}: {response.ReasonPhrase}";
                _log.LogError(message);
                throw new HttpRequestException(message);
            }
            string content = await response.Content.ReadAsStringAsync();
            T? obj = Deserialize<T>(content);

            return obj!;
        }
    }
    #endregion
}
