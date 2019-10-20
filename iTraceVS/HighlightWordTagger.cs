using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.Windows.Media;

namespace iTraceVS {

    internal class HighlightWordTag : TextMarkerTag {
        public HighlightWordTag() : base("MarkerFormatDefinition/HighlightWordFormatDefinition") { }
    }

    [Export(typeof(EditorFormatDefinition))]
    [Name("MarkerFormatDefinition/HighlightWordFormatDefinition")]
    [UserVisible(true)]
    internal class HighlightWordFormatDefinition : MarkerFormatDefinition {
        public HighlightWordFormatDefinition() {
            BackgroundColor = Colors.LightBlue;
            ForegroundColor = Colors.DarkBlue;
            DisplayName = "Highlight Word";
            ZOrder = 5;
        }
    }

    internal class HighlightWordTagger : ITagger<HighlightWordTag> {
        ITextView View { get; set; }
        ITextBuffer SourceBuffer { get; set; }
        ITextSearchService TextSearchService { get; set; }
        ITextStructureNavigator TextStructureNavigator { get; set; }
        SnapshotSpan? CurrentWord { get; set; }
        SnapshotPoint RequestedPoint { get; set; }
        object updateLock = new object();
        System.Windows.Forms.Timer timer;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public HighlightWordTagger(ITextView view, ITextBuffer sourceBuffer, ITextSearchService textSearchService, ITextStructureNavigator textStructureNavigator) {
            View = view;
            SourceBuffer = sourceBuffer;
            TextSearchService = textSearchService;
            TextStructureNavigator = textStructureNavigator;
            CurrentWord = null;
            timer = new System.Windows.Forms.Timer() { Interval = 50, Enabled = true };
            timer.Tick += new EventHandler(TimerTick);
        }

        void TimerTick(object sender, EventArgs e) {
            if (WindowControl.highlighting && WindowControl.connected) {
                UpdateAtGazePosition();
            }
        }

        void UpdateAtGazePosition() {
            SnapshotPoint? point = XmlWriter.bufferPos;

            if (!point.HasValue)
                return;

            //If the new gaze position is still within the current word, we don't need to check it   
            if (CurrentWord.HasValue
                && CurrentWord.Value.Snapshot == View.TextSnapshot
                && point.Value >= CurrentWord.Value.Start
                && point.Value <= CurrentWord.Value.End) {
                return;
            }

            RequestedPoint = point.Value;
            UpdateWordAdornments();
        }

        void UpdateWordAdornments() {
            SnapshotPoint currentRequest = RequestedPoint;             
            TextExtent word = TextStructureNavigator.GetExtentOfWord(currentRequest);
            bool foundWord = true;

            //If we've selected something not worth highlighting, we might have missed a "word" by a little bit  
            if (!WordExtentIsValid(currentRequest, word)) {
                //Before we retry, make sure it is worthwhile   
                if (word.Span.Start != currentRequest
                     || currentRequest == currentRequest.GetContainingLine().Start
                     || char.IsWhiteSpace((currentRequest - 1).GetChar())) {
                    foundWord = false;
                }
                else {
                    //Try again, one character previous.    
                    //If looking at the end of a word, pick up the word.  
                    word = TextStructureNavigator.GetExtentOfWord(currentRequest - 1);

                    //If the word still isn't valid, we're done   
                    if (!WordExtentIsValid(currentRequest, word))
                        foundWord = false;
                }
            }

            if (!foundWord) {
                //If we couldn't find a word, clear out the existing markers  
                SynchronousUpdate(currentRequest, null);
                return;
            }

            SnapshotSpan currentWord = word.Span;
            //If this is the current word, and the user's eyes moved within a word, we're done.   
            if (CurrentWord.HasValue && currentWord == CurrentWord)
                return;

            //If another change hasn't happened, do a real update   
            if (currentRequest == RequestedPoint)
                SynchronousUpdate(currentRequest, currentWord);
        }

        static bool WordExtentIsValid(SnapshotPoint currentRequest, TextExtent word) {
            return word.IsSignificant && currentRequest.Snapshot.GetText(word.Span).Any(c => char.IsLetterOrDigit(c));
        }

        void SynchronousUpdate(SnapshotPoint currentRequest, SnapshotSpan? newCurrentWord) {
            lock (updateLock) {
                if (currentRequest != RequestedPoint)
                    return;

                CurrentWord = newCurrentWord;
                TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(SourceBuffer.CurrentSnapshot, 0, SourceBuffer.CurrentSnapshot.Length)));
            }
        }

        public IEnumerable<ITagSpan<HighlightWordTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
            if (CurrentWord == null)
                yield break;

            //Hold on to a "snapshot" of the word spans and current word, so that we maintain the same collection throughout
            SnapshotSpan currentWord = CurrentWord.Value;

            if (spans.Count == 0)
                yield break;

            if (spans.OverlapsWith(new NormalizedSnapshotSpanCollection(currentWord)))
                yield return new TagSpan<HighlightWordTag>(currentWord, new HighlightWordTag());
        }

        [Export(typeof(IViewTaggerProvider))]
        [ContentType("text")]
        [TagType(typeof(TextMarkerTag))]
        internal class HighlightWordTaggerProvider : IViewTaggerProvider {
            [Import]
            internal ITextSearchService TextSearchService { get; set; }

            [Import]
            internal ITextStructureNavigatorSelectorService TextStructureNavigatorSelector { get; set; }

            public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag {
                //provide highlighting only on the top buffer   
                //if (textView.TextBuffer != buffer)
                //return null;
                ITextStructureNavigator textStructureNavigator = 
                    TextStructureNavigatorSelector.GetTextStructureNavigator(buffer);

                return new HighlightWordTagger(textView, buffer, TextSearchService, textStructureNavigator) as ITagger<T>;
            }
        }
    }
}
