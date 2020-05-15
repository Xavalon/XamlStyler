// (c) Xavalon. All rights reserved.

using System.Collections.Generic;
using Xavalon.XamlStyler.Parser;

namespace Xavalon.XamlStyler.DocumentProcessors
{
    public class ElementProcessContext
    {
        private readonly Stack<ElementProcessStatus> elementProcessStatusStack;

        public int Count => this.elementProcessStatusStack.Count;

        public ElementProcessStatus Current => this.elementProcessStatusStack.Peek();

        public ElementProcessContext()
        {
            this.elementProcessStatusStack = new Stack<ElementProcessStatus>();
            this.elementProcessStatusStack.Push(new ElementProcessStatus());
        }

        public void Push(ElementProcessStatus elementProcessStatus)
        {
            this.elementProcessStatusStack.Push(elementProcessStatus);
        }

        public ElementProcessStatus Pop()
        {
            return this.elementProcessStatusStack.Pop();
        }

        public void UpdateParentElementProcessStatus(ContentTypes contentType)
        {
            ElementProcessStatus parentElementProcessStatus = this.elementProcessStatusStack.Peek();
            parentElementProcessStatus.ContentType |= contentType;
        }
    }
}