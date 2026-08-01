using KaaDebug.Core.Interfaces.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Simulation
{
    public class SimulatedAuthService : IAuthService
    {
        private readonly InMemoryDataStore _store;

        public SimulatedAuthService(InMemoryDataStore store) => _store = store;

        public Task<bool> IsSessionValidAsync()
            => Task.FromResult(_store.IsAuthenticated && !string.IsNullOrEmpty(_store.SessionToken));

        public Task SaveSessionAsync(string token)
        {
            _store.SessionToken = token;
            _store.IsAuthenticated = true;
            return Task.CompletedTask;
        }

        public Task ClearSessionAsync()
        {
            _store.SessionToken = null;
            _store.IsAuthenticated = false;
            return Task.CompletedTask;
        }
    }

    public class SimulatedLoginService : ILoginService
    {
        private readonly InMemoryDataStore _store;
        private readonly IAuthService _authService;

        public SimulatedLoginService(InMemoryDataStore store, IAuthService authService)
        {
            _store = store;
            _authService = authService;
        }

        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            await Task.Delay(800);

            var user = _store.CurrentUser;

            if (user is null
                || !user.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                || user.Password != password)
            {
                return LoginResult.Fail("E-mail ou senha incorretos.");
            }

            var token = AppScenarioSimulator_TokenHelper.GenerateToken();
            await _authService.SaveSessionAsync(token);
            return LoginResult.Ok(token);
        }
    }

    /// <summary>Helper interno para geração do token fake compartilhado.</summary>
    internal static class AppScenarioSimulator_TokenHelper
    {
        internal static string GenerateToken() =>
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9." +
            "eyJzdWIiOiJ1MSIsIm5hbWUiOiJBbmEgUGF1bGEiLCJleHAiOjQxMDI0NDQ4MDB9." +
            "simulation_signature_not_real";
    }
}
