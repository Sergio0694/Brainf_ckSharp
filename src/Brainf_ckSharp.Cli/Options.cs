using Brainf_ckSharp.Enums;
using CommandLine;

namespace Brainf_ckSharp.Cli
{
    /// <summary>
    /// A model with all the options for a script invocation
    /// </summary>
    public sealed class Options
    {
        /// <summary>
        /// Gets or sets the source code to execute
        /// </summary>
        [Option(
            's',
            "source",
            HelpText = "The source code to execute",
            SetName = "source")]
        public string? Source { get; set; }

        /// <summary>
        /// Gets or sets the source file with the code to execute
        /// </summary>
        [Option(
            'f',
            "file",
            HelpText = "The source file with the code to execute",
            SetName = "source")]
        public string? SourceFile { get; set; }

        /// <summary>
        /// Gets or sets the stdin buffer to pass to the script
        /// </summary>
        [Option(
            "stdin",
            HelpText = "The stdin buffer to pass to the script",
            Required = false)]
        public string? Stdin { get; set; }

        /// <summary>
        /// Gets or sets the path of the file with the buffer to pass to the script
        /// </summary>
        [Option(
            "stdin-file",
            HelpText = "The path of the file with the buffer to pass to the script",
            Required = false)]
        public string? StdinFile { get; set; }

        /// <summary>
        /// Gets or sets the size of the memory buffer to use
        /// </summary>
        [Option(
            "memory-size",
            Default = 128,
            HelpText = "The size of the memory buffer to use [32, 65536]",
            Required = false)]
        public int MemorySize { get; set; }

        /// <summary>
        /// Gets or sets the overflow mode to use for the memory buffer
        /// </summary>
        [Option(
            "overflow",
            Default = OverflowMode.ByteWithOverflow,
            HelpText = "The overflow mode to use for the memory buffer [ByteWithOverflow|ByteWithNoOverflow|UshortWithOverflow|UshortWithNoOverflow]",
            Required = false)]
        public OverflowMode OverflowMode { get; set; }

        /// <summary>
        /// Gets or sets the path for a file to dump the output buffer to
        /// </summary>
        [Option(
            "stdout-file",
            HelpText = "The path for a file to dump the output buffer to",
            Required = false)]
        public string? Stdout { get; set; }

        /// <summary>
        /// Gets or sets the timeout for the script to execute
        /// </summary>
        [Option(
            't',
            "timeout",
            Default = 2,
            HelpText = "The timeout in seconds for the script to execute (0 to disable)",
            Required = false)]
        public int Timeout { get; set; }

        /// <summary>
        /// Gets or sets whether or not to notify with a sound when the execution completes
        /// </summary>
        [Option(
            "beep",
            HelpText = "Indicates whether or not to notify with a sound when the execution completes",
            Required = false)]
        public bool Beep { get; set; }
    }
}
