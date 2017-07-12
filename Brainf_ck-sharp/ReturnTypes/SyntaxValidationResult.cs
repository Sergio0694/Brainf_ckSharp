﻿namespace Brainf_ck_sharp.ReturnTypes
{
    /// <summary>
    /// Indicates the result of a syntax validation check
    /// </summary>
    public struct SyntaxValidationResult
    {
        /// <summary>
        /// Gets whether or not the source code is valid and can be interpreted successfully
        /// </summary>
        public bool Valid { get; }

        public int ErrorPosition { get; }

        // Internal constructor
        internal SyntaxValidationResult(bool valid, int errorPosition)
        {
            Valid = valid;
            ErrorPosition = errorPosition;
        }
    }
}