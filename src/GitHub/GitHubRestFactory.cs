﻿using System;
using System.Net.Http;
using GitHub.APIs;
using GitHub.HttpHandlers;
using Refit;

namespace GitHub;

/// <summary>
/// The entry point to retrieve instances of GitHub services
/// </summary>
public static class GitHubRestFactory
{
    /// <summary>
    /// Gets the base URL for the service
    /// </summary>
    public const string BaseUrl = "https://api.github.com/";

    /// <summary>
    /// Gets a new instance of an unauthenticated GitHub service
    /// </summary>
    /// <param name="userAgent">The user agent to use to send web requests</param>
    public static IGitHubService GetGitHubService(string userAgent)
    {
        HttpClient client = new(new UnauthenticatedHttpClientHandler(userAgent)) { BaseAddress = new Uri(BaseUrl) };
        RefitSettings settings = new(new SystemTextJsonContentSerializer());

        return RestService.For<IGitHubService>(client, settings);
    }
}
