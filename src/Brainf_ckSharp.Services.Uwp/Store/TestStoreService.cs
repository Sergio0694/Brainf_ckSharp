using System;
using System.Threading.Tasks;
using Brainf_ckSharp.Services.Enums;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Brainf_ckSharp.Services.Uwp.Store
{
    /// <summary>
    /// A <see langword="class"/> that manages the Windows Store in a test environment
    /// </summary>
    public sealed class TestStoreService : IStoreService
    {
        /// <inheritdoc/>
        public Task RequestReviewAsync()
        {
            return SystemInformation.LaunchStoreForReviewAsync();
        }

        /// <inheritdoc/>
        public Task<bool> IsProductPurchasedAsync(string id) => Task.FromResult(true);

        /// <inheritdoc/>
        public Task<StorePurchaseResult> TryPurchaseProductAsync(string id)
        {
            bool success = new Random().Next(0, 2) == 1;

            return Task.FromResult(StorePurchaseResult.Success);
        }
    }
}
