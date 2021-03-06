﻿using System;
using WeihanLi.Common;

#if NET45
namespace WeihanLi.AspNetMvc.AccessControlHelper
{
    public static class AccessControlHelper
    {
        public static void RegisterAccessControlHelper<TResourceStragety, TControlStragety>(Func<IServiceProvider> registerFunc)
            where TResourceStragety : class, IResourceAccessStrategy
            where TControlStragety : class, IControlAccessStrategy
        {
            DependencyResolver.SetDependencyResolver(registerFunc());
        }

        public static void RegisterAccessControlHelper<TResourceStragety, TControlStragety>(Func<Type, object> getServiceFunc)
            where TResourceStragety : class, IResourceAccessStrategy
            where TControlStragety : class, IControlAccessStrategy
        {
            DependencyResolver.SetDependencyResolver(getServiceFunc);
        }

        public static void RegisterAccessControlHelper<TResourceStragety, TControlStragety>(Action<Type, Type> registerTypeAsAction, Func<Type, object> getServiceFunc)
            where TResourceStragety : class, IResourceAccessStrategy
            where TControlStragety : class, IControlAccessStrategy
        {
            registerTypeAsAction(typeof(TResourceStragety), typeof(IResourceAccessStrategy));
            registerTypeAsAction(typeof(TControlStragety), typeof(IControlAccessStrategy));

            DependencyResolver.SetDependencyResolver(getServiceFunc);
        }
    }
}
#else

using WeihanLi.AspNetMvc.AccessControlHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAccessControlHelper(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            return app.UseMiddleware<AccessControlHelperMiddleware>();
        }
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register resource access strategy
        /// </summary>
        /// <typeparam name="TResourceAccessStrategy">TControlStrategy</typeparam>
        /// <param name="services">services</param>
        /// <param name="configAction">config for middleware</param>
        /// <returns>services</returns>
        [Obsolete("Please use AddAccessControlHelper().AddResourceAccessStrategy() instead", true)]
        public static IServiceCollection RegisterResourceAccessStrategy<TResourceAccessStrategy>(
            this IServiceCollection services, Action<AccessControlOptions> configAction = null) where TResourceAccessStrategy : class, IResourceAccessStrategy
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configAction != null)
            {
                services.Configure(configAction);
            }

            services.AddAccessControlHelper()
                .AddResourceAccessStrategy<TResourceAccessStrategy>();

            return services;
        }

        /// <summary>
        /// Register view control access strategy
        /// </summary>
        /// <typeparam name="TControlStrategy">TControlStrategy</typeparam>
        /// <param name="services">services</param>
        /// <returns>services</returns>
        [Obsolete("Please use AddAccessControlHelper().AddControlAccessStrategy() instead", true)]
        public static IServiceCollection RegisterControlAccessStrategy<TControlStrategy>(
            this IServiceCollection services) where TControlStrategy : class, IControlAccessStrategy
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.TryAddSingleton<IControlAccessStrategy, TControlStrategy>();
            return services;
        }

        public static IAccessControlHelperBuilder AddAccessControlHelper<TResourceAccessStrategy, TControlStrategy>(this IServiceCollection services)
            where TResourceAccessStrategy : class, IResourceAccessStrategy
            where TControlStrategy : class, IControlAccessStrategy
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddSingleton<IResourceAccessStrategy, TResourceAccessStrategy>();
            services.TryAddSingleton<IControlAccessStrategy, TControlStrategy>();

            return services.AddAccessControlHelper();
        }

        public static IAccessControlHelperBuilder AddAccessControlHelper<TResourceAccessStrategy, TControlStrategy>(this IServiceCollection services, ServiceLifetime resourceAccessStrategyLifetime, ServiceLifetime controlAccessStrategyLifetime)
            where TResourceAccessStrategy : class, IResourceAccessStrategy
            where TControlStrategy : class, IControlAccessStrategy
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAdd(new ServiceDescriptor(typeof(IResourceAccessStrategy), typeof(TResourceAccessStrategy), resourceAccessStrategyLifetime));
            services.TryAdd(new ServiceDescriptor(typeof(IControlAccessStrategy), typeof(TControlStrategy), controlAccessStrategyLifetime));

            return services.AddAccessControlHelper();
        }

        public static IAccessControlHelperBuilder AddAccessControlHelper<TResourceAccessStrategy, TControlStrategy>(this IServiceCollection services, Action<AccessControlOptions> configAction)
            where TResourceAccessStrategy : class, IResourceAccessStrategy
            where TControlStrategy : class, IControlAccessStrategy
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configAction != null)
            {
                services.Configure(configAction);
            }
            return services.AddAccessControlHelper<TResourceAccessStrategy, TControlStrategy>();
        }

        public static IAccessControlHelperBuilder AddAccessControlHelper<TResourceAccessStrategy, TControlStrategy>(this IServiceCollection services, Action<AccessControlOptions> configAction, ServiceLifetime resourceAccessStrategyLifetime, ServiceLifetime controlAccessStrategyLifetime)
            where TResourceAccessStrategy : class, IResourceAccessStrategy
            where TControlStrategy : class, IControlAccessStrategy
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configAction != null)
            {
                services.Configure(configAction);
            }
            return services.AddAccessControlHelper<TResourceAccessStrategy, TControlStrategy>(resourceAccessStrategyLifetime, controlAccessStrategyLifetime);
        }

        public static IAccessControlHelperBuilder AddAccessControlHelper(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.AddAuthorization(options => options.AddPolicy(AccessControlHelperConstants.PolicyName, new AuthorizationPolicyBuilder().AddRequirements(new AccessControlRequirement()).Build()));
            services.AddSingleton<IAuthorizationHandler, AccessControlAuthorizationHandler>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return new AccessControlHelperBuilder(services);
        }

        public static IAccessControlHelperBuilder AddAccessControlHelper(this IServiceCollection services, Action<AccessControlOptions> configAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configAction != null)
            {
                services.Configure(configAction);
            }
            return services.AddAccessControlHelper();
        }

        public static IAccessControlHelperBuilder AddResourceAccessStrategy<TResourceAccessStrategy>(this IAccessControlHelperBuilder builder) where TResourceAccessStrategy : IResourceAccessStrategy
        {
            return AddResourceAccessStrategy<TResourceAccessStrategy>(builder, ServiceLifetime.Singleton);
        }

        public static IAccessControlHelperBuilder AddResourceAccessStrategy<TResourceAccessStrategy>(this IAccessControlHelperBuilder builder, ServiceLifetime serviceLifetime) where TResourceAccessStrategy : IResourceAccessStrategy
        {
            if (null == builder)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.Add(
                new ServiceDescriptor(typeof(IResourceAccessStrategy), typeof(TResourceAccessStrategy), serviceLifetime));
            return builder;
        }

        public static IAccessControlHelperBuilder AddControlAccessStrategy<TControlAccessStrategy>(this IAccessControlHelperBuilder builder) where TControlAccessStrategy : IControlAccessStrategy
        {
            return AddControlAccessStrategy<TControlAccessStrategy>(builder, ServiceLifetime.Singleton);
        }

        public static IAccessControlHelperBuilder AddControlAccessStrategy<TControlAccessStrategy>(this IAccessControlHelperBuilder builder, ServiceLifetime serviceLifetime) where TControlAccessStrategy : IControlAccessStrategy
        {
            if (null == builder)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.Add(new ServiceDescriptor(typeof(IControlAccessStrategy), typeof(TControlAccessStrategy), serviceLifetime));
            return builder;
        }
    }
}

#endif
