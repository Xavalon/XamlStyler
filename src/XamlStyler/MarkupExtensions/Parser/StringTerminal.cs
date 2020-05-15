// (c) Xavalon. All rights reserved.

namespace Xavalon.XamlStyler.MarkupExtensions.Parser
{
    internal class StringTerminal : MemberNameOrStringTerminal
    {
        public StringTerminal(string name) : base(name)
        {
        }

        protected override bool IsMemberName => false;
    }
}