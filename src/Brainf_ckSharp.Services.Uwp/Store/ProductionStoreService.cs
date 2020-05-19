using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Services.Store;

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
        public async Task<bool> IsProductPurchasedAsync(string id)
        {
            if (!(await StoreContext.GetAppLicenseAsync() is StoreAppLicense license)) return false;

            return license.AddOnLicenses
                .FirstOrDefault(pair => pair.Value.InAppOfferToken.Equals(id))
                .Value?.IsActive == true;
        }

        /// <inheritdoc/>
        public async Task<bool> TryPurchaseProductAsync(string id)
        {
            try
            {
                StorePurchaseResult result = await StoreContext.RequestPurchaseAsync(id);

                return result.Status == StorePurchaseStatus.Succeeded ||
                       result.Status == StorePurchaseStatus.AlreadyPurchased;
            }
            catch
            {
                // Something went wrong
                return false;
            }
        }
    }
}
