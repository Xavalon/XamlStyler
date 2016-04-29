namespace Xavalon.XamlStyler.Core.MarkupExtensions.Parser
{
    internal class StringTerminal : MemberNameOrStringTerminal
    {
        public StringTerminal(string name): base(name)
        {
        }

        protected override bool IsMemberName => false;
    }
}