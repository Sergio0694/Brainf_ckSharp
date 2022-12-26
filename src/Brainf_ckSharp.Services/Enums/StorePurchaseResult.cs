namespace Brainf_ckSharp.Services.Enums;

/// <summary>
/// An enum that represent the status of a request to purchase an app or add-on.
/// </summary>
public enum StorePurchaseResult
{
    /// <summary>
    /// The purchase request was successful
    /// </summary>
    Success,

    /// <summary>
    /// The current user has already purchased the specified app or add-on
    /// </summary>
    AlreadyPurchased,

    /// <summary>
    /// The purchase request did not succeed due to user cancelation
    /// </summary>
    NotPurchased,

    /// <summary>
    /// The purchase request did not succeed because of a network connectivity error
    /// </summary>
    NetworkError,

    /// <summary>
    /// The purchase request did not succeed because of a server error returned by the store
    /// </summary>
    ServerError,

    /// <summary>
    /// The purchase failed due to some unspecified error
    /// </summary>
    UnknownError
}
