using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoGameStore.Context;
using VideoGameStore.Tests;

namespace VideoGameStore.Test.Integrations
{
    [Collection("Database collection")]
    public abstract class IntegrationTestBase : IAsyncLifetime
    {
        protected AppDbContext DbContext { get; private set; } = null!;
        protected IServiceProvider ServiceProvider { get; private set; } = null!;
        private MsSqlFixture _fixture = null!;
        private WebApplicationFactory<Program> _factory = null!;
        private IServiceScope _scope = null!;

        protected IntegrationTestBase(MsSqlFixture fixture)
        {
            _fixture = fixture; 
        }

        public virtual async Task InitializeAsync()
        {
            _fixture = new MsSqlFixture();
            await _fixture.InitializeAsync();

            _factory = new IntegrationTestWebAppFactory(_fixture.ConnectionString);
            ServiceProvider = _factory.Services;
            _scope = ServiceProvider.CreateScope();

            DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }

        public async Task DisposeAsync()
        {
            //DbContext?.Dispose();
            //_scope?.Dispose();
            _factory?.Dispose();
            await _fixture.DisposeAsync();
        }

        protected T GetService<T>() where T : notnull
        {
            return _scope.ServiceProvider.GetRequiredService<T>();
        }
    }

}
