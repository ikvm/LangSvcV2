﻿namespace Tvl.VisualStudio.Language.Java
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using Tvl.VisualStudio.Language.Intellisense;
    using VSOBJGOTOSRCTYPE = Microsoft.VisualStudio.Shell.Interop.VSOBJGOTOSRCTYPE;

    internal sealed class JavaIntellisenseController : IntellisenseController
    {
        public JavaIntellisenseController(ITextView textView, JavaIntellisenseControllerProvider provider)
            : base(textView, provider)
        {
        }

        public new JavaIntellisenseControllerProvider Provider
        {
            get
            {
                return (JavaIntellisenseControllerProvider)base.Provider;
            }
        }

        public override bool SupportsCompletion
        {
            get
            {
                return false;
            }
        }

        public override bool SupportsGotoDefinition
        {
            get
            {
                return true;
            }
        }

        public override IEnumerable<INavigateToTarget> GoToSourceImpl(VSOBJGOTOSRCTYPE gotoSourceType, ITrackingPoint triggerPoint)
        {
            if (triggerPoint == null)
                return new INavigateToTarget[0];

            return base.GoToSourceImpl(gotoSourceType, triggerPoint);
        }
    }
}
