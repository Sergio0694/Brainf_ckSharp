namespace Brainf_ck_sharp.NET.Models
{
    /// <summary>
    /// A <see langword="class"/> that contains the info on a function definition in a script
    /// </summary>
    public sealed class FunctionDefinition
    {
        /// <summary>
        /// Gets the numeric value associated with the function definition
        /// </summary>
        public ushort Value { get; }

        /// <summary>
        /// Gets the offset of the function in the original source code
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Gets the source code of the defined function
        /// </summary>
        public string Body { get; }

        /// <summary>
        /// Creates a new instance with the given parameters
        /// </summary>
        /// <param name="value">The function value</param>
        /// <param name="offset">The function script offset</param>
        /// <param name="body">The function code</param>
        public FunctionDefinition(ushort value, int offset, string body)
        {
            Value = value;
            Offset = offset;
            Body = body;
        }
    }
}
