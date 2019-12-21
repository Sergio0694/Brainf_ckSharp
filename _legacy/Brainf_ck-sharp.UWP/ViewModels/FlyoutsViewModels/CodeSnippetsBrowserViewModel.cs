using System.Collections.Generic;
using Brainf_ck_sharp.Legacy.UWP.DataModels;
using Brainf_ck_sharp.Legacy.UWP.DataModels.Misc;
using Brainf_ck_sharp.Legacy.UWP.Helpers.UI;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.ViewModels.FlyoutsViewModels
{
    public class CodeSnippetsBrowserViewModel
    {
        /// <summary>
        /// Gets the collection of availale <see cref="CodeSnippet"/> instances
        /// </summary>
        [NotNull, ItemNotNull]
        public IEnumerable<IndexedModelWithValue<CodeSnippet>> CodeSnippets { get; } = IndexedModelWithValue<CodeSnippet>.New(new[]
        {
            new CodeSnippet(LocalizationManager.GetResource("SnippetInlineLoop"), "[]", 1),
            new CodeSnippet(LocalizationManager.GetResource("SnippetResetCell"), "[-]", 3),
            new CodeSnippet(LocalizationManager.GetResource("SnippetDuplicateValue"), "[>+>+<<-]>>[<<+>>-]<<", 21),
            new CodeSnippet(LocalizationManager.GetResource("SnippetNestedLoop"), ">+\r[\r\r]<-", 5), 
            new CodeSnippet("if (x == 0) then { }", ">+<[>-]>\r[\r->\r\r[-]\r]<<", 14),
            new CodeSnippet("if (x > 0) then { } else { }", ">+<\r[\r>\r\r[-]\r]>\r[\r->\r\r[-]\r]<<", 8)
        });

        /* ============================================
         * Instructions to add new snippets correctly
         * ============================================
         * 1) Add the new snippet here, with the right cursor offset (counting the new lines too). The
         *    snippets here are always assumed to be formatted with open [ operators on a new line.
         * 2) Adjust the fixed height of the two snippets lists (holding the < button, and by right click)
         * 3) Increment the index (the number of total snippets) in the code behind of the snippets template */
    }
}
