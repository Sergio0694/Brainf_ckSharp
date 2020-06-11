using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Services.Store;
using StorePurchaseResult = Brainf_ckSharp.Services.Enums.StorePurchaseResult;

namespace Brainf_ckSharp.Services.Uwp.Store
{
    /// <summary>
    /// A <see langword="class"/> that manages the Windows Store in a production environment
    /// </summary>
    public sealed class ProductionStoreService : IStoreService
    {
        /// <summary>
        /// The <see cref="Windows.Services.Store.StoreContext"/> instance to use for the current application
        /// </summary>
        private readonly StoreContext StoreContext = StoreContext.GetDefault();

        /// <inheritdoc/>
        public Task RequestReviewAsync()
        {
            if (Debugger.IsAttached) Debug.WriteLine("[STORE] Review requested");
            else Console.WriteLine("[STORE] Review requested");

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task<bool> IsProductPurchasedAsync(string id)
        {
            if (!(await StoreContext.GetAppLicenseAsync() is StoreAppLicense license)) return false;

            return license.AddOnLicenses
                .FirstOrDefault(pair => pair.Value.InAppOfferToken.Equals(id))
                .Value?.IsActive == true;
        }

        /// <inheritdoc/>
        public async Task<StorePurchaseResult> TryPurchaseProductAsync(string id)
        {
            try
            {

                var result = await StoreContext.RequestPurchaseAsync(id);

                return result.Status switch
                {
                    StorePurchaseStatus.Succeeded => StorePurchaseResult.Success,
                    StorePurchaseStatus.AlreadyPurchased => StorePurchaseResult.AlreadyPurchased,
                    StorePurchaseStatus.NotPurchased => StorePurchaseResult.NotPurchased,
                    StorePurchaseStatus.NetworkError => StorePurchaseResult.NetworkError,
                    StorePurchaseStatus.ServerError => StorePurchaseResult.ServerError,
                    _ => StorePurchaseResult.UnknownError
                };
            }
            catch
            {
                // Something went wrong
                return StorePurchaseResult.UnknownError;
            }
        }
    }
}
