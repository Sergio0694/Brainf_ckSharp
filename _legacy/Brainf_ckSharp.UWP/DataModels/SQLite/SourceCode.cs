using System;
using Brainf_ck_sharp.Legacy.UWP.Helpers;
using SQLite.Net.Attributes;

namespace Brainf_ck_sharp.Legacy.UWP.DataModels.SQLite
{
    /// <summary>
    /// A table that contains saved source codes along with their general info
    /// </summary>
    [Table(nameof(SourceCode))]
    public class SourceCode
    {
        /// <summary>
        /// The string representation of the Guid for the current instance
        /// </summary>
        [Column(nameof(Uid)), PrimaryKey]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the title of the saved code
        /// </summary>
        [Column(nameof(Title)), NotNull, Unique]
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the actual code stored as plain text
        /// </summary>
        [Column(nameof(Code)), NotNull]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the creation time for the current code
        /// </summary>
        [Column(nameof(Created)), NotNull, Default]
        public long Created { get; set; }

        /// <summary>
        /// Gets or sets the last edit time for the current code
        /// </summary>
        [Column(nameof(Modified)), NotNull, Default]
        public long Modified { get; set; }

        /// <summary>
        /// Gets or sets the delete time for the current code
        /// </summary>
        [Column(nameof(Deleted)), NotNull, Default]
        public long Deleted { get; set; }

        /// <summary>
        /// Gets or sets the flags for the current instance
        /// </summary>
        /// <remarks>Bit 0 - Favorite</remarks>
        [Column(nameof(Flags)), NotNull, Default]
        public uint Flags { get; set; }

        /// <summary>
        /// Gets or sets the enabled breakpoints for this source code
        /// </summary>
        /// <remarks>Each bit set to 1 represents a breakpoint on the n-th line</remarks>
        [Column(nameof(Breakpoints)), Default]
        public byte[] Breakpoints { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DateTime"/> object that indicates the creation time
        /// </summary>
        [Ignore]
        public DateTime CreatedTime
        {
            get => Created != 0 ? DateTime.FromBinary(Created) : DateTime.MinValue;
            set => Created = value.ToBinary();
        }
        
        /// <summary>
        /// Gets or sets a <see cref="DateTime"/> object that indicates the edit time
        /// </summary>
        [Ignore]
        public DateTime ModifiedTime
        {
            get => Modified != 0 ? DateTime.FromBinary(Modified) : DateTime.MinValue;
            set => Modified = value.ToBinary();
        }

        /// <summary>
        /// Gets or sets a <see cref="DateTime"/> object that indicates the delete time
        /// </summary>
        [Ignore]
        public DateTime DeletedTime
        {
            get => Deleted != 0 ? DateTime.FromBinary(Deleted) : DateTime.MinValue;
            set => Deleted = value.ToBinary();
        }

        /// <summary>
        /// Gets or sets whether or not the source code has been added to the favorites
        /// </summary>
        [Ignore]
        public bool Favorited
        {
            get => Flags.Test(0);
            set => Flags = Flags.Set(value, 0);
        }
    }
}
