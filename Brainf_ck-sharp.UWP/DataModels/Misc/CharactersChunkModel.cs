using System;
using System.Linq;
using Brainf_ck_sharp.MemoryState;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.DataModels.Misc
{
    /// <summary>
    /// A class that represents a group of 4 contiguous memory cells
    /// </summary>
    public class CharactersChunkModel
    {
        // The current chunk
        private readonly Brainf_ckMemoryCell[] Chunk;

        /// <summary>
        /// Gets the first memory cell
        /// </summary>
        public Brainf_ckMemoryCell _1st => Chunk[0];

        /// <summary>
        /// Gets the second memory cell
        /// </summary>
        public Brainf_ckMemoryCell _2nd => Chunk[1];

        /// <summary>
        /// Gets the third memory cell
        /// </summary>
        public Brainf_ckMemoryCell _3rd => Chunk[2];

        /// <summary>
        /// Gets the fourth memory cell
        /// </summary>
        public Brainf_ckMemoryCell _4th => Chunk[3];

        /// <summary>
        /// Gets the offset of the first memory cell in the chunk with respect to the whole memory state
        /// </summary>
        public uint BaseOffset { get; }

        /// <summary>
        /// Gets whether or not the current position is within the current chunk
        /// </summary>
        public bool ChunkSelected => Chunk.Any(c => c.Selected);

        /// <summary>
        /// Gets the index of the selected cell in the current chunk, if present
        /// </summary>
        public int SelectedIndex => Chunk.IndexOf(c => c.Selected);

        /// <summary>
        /// Creates a new instance for a chunk of memory cells
        /// </summary>
        /// <param name="chunk">The current group of memory cells</param>
        /// <param name="offset">The chunk offset in the memory state</param>
        public CharactersChunkModel([NotNull] Brainf_ckMemoryCell[] chunk, int offset)
        {
            if (chunk.Length != 4) throw new ArgumentOutOfRangeException("The chunk must contain 4 memory cells");
            if (offset < 0 || offset % 4 != 0) throw new ArgumentOutOfRangeException("The memory offset isn't valid");
            Chunk = chunk;
            BaseOffset = (uint)offset;
        }
    }
}
