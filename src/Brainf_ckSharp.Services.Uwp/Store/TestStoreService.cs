using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Brainf_ckSharp.Services.Enums;

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
            if (Debugger.IsAttached) Debug.WriteLine("[STORE] Review requested");
            else Console.WriteLine("[STORE] Review requested");

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<bool> IsProductPurchasedAsync(string id) => Task.FromResult(true);

        /// <inheritdoc/>
        public Task<StorePurchaseResult> TryPurchaseProductAsync(string id)
        {
            return Task.FromResult(StorePurchaseResult.Success);
        }
    }
}
