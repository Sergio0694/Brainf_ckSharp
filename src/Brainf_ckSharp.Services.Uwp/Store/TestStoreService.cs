using System;
using System.Threading.Tasks;

namespace Brainf_ckSharp.Services.Uwp.Store
{
    /// <summary>
    /// A <see langword="class"/> that manages the Windows Store in a test environment
    /// </summary>
    public sealed class TestStoreService : IStoreService
    {
        /// <inheritdoc/>
        public Task<bool> IsProductPurchasedAsync(string id) => Task.FromResult(true);

        /// <inheritdoc/>
        public Task<bool> TryPurchaseProductAsync(string id)
        {
            bool success = new Random().Next(0, 2) == 1;

            return Task.FromResult(success);
        }
    }
}
