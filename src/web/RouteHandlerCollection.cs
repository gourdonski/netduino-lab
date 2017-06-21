using System;
using Microsoft.SPOT;
using System.Collections;

namespace NetduinoLab.Web
{
    //Is it a bad idea making everything in here virtual?
    public class RouteHandlerCollection : IEnumerable
    {
        private ArrayList routeHandlers;

        public virtual RouteHandler this[int index]
        {
            get
            {
                return (RouteHandler)this.routeHandlers[index];
            }
            protected set
            {
                this.routeHandlers[index] = value;
            }
        }

        #region Constructors

        public RouteHandlerCollection()
        {
            this.routeHandlers = new ArrayList();
        }

        #endregion

        #region Public Properties

        public virtual int Count
        {
            get
            {
                return this.routeHandlers.Count;
            }
        }

        #endregion

        #region IEnumerable Members

        public virtual IEnumerator GetEnumerator()
        {
            //Can't use foreach here or we'll get a CLR_E_FAIL compile error.
            for (int i = 0; i < this.routeHandlers.Count; i++)
            {
                var routeHandler = (RouteHandler)this.routeHandlers[i];

                if (routeHandler == null)
                    break;

                yield return routeHandler;
            }
        }

        #endregion

        #region Public Methods

        public virtual void Add(RouteHandler routeHandler)
        {
            this.routeHandlers.Add(routeHandler);
        }

        public virtual void Remove(RouteHandler routeHandler)
        {
            this.routeHandlers.Remove(routeHandler);
        }

        public virtual bool Contains(RouteHandler routeHandler)
        {
            return this.routeHandlers.Contains(routeHandler);
        }

        public virtual void Clear()
        {
            this.routeHandlers.Clear();
        }

        #endregion
    }
}
