using System.Threading.Tasks;
using Brainf_ckSharp.Services.Enums;

namespace Brainf_ckSharp.Services;

/// <summary>
/// The default <see langword="interface"/> for a Store service
/// </summary>
public interface IStoreService
{
    /// <summary>
    /// Requests a review in the Store for the current application
    /// </summary>
    Task RequestReviewAsync();

    /// <summary>
    /// Checks if a product with a given id has already been purchased by the current user
    /// </summary>
    /// <param name="id">The id of the product to check</param>
    Task<bool> IsProductPurchasedAsync(string id);

    /// <summary>
    /// Tries to purchase a product with the specified id
    /// </summary>
    /// <param name="id">The id of the product to purchase</param>
    /// <returns>Whether the purchase was successful</returns>
    Task<StorePurchaseResult> TryPurchaseProductAsync(string id);
}
