using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KaaDebug.Infrastructure.http
{
    /// <summary>
    /// Cliente HTTP centralizado do app mobile.
    /// Responsabilidades:
    ///   - Injetar o token JWT em todas as requisições autenticadas
    ///   - Serializar/desserializar JSON de forma consistente
    ///   - Tratar erros de rede (sem conexão, timeout, servidor fora)
    ///   - Fornecer métodos tipados para GET, POST, PUT, DELETE
    ///
    /// Todos os serviços reais injetam este cliente em vez de criar
    /// HttpClient diretamente — evita instâncias desnecessárias e
    /// centraliza a configuração.
    /// </summary>
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthTokenProvider _tokenProvider;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };

        public ApiClient(HttpClient httpClient, IAuthTokenProvider tokenProvider)
        {
            _httpClient = httpClient;
            _tokenProvider = tokenProvider;
        }

        // ── GET ───────────────────────────────────────────────────────────────────

        public async Task<ApiResult<T>> GetAsync<T>(string endpoint)
        {
            try
            {
                await AttachTokenAsync();
                var response = await _httpClient.GetAsync(endpoint);
                return await ParseResponseAsync<T>(response);
            }
            catch (HttpRequestException)
            {
                return ApiResult<T>.NetworkError();
            }
            catch (TaskCanceledException)
            {
                return ApiResult<T>.Timeout();
            }
        }

        // ── POST ──────────────────────────────────────────────────────────────────

        public async Task<ApiResult<T>> PostAsync<T>(string endpoint, object body)
        {
            try
            {
                await AttachTokenAsync();
                var response = await _httpClient.PostAsJsonAsync(endpoint, body, JsonOptions);
                return await ParseResponseAsync<T>(response);
            }
            catch (HttpRequestException)
            {
                return ApiResult<T>.NetworkError();
            }
            catch (TaskCanceledException)
            {
                return ApiResult<T>.Timeout();
            }
        }

        public async Task<ApiResult> PostAsync(string endpoint, object body)
        {
            try
            {
                await AttachTokenAsync();
                var response = await _httpClient.PostAsJsonAsync(endpoint, body, JsonOptions);
                return await ParseResponseAsync(response);
            }
            catch (HttpRequestException)
            {
                return ApiResult.NetworkError();
            }
            catch (TaskCanceledException)
            {
                return ApiResult.Timeout();
            }
        }

        // ── POST multipart (upload de imagem para diagnóstico) ────────────────────

        public async Task<ApiResult<T>> PostMultipartAsync<T>(string endpoint, MultipartFormDataContent content)
        {
            try
            {
                await AttachTokenAsync();
                var response = await _httpClient.PostAsync(endpoint, content);
                return await ParseResponseAsync<T>(response);
            }
            catch (HttpRequestException)
            {
                return ApiResult<T>.NetworkError();
            }
            catch (TaskCanceledException)
            {
                return ApiResult<T>.Timeout();
            }
        }

        // ── PUT ───────────────────────────────────────────────────────────────────

        public async Task<ApiResult> PutAsync(string endpoint, object body)
        {
            try
            {
                await AttachTokenAsync();
                var response = await _httpClient.PutAsJsonAsync(endpoint, body, JsonOptions);
                return await ParseResponseAsync(response);
            }
            catch (HttpRequestException)
            {
                return ApiResult.NetworkError();
            }
            catch (TaskCanceledException)
            {
                return ApiResult.Timeout();
            }
        }

        public async Task<ApiResult> PutAsync(string endpoint)
        {
            try
            {
                await AttachTokenAsync();
                var response = await _httpClient.PutAsync(endpoint, null);
                return await ParseResponseAsync(response);
            }
            catch (HttpRequestException)
            {
                return ApiResult.NetworkError();
            }
            catch (TaskCanceledException)
            {
                return ApiResult.Timeout();
            }
        }

        // ── DELETE ────────────────────────────────────────────────────────────────

        public async Task<ApiResult> DeleteAsync(string endpoint)
        {
            try
            {
                await AttachTokenAsync();
                var response = await _httpClient.DeleteAsync(endpoint);
                return await ParseResponseAsync(response);
            }
            catch (HttpRequestException)
            {
                return ApiResult.NetworkError();
            }
            catch (TaskCanceledException)
            {
                return ApiResult.Timeout();
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private async Task AttachTokenAsync()
        {
            var token = await _tokenProvider.GetTokenAsync();

            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            else
                _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        private static async Task<ApiResult<T>> ParseResponseAsync<T>(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                if (string.IsNullOrWhiteSpace(body))
                    return ApiResult<T>.Ok(default!);

                var data = JsonSerializer.Deserialize<T>(body, JsonOptions);
                return ApiResult<T>.Ok(data!);
            }

            var errorMessage = TryExtractError(body, (int)response.StatusCode);
            return ApiResult<T>.Fail(errorMessage, (int)response.StatusCode);
        }

        private static async Task<ApiResult> ParseResponseAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return ApiResult.Ok();

            var body = await response.Content.ReadAsStringAsync();
            var errorMessage = TryExtractError(body, (int)response.StatusCode);
            return ApiResult.Fail(errorMessage, (int)response.StatusCode);
        }

        /// <summary>
        /// Tenta extrair a mensagem de erro do JSON retornado pela API.
        /// A API retorna: { "error": "mensagem" }
        /// </summary>
        private static string TryExtractError(string body, int statusCode)
        {
            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("error", out var errorProp))
                    return errorProp.GetString() ?? DefaultErrorMessage(statusCode);
            }
            catch { /* body não é JSON válido */ }

            return DefaultErrorMessage(statusCode);
        }

        private static string DefaultErrorMessage(int statusCode) => statusCode switch
        {
            400 => "Dados inválidos. Verifique as informações e tente novamente.",
            401 => "Sessão expirada. Faça login novamente.",
            403 => "Você não tem permissão para realizar esta ação.",
            404 => "Recurso não encontrado.",
            409 => "Conflito de dados. Este registro já existe.",
            500 => "Erro interno do servidor. Tente novamente mais tarde.",
            _ => "Ocorreu um erro inesperado. Tente novamente."
        };
    }

    // ── Result types do ApiClient ────────────────────────────────────────────────

    public class ApiResult
    {
        public bool Success { get; protected init; }
        public string? ErrorMessage { get; protected init; }
        public int StatusCode { get; protected init; }

        public bool IsUnauthorized => StatusCode == 401;
        public bool IsNotFound => StatusCode == 404;
        public bool IsNetworkError => StatusCode == 0;

        public static ApiResult Ok() => new() { Success = true, StatusCode = 200 };
        public static ApiResult Fail(string message, int statusCode = 400) =>
            new() { Success = false, ErrorMessage = message, StatusCode = statusCode };
        public static ApiResult NetworkError() =>
            new() { Success = false, ErrorMessage = "Sem conexão com a internet.", StatusCode = 0 };
        public static ApiResult Timeout() =>
            new() { Success = false, ErrorMessage = "A requisição demorou muito. Verifique sua conexão.", StatusCode = 0 };
    }

    public class ApiResult<T> : ApiResult
    {
        public T? Data { get; private init; }

        public static ApiResult<T> Ok(T data) =>
            new() { Success = true, Data = data, StatusCode = 200 };
        public new static ApiResult<T> Fail(string message, int statusCode = 400) =>
            new() { Success = false, ErrorMessage = message, StatusCode = statusCode };
        public new static ApiResult<T> NetworkError() =>
            new() { Success = false, ErrorMessage = "Sem conexão com a internet.", StatusCode = 0 };
        public new static ApiResult<T> Timeout() =>
            new() { Success = false, ErrorMessage = "A requisição demorou muito. Verifique sua conexão.", StatusCode = 0 };
    }
}
