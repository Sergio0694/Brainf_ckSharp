namespace Brainf_ck_sharp.NET.Enums
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
        /// A closed square bracket was exceeding the current depth level
        /// </summary>
        MismatchedSquareBracket,

        /// <summary>
        /// A closed square bracket was missing at the end of the source file
        /// </summary>
        IncompleteLoop,

        /// <summary>
        /// An closed parenthesis was placed incorrectly
        /// </summary>
        MismatchedParenthesis,

        /// <summary>
        /// A function was being defined within a loop and not at the root lebel of the source file
        /// </summary>
        InvalidFunctionDeclaration,

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
        IncompleteFunctionDeclaration,

        /// <summary>
        /// No valid operators have been found in the input source file
        /// </summary>
        MissingOperators,
    }
}
