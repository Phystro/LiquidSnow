﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Thismaker.Aba.Client
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Automatically configure the client, calling the required methods afterwards.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static AbaClientBuilder<T> WithConfiguration<T>(this AbaClientBuilder<T> aba, IConfiguration config)where T:ClientBase<T>, new()
        {
            aba.client.Configure(config);

            return aba;
        }

        /// <summary>
        /// Configures the app using the provided Configuration, calling <see cref="ClientBase{T}.MakeApp"/> once done.
        /// Use this to quickly assign values to the msal client. The configuration section must be named "AbaClient"
        /// </summary>
        public static void Configure<T>(this ClientBase<T> client, IConfiguration config) where T : ClientBase<T>
        {
            config.Bind("AbaClient", client);
            client.MakeApp();
        }
    }
}
