using System.Diagnostics.Contracts;
using Brainf_ck_sharp.NET.Constants;
using Brainf_ck_sharp.NET.Enums;
using Brainf_ck_sharp.NET.Helpers;
using Brainf_ck_sharp.NET.Interfaces;
using Brainf_ck_sharp.NET.Models.Internal;

namespace Brainf_ck_sharp.NET.Tools
{
    /// <summary>
    /// A <see langword="class"/> that provides the ability to create empty machine states for later use
    /// </summary>
    public static class TuringMachineStateProvider
    {
        /// <summary>
        /// Gets the default machine state instance
        /// </summary>
        public static IReadOnlyTuringMachineState Default { get; } = new TuringMachineState(Specs.DefaultMemorySize, Specs.DefaultOverflowMode);

        /// <summary>
        /// Creates a new <see cref="IReadOnlyTuringMachineState"/> instance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the state machine to create</param>
        /// <returns>A new <see cref="IReadOnlyTuringMachineState"/> instance with the specified parameters</returns>
        [Pure]
        public static IReadOnlyTuringMachineState Create(int size) => Create(size, Specs.DefaultOverflowMode);

        /// <summary>
        /// Creates a new <see cref="IReadOnlyTuringMachineState"/> instance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the state machine to create</param>
        /// <param name="overflowMode">The overflow mode to use in the state machine to create</param>
        /// <returns>A new <see cref="IReadOnlyTuringMachineState"/> instance with the specified parameters</returns>
        [Pure]
        public static IReadOnlyTuringMachineState Create(int size, OverflowMode overflowMode)
        {
            Guard.MustBeGreaterThanOrEqualTo(size, 32, nameof(size));
            Guard.MustBeLessThanOrEqualTo(size, 1024, nameof(size));

            return new TuringMachineState(size, overflowMode);
        }
    }
}
