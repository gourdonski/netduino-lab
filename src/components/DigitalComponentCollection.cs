using System;
using Microsoft.SPOT;
using System.Collections;
using NetduinoLab.Components.Abstract;

namespace NetduinoLab.Components
{
    //Oh for a lack of generics.
    public class DigitalComponentCollection : IEnumerable
    {
        //We make this private because the Micro Framework currently doesn't support
        //generic lists and we want to make sure that only IDigitalComponents are getting
        //handed in. 
        private ArrayList digitalComponents;

        public IDigitalComponent this[int index]
        {
            get
            {
                return (IDigitalComponent)this.digitalComponents[index];
            }
            protected set
            {
                this.digitalComponents[index] = value;
            }
        }

        #region Constructors

        public DigitalComponentCollection()
        {
            this.digitalComponents = new ArrayList();
        }

        #endregion

        #region Public Properties

        public int Count 
        { 
            get 
            { 
                return this.digitalComponents.Count; 
            } 
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            //Can't use foreach here or we'll get a CLR_E_FAIL compile error.
            for (int i = 0; i < this.digitalComponents.Count; i++)
            {
                var digitalComponent = (IDigitalComponent)this.digitalComponents[i];

                if (digitalComponent == null)
                    break;

                yield return digitalComponent;
            }
        }

        #endregion

        #region Public Methods

        public void Add(IDigitalComponent digitalComponent)
        {
            this.digitalComponents.Add(digitalComponent);
        }

        public void Remove(IDigitalComponent digitalComponent)
        {
            digitalComponent.SetState(false);
            
            this.digitalComponents.Remove(digitalComponent);
        }

        public bool Contains(IDigitalComponent digitalComponent)
        {
            return this.digitalComponents.Contains(digitalComponent);
        }

        public void Clear()
        {
            this.digitalComponents.Clear();
        }

        public virtual void SyncState(bool state)
        {
            foreach (IDigitalComponent digitalComponent in this)
                digitalComponent.SetState(state);
        }

        #endregion
    }
}
