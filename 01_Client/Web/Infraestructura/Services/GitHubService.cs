using Infraestructura.Models.GitHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Services
{
    public class GitHubService
    {
        public event Action<FetchProgressEventArgs> OnProgress;

        private readonly HttpClient _httpClient;

        public GitHubService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Issue>> GetIssues(DateTime date)
        {
            var issues = new List<Issue>();
            // Simula el progreso
            OnProgress?.Invoke(new FetchProgressEventArgs { Current = 1, Total = 10 });
            await Task.Delay(1000); // Simulación de tiempo
            OnProgress?.Invoke(new FetchProgressEventArgs { Current = 10, Total = 10 });
            return issues;
        }
    }

    public class FetchProgressEventArgs : EventArgs
    {
        public int Current { get; set; }
        public int Total { get; set; }
    }

}
