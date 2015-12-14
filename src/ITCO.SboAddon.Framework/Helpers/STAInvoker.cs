using System;
using System.Threading;

namespace ITCO.SboAddon.Framework.Helpers
{
    /// <summary>
    /// Invoice STATThread Methods
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    /// <typeparam name="TReturn">Return type</typeparam>
    public class STAInvoker<T, TReturn>
    {
        private Thread _invokeThread;
        private Func<T, TReturn> _invoker;
        private TReturn _result;
        private T _statObject;

        /// <summary>
        /// Initialize Invoker with objec and invoice method
        /// </summary>
        /// <param name="staTreadObject"></param>
        /// <param name="invoker"></param>
        public STAInvoker(T staTreadObject, Func<T, TReturn> invoker)
        {
            _statObject = staTreadObject;
            _invoker = invoker;
            _invokeThread = new Thread(new ThreadStart(InvokeMethod));
            _invokeThread.SetApartmentState(ApartmentState.STA);
        }
        /// <summary>
        /// Invoke STATThread method
        /// </summary>
        /// <returns>TReturn</returns>
        public TReturn Invoke()
        {
            _invokeThread.Start();
            _invokeThread.Join();
            return _result;
        }

        private void InvokeMethod()
        {
            _result = _invoker.Invoke(_statObject);
        }
    }
}
