﻿using System;
using Marvin.Cache.Headers.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Marvin.Cache.Headers.Test.Extensions
{
    public class ServiceExtensionFacts
    {
        [Fact]
        public void Correctly_register_HttpCacheHeadersMiddleware_as_service()
        {
            var hostBuilder = new WebHostBuilder().Configure(app => app.UseHttpCacheHeaders()).ConfigureServices(service => service.AddHttpCacheHeaders());
            var testServer = new TestServer(hostBuilder);
            var middleware = testServer.Host.Services.GetService(typeof(IValidationValueStore));
            Assert.NotNull(middleware);
        }

        [Fact]
        public void Correctly_register_HttpCacheHeadersMiddleware_as_service_with_ExpirationModelOptions()
        {

            var hostBuilder =
                new WebHostBuilder().Configure(app => app.UseHttpCacheHeaders())
                    .ConfigureServices(
                        service => service.AddHttpCacheHeaders((ExpirationModelOptions options) => options.MaxAge = 1));
            var testServer = new TestServer(hostBuilder);

            ValidateServiceOptions<ExpirationModelOptions>(testServer, options => options.Value.MaxAge == 1);
        }

        [Fact]
        public void Correctly_register_HttpCacheHeadersMiddleware_as_service_with_ValidationModelOptions()
        {
            var hostBuilder =
                new WebHostBuilder().Configure(app => app.UseHttpCacheHeaders())
                    .ConfigureServices(
                        service => service.AddHttpCacheHeaders((ValidationModelOptions options) => options.AddNoCache = true));
            var testServer = new TestServer(hostBuilder);

            ValidateServiceOptions<ValidationModelOptions>(testServer, options => options.Value.AddNoCache);
        }

        [Fact]
        public void Correctly_register_HttpCacheHeadersMiddleware_as_service_with_ExpirationModelOptions_and_ValidationModelOptions()
        {
            var hostBuilder =
                new WebHostBuilder().Configure(app => app.UseHttpCacheHeaders())
                    .ConfigureServices(
                        service =>
                            service.AddHttpCacheHeaders(
                                (ExpirationModelOptions options) => options.MaxAge = 1,
                                (ValidationModelOptions options) => options.AddNoCache = true));
            var testServer = new TestServer(hostBuilder);

            ValidateServiceOptions<ExpirationModelOptions>(testServer, options => options.Value.MaxAge == 1);
            ValidateServiceOptions<ValidationModelOptions>(testServer, options => options.Value.AddNoCache);
        }

        private void ValidateServiceOptions<T>(TestServer testServer, Func<OptionsManager<T>, bool> validOptions) where T : class, new()
        {
            var options = testServer.Host.Services.GetService(typeof(IOptions<T>));
            Assert.NotNull(options);
            var manager = (OptionsManager<T>) options;
            Assert.True(validOptions(manager));
        }

        [Fact]
        public void When_no_ApplicationBuilder_expect_ArgumentNullException()
        {
            IServiceCollection serviceCollection = null;

            Assert.Throws<ArgumentNullException>(() => serviceCollection.AddHttpCacheHeaders());
        }

        [Fact]
        public void When_no_ApplicationBuilder_while_using_ExpirationModelOptions_expect_ArgumentNullException()
        {
            IServiceCollection serviceCollection = null;

            Assert.Throws<ArgumentNullException>(
                () => serviceCollection.AddHttpCacheHeaders((ExpirationModelOptions options) => options.MaxAge = 1));
        }

        [Fact]
        public void When_no_ApplicationBuilder_while_using_ValidationModelOptions_expect_ArgumentNullException()
        {
            IServiceCollection serviceCollection = null;

            Assert.Throws<ArgumentNullException>(
                () =>
                    serviceCollection.AddHttpCacheHeaders((ValidationModelOptions options) => options.AddNoCache = true));
        }

        [Fact]
        public void When_no_ApplicationBuilder_when_setting_both_ValidationModelOption_and_ExpirationModelOptions_expect_ArgumentNullException()
        {
            IServiceCollection serviceCollection = null;

            Assert.Throws<ArgumentNullException>(
                () =>
                    serviceCollection.AddHttpCacheHeaders(
                        (ExpirationModelOptions options) => options.MaxAge = 1,
                        (ValidationModelOptions options) => options.AddNoCache = true));
        }
    }
}