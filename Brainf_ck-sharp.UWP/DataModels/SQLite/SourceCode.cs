using System;
using SQLite.Net.Attributes;

namespace Brainf_ck_sharp_UWP.DataModels.SQLite
{
    /// <summary>
    /// A table that contains saved source codes along with their general info
    /// </summary>
    [Table(nameof(SourceCode))]
    public class SourceCode
    {
        /// <summary>
        /// The String representation of the Guid for the current instance
        /// </summary>
        [Column(nameof(Uid)), PrimaryKey]
        public String Uid { get; set; }

        /// <summary>
        /// Gets or sets the title of the saved code
        /// </summary>
        [Column(nameof(Title)), NotNull, Unique]
        public String Title { get; set; }

        /// <summary>
        /// Gets or sets the actual code stored as plain text
        /// </summary>
        [Column(nameof(Code)), NotNull]
        public String Code { get; set; }

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
    }
}
