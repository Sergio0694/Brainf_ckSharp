namespace Brainf_ck_sharp.NET.Enum
{
    /// <summary>
    /// An <see langword="enum"/> that indicates the type of syntax error encountered while parsing a source file
    /// </summary>
    public enum SyntaxError
    {
        /// <summary>
        /// The input source code was parsed successfully
        /// </summary>
        None,

        /// <summary>
        /// A closed square bracket was either missing or exceeding the current depth level
        /// </summary>
        MismatchedSquareBracket,

        /// <summary>
        /// An open or closed parenthesis was placed incorrectly
        /// </summary>
        MismatchedParenthesis,

        /// <summary>
        /// A function was being defined within another function
        /// </summary>
        NestedFunctionDeclaration,

        /// <summary>
        /// A function definition had no operators in its body
        /// </summary>
        EmptyFunctionDeclaration,

        /// <summary>
        /// A function definition had not been completed with a closed parenthesis
        /// </summary>
        IncompleteFunctionDeclaration
    }
}
