using System.Collections.Generic;
using XamlStyler.Core.Parser;

namespace XamlStyler.Core.DocumentProcessors
{
    public class ElementProcessContext
    {
        private readonly Stack<ElementProcessStatus> _elementProcessStatusStack;

        public ElementProcessContext()
        {
            _elementProcessStatusStack = new Stack<ElementProcessStatus>();
            _elementProcessStatusStack.Push(new ElementProcessStatus());
        }

        public void Push(ElementProcessStatus elementProcessStatus)
        {
            _elementProcessStatusStack.Push(elementProcessStatus);
        }

        public ElementProcessStatus Pop()
        {
            return _elementProcessStatusStack.Pop();
        }

        public ElementProcessStatus Current => _elementProcessStatusStack.Peek();

        public int Count => _elementProcessStatusStack.Count;

        public void UpdateParentElementProcessStatus(ContentTypeEnum contentType)
        {
            ElementProcessStatus parentElementProcessStatus = _elementProcessStatusStack.Peek();

            parentElementProcessStatus.ContentType |= contentType;
        }
    }
}