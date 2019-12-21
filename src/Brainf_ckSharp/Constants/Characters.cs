namespace Brainf_ckSharp.Constants
{
    /// <summary>
    /// A <see langword="class"/> that exposes the collection of available Brainf*ck/PBrain characters
    /// </summary>
    public static class Characters
    {
        /// <summary>
        /// The <see langword="+"/> operator, that increments the current memory cell (<see langword="(*ptr)++"/>)
        /// </summary>
        public const char Plus = '+';

        /// <summary>
        /// The <see langword="-"/> operator, that decrements the current memory cell (<see langword="(*ptr)--"/>)
        /// </summary>
        public const char Minus = '-';

        /// <summary>
        /// The <see langword=">"/> operator, that moves the memory pointer forwards (<see langword="++ptr"/>)
        /// </summary>
        public const char ForwardPtr = '>';

        /// <summary>
        /// The <see langword="&lt;"/> operator, that moves the memory pointer backwards (<see langword="--ptr"/>)
        /// </summary>
        public const char BackwardPtr = '<';

        /// <summary>
        /// The <see langword="."/> operator, prints the current memory cell (<see langword="putchar(*ptr)"/>)
        /// </summary>
        public const char PrintChar = '.';

        /// <summary>
        /// The <see langword=","/> operator, that reads a value for the current memory cell (<see langword="*ptr = getchar()"/>)
        /// </summary>
        public const char ReadChar = ',';

        /// <summary>
        /// The <see langword="["/> operator, that starts a loop if the current cell is not 0 (<see langword="while (*ptr) {"/>)
        /// </summary>
        public const char LoopStart = '[';

        /// <summary>
        /// The <see langword="]"/> operator, that ands the current loop (<see langword="}"/>)
        /// </summary>
        public const char LoopEnd = ']';

        /// <summary>
        /// The <see langword="("/> operator, that starts a function definition (<see langword="f[*ptr] = []() {"/>)
        /// </summary>
        public const char FunctionStart = '(';

        /// <summary>
        /// The <see langword=")"/> operator, that completes the current function definition (<see langword="}"/>)
        /// </summary>
        public const char FunctionEnd = ')';

        /// <summary>
        /// The <see langword=":"/> operator, that invokes a specified function (<see langword="f[*ptr]()"/>)
        /// </summary>
        public const char FunctionCall = ':';
    }
}
