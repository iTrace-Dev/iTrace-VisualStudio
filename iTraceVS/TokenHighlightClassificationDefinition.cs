using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace iTraceVS
{
    /// <summary>
    /// Classification type definition export for TokenHighlight
    /// </summary>
    internal static class TokenHighlightClassificationDefinition
    {
        // This disables "The field is never used" compiler's warning. Justification: the field is used by MEF.
#pragma warning disable 169

        /// <summary>
        /// Defines the "TokenHighlight" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("TokenHighlight")]
        private static ClassificationTypeDefinition typeDefinition;

#pragma warning restore 169
    }
}
