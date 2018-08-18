using System.Collections.Generic;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels
{
    public class CodeSnippetsBrowserViewModel
    {
        /// <summary>
        /// Gets the collection of availale <see cref="CodeSnippet"/> instances
        /// </summary>
        [NotNull, ItemNotNull]
        public IReadOnlyList<CodeSnippet> CodeSnippets { get; } = new[]
        {
            new CodeSnippet("Inline loop", "[]", 1), 
            new CodeSnippet("Reset loop", "[-]", 3),
            new CodeSnippet("Duplicate value", "[>+>+<<-]>>[<<+>>-]<<", null),
            new CodeSnippet("if (x == 0) then { }", ">+<[-]>\r[\r->\r// TODO\r[-]\r]<<", 10) 
        };
    }
}
