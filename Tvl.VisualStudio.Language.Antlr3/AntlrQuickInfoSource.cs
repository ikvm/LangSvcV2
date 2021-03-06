﻿namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;

    internal sealed class AntlrQuickInfoSource : IQuickInfoSource
    {
        public AntlrQuickInfoSource(ITextBuffer textBuffer, AntlrBackgroundParser backgroundParser, ITagAggregator<ClassificationTag> aggregator)
        {
            this.Aggregator = aggregator;
            this.BackgroundParser = backgroundParser;
            this.TextBuffer = textBuffer;
        }

        public ITextBuffer TextBuffer
        {
            get;
            private set;
        }

        public AntlrBackgroundParser BackgroundParser
        {
            get;
            private set;
        }

        public ITagAggregator<ClassificationTag> Aggregator
        {
            get;
            private set;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
        {
            applicableToSpan = null;

            if (session.TextView.TextBuffer == this.TextBuffer)
            {
                ITextSnapshot currentSnapshot = this.TextBuffer.CurrentSnapshot;
                SnapshotPoint? triggerPoint = session.GetTriggerPoint(currentSnapshot);
                if (!triggerPoint.HasValue)
                    return;

                foreach (var span in this.Aggregator.GetTags(new SnapshotSpan(triggerPoint.Value, triggerPoint.Value)))
                {
                    if (!span.Tag.ClassificationType.IsOfType(AntlrClassificationTypeNames.LexerRule)
                        && !span.Tag.ClassificationType.IsOfType(AntlrClassificationTypeNames.ParserRule))
                    {
                        continue;
                    }

                    NormalizedSnapshotSpanCollection spans = span.Span.GetSpans(currentSnapshot);
                    if (spans.Count == 1)
                    {
                        SnapshotSpan span2 = spans[0];
                        SnapshotSpan span3 = span.Span.GetSpans(span.Span.AnchorBuffer)[0];
                        if (span2.Length == span3.Length)
                        {
                            SnapshotSpan span4 = spans[0];
                            if (span4.Contains(triggerPoint.Value))
                            {
                                StringBuilder builder = new StringBuilder();
                                string ruleName = span2.GetText();
                                var rules = BackgroundParser.RuleSpans;
                                KeyValuePair<ITrackingSpan, ITrackingPoint> value;
                                if (rules == null || !rules.TryGetValue(ruleName, out value))
                                {
#if DEBUG
                                    if (span.Tag.ClassificationType.IsOfType(AntlrClassificationTypeNames.LexerRule))
                                        builder.Append("Found an unknown lexer rule.");
                                    else
                                        builder.Append("Found an unknown parser rule.");
#else
                                    return;
#endif
                                }
                                else
                                {
                                    SnapshotPoint ruleSeek = value.Value.GetPoint(triggerPoint.Value.Snapshot);
                                    if (span2.Contains(ruleSeek))
                                        return;

                                    builder.Append(value.Key.GetText(span2.Snapshot));
                                }

                                //builder.AppendLine(span.Tag.Url.OriginalString);
                                //builder.Append(Strings.UrlQuickInfoFollowLink);
                                quickInfoContent.Add(builder.ToString());
                                applicableToSpan = currentSnapshot.CreateTrackingSpan((Span)spans[0], SpanTrackingMode.EdgeExclusive);
                            }
                        }
                    }
                }
            }
        }

        private void Dispose(bool disposing)
        {
        }
    }
}
