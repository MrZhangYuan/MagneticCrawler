using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UltimatePresentation.Presentation
{
        [ComVisible(true)]
        public class DisposableObject : IDisposable
        {
                private EventHandler _disposing;
                public event EventHandler Disposing
                {
                        add
                        {
                                this.ThrowIfDisposed();
                                this._disposing = (EventHandler)Delegate.Combine(this._disposing, value);
                        }
                        remove
                        {
                                this.ThrowIfDisposed();
                                this._disposing = (EventHandler)Delegate.Remove(this._disposing, value);
                        }
                }
                public bool IsDisposed
                {
                        get;
                        private set;
                }
                ~DisposableObject()
                {
                        this.Dispose(false);
                }
                public void Dispose()
                {
                        this.Dispose(true);
                        GC.SuppressFinalize(this);
                }
                protected void ThrowIfDisposed()
                {
                        if (this.IsDisposed)
                        {
                                throw new ObjectDisposedException(base.GetType().Name);
                        }
                }
                protected void Dispose(bool disposing)
                {
                        if (!this.IsDisposed)
                        {
                                try
                                {
                                        if (this._disposing != null)
                                        {
                                                //this._disposing.RaiseEvent(this);
                                                this._disposing(this, EventArgs.Empty);
                                                this._disposing = null;
                                        }
                                        if (disposing)
                                        {
                                                this.DisposeManagedResources();
                                        }
                                        this.DisposeNativeResources();
                                }
                                finally
                                {
                                        this.IsDisposed = true;
                                }
                        }
                }
                protected virtual void DisposeManagedResources()
                {
                }
                protected virtual void DisposeNativeResources()
                {
                }
        }
}
