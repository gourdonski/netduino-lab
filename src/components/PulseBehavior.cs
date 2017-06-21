using System;
using Microsoft.SPOT;
using NetduinoLab.Components.Abstract;
using System.Threading;
using Microsoft.SPOT.Hardware;

namespace NetduinoLab.Components.Behaviors
{
    public class PulseBehavior : DigitalComponentBehavior, IDisposable
    {
        private Thread pulseThread;
        private ManualResetEvent manualResetEvent;
        private bool isDisposed;

        #region Constructors

        public PulseBehavior()
        {
            this.manualResetEvent = new ManualResetEvent(true);
            this.isDisposed = false;
        }

        #endregion

        #region Public Properties

        public int IntervalInMilliseconds { get; set; }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            //Do we need to check if IsAlive here?
            if (this.pulseThread != null && this.pulseThread.IsAlive)
            {
                this.manualResetEvent.Set();
                this.isDisposed = true;
                this.pulseThread.Join(1000);
            }

            base.SyncState(false);
        }

        #endregion

        #region DigitalComponentBehavior Members

        public override bool State { get; protected set; }

        public override void SetState(bool state)
        {
            if (this.State == state)
                return;

            this.State = state;

            if (this.IntervalInMilliseconds > 0)
            {
                if (state)
                {
                    this.manualResetEvent.Set();

                    if (this.pulseThread == null || !this.pulseThread.IsAlive)
                    {
                        this.pulseThread = new Thread(new ThreadStart(this.pulse));
                        this.pulseThread.Start();
                    }
                }
                else
                {
                    //Tell thread to block until further notice.
                    this.manualResetEvent.Reset();

                    base.SyncState(false);
                }
            }
            else
                base.SyncState(state);
        }

        #endregion

        #region DigitalComponentCollection Members

        public override void SyncState(bool state)
        {
            this.SetState(false);
        }

        #endregion

        #region Private Methods

        private void pulse()
        {
            while (!this.isDisposed)
            {
                this.State = !this.State;

                base.SyncState(this.State);

                //Is an interval from the start of one ON state to the start of the next? 
                Thread.Sleep(this.IntervalInMilliseconds / 2);

                this.manualResetEvent.WaitOne();
            }
        }

        #endregion
    }
}
