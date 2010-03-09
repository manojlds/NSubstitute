using System;

namespace NSubstitute
{
    public class InvocationHandler : IInvocationHandler
    {
        readonly IInvocationStack _invocationStack;
        readonly IInvocationResults _invocationResults;
        readonly ISubstitutionContext _context;
        bool _assertNextCallReceived;

        public InvocationHandler(IInvocationStack invocationStack, IInvocationResults invocationResults, ISubstitutionContext context)
        {
            _invocationStack = invocationStack;
            _invocationResults = invocationResults;
            _context = context;
        }

        public void LastInvocationShouldReturn<T>(T valueToReturn)
        {
            var lastCall = _invocationStack.Pop();
            _invocationResults.SetResult(lastCall, valueToReturn);
        }

        public object HandleInvocation(IInvocation invocation)
        {
            if (_assertNextCallReceived)
            {
                _assertNextCallReceived = false;
                _invocationStack.ThrowIfCallNotFound(invocation);
                return _invocationResults.GetDefaultResultFor(invocation);
            }
            _invocationStack.Push(invocation);
            _context.LastInvocationHandlerInvoked(this);
            return _invocationResults.GetResult(invocation);
        }

        public void AssertNextCallHasBeenReceived()
        {
            _assertNextCallReceived = true;
        }
    }
}