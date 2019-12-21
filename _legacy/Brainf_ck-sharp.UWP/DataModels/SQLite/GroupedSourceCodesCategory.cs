using System.Collections.Generic;
using Brainf_ck_sharp_UWP.DataModels.SQLite.Enums;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.DataModels.SQLite
{
    /// <summary>
    /// A simple class that contains the info on a category of source codes and the contained items
    /// </summary>
    public class GroupedSourceCodesCategory
    {
        /// <summary>
        /// Gets the current category of source codes
        /// </summary>
        public SavedSourceCodeType Type { get; }

        /// <summary>
        /// Gets the list of source codes in the current category
        /// </summary>
        [NotNull]
        public IList<SourceCode> Items { get; }

        /// <summary>
        /// Creates a new category with the given parameters
        /// </summary>
        /// <param name="type">The category of source codes</param>
        /// <param name="items">The source codes in the new category</param>
        public GroupedSourceCodesCategory(SavedSourceCodeType type, [NotNull] IList<SourceCode> items)
        {
            Type = type;
            Items = items;
        }
    }
}