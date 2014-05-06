using System;

namespace EndpointSystems.BizTalk.Documentation
{
    /// <summary>
    /// Act as the observer of all work being performed.
    /// </summary>
    public class ProgressObserver: IObserver<OrchestrationImage>
    {
        #region Implementation of IObserver<OrchestrationImage>

        /// <summary>
        /// Notifies the observer of a new value in the sequence.
        /// </summary>
        public void OnNext(OrchestrationImage value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Notifies the observer that an exception has occurred.
        /// </summary>
        public void OnError(Exception exception)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Notifies the observer of the end of the sequence.
        /// </summary>
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
