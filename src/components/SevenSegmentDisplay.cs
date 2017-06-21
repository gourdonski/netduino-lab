using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Collections;
using NetduinoLab.Components.Behaviors;
using System.Threading;
using NetduinoLab.Components.Abstract;
using NetduinoLab.Web;
using NetduinoLab.Web.Abstract;
using NetduinoLab.Common;

namespace NetduinoLab.Components
{
    public class SevenSegmentDisplay : 
        IDigitalComponent, IDynamicDigitalComponent, IHttpComponent
    {
        #region Private Fields

        private DigitalComponentBehavior behavior;
        private DigitalComponentCollection segments;
        private IDigitalComponent decimalPoint;
        private bool[] digitZeroMask = new bool[] { true, true, true, true, true, true, false };
        private bool[] digitOneMask = new bool[] { false, true, true, false, false, false, false };
        private bool[] digitTwoMask = new bool[] { true, true, false, true, true, false, true };
        private bool[] digitThreeMask = new bool[] { true, true, true, true, false, false, true };
        private bool[] digitFourMask = new bool[] { false, true, true, false, false, true, true };
        private bool[] digitFiveMask = new bool[] { true, false, true, true, false, true, true };
        private bool[] digitSixMask = new bool[] { true, false, true, true, true, true, true };
        private bool[] digitSevenMask = new bool[] { true, true, true, false, false, false, false };
        private bool[] digitEightMask = new bool[] { true, true, true, true, true, true, true };
        private bool[] digitNineMask = new bool[] { true, true, true, true, false, true, true };
        private bool[] digitNullMask = new bool[] { false, false, false, false, false, false, false };
        private bool[] digitErrorMask = new bool[] { true, false, false, true, true, true, true };

        #endregion

        #region Constructors

        public SevenSegmentDisplay(Cpu.Pin segmentAPin, Cpu.Pin segmentBPin,
            Cpu.Pin segmentCPin, Cpu.Pin segmentDPin, Cpu.Pin segmentEPin,
            Cpu.Pin segmentFPin, Cpu.Pin segmentGPin, Cpu.Pin decimalPin)
        {
            this.segments = new DigitalComponentCollection
            {
                new DigitalComponent(segmentAPin),
                new DigitalComponent(segmentBPin),
                new DigitalComponent(segmentCPin),
                new DigitalComponent(segmentDPin),
                new DigitalComponent(segmentEPin),
                new DigitalComponent(segmentFPin),
                new DigitalComponent(segmentGPin)
            };
            this.decimalPoint = new DigitalComponent(decimalPin);

            //In case the pins are aleady in the on state.
            this.ClearDisplay();
        }

        #endregion

        #region Public Properties

        public Digit Digit { get; private set; }

        public bool DecimalPoint { get; private set; }

        #endregion

        #region IDigitalComponent Members

        public bool State { get; private set; }

        public void SetState(bool state)
        {
            this.State = state;

            if (!state)
            {
                if (this.behavior == null)
                {
                    this.segments.SyncState(false);

                    this.decimalPoint.SetState(false);
                }
                else
                    this.behavior.SetState(false);
            }
            //Restore previous state.  
            else
            {
                this.SetDigit(this.Digit, this.DecimalPoint);
            }
        }

        #endregion

        #region IDynamicDigitalComponent Members

        public Type BehaviorType { get; private set; }

        //Haven't fully tested this.
        public void SetBehavior(DigitalComponentBehavior behavior)
        {
            this.BehaviorType = behavior.GetType();

            this.behavior = behavior;

            foreach (IDigitalComponent segment in this.segments)
                if (segment.State)
                    this.behavior.Add(segment);

            if (this.DecimalPoint)
                this.behavior.Add(this.decimalPoint);

            this.behavior.SetState(this.State);
        }

        public void ClearBehavior()
        {
            this.BehaviorType = null;

            this.behavior.Clear();
            this.behavior = null;

            this.SetDigit(this.Digit, this.DecimalPoint);
        }

        #endregion

        #region IHttpComponent Members

        //Need to implement this.
        public void Poll(HttpRequestContext requestContext, IHttpResponse response)
        {
            throw new NotImplementedException();
        }

        public void Command(HttpRequestContext requestContext, IHttpResponse response)
        {
            foreach (Parameter parameter in requestContext.Parameters)
                //Avoid hardcoding the parameter name here?
                if (parameter.Key.ToLower().Trim() == "digit")
                    switch (((string)parameter.Value).ToLower().Trim())
                    {
                        case "0":
                        case "zero":
                            this.SetDigit(Digit.Zero);

                            break;
                        case "1":
                        case "one":
                            this.SetDigit(Digit.One);

                            break;
                        case "2":
                        case "two":
                            this.SetDigit(Digit.Two);

                            break;
                        case "3":
                        case "three":
                            this.SetDigit(Digit.Three);

                            break;
                        case "4":
                        case "four":
                            this.SetDigit(Digit.Four);

                            break;
                        case "5":
                        case "five":
                            this.SetDigit(Digit.Five);

                            break;
                        case "6":
                        case "six":
                            this.SetDigit(Digit.Six);

                            break;
                        case "7":
                        case "seven":
                            this.SetDigit(Digit.Seven);

                            break;
                        case "8":
                        case "eight":
                            this.SetDigit(Digit.Eight);

                            break;
                        case "9":
                        case "nine":
                            this.SetDigit(Digit.Nine);

                            break;
                        case "null":
                            this.SetDigit(Digit.Null);

                            break;
                        default:
                            this.SetDigit(Digit.Error);

                            break;
                    }
        }

        #endregion

        #region Public Methods

        public void ClearDisplay()
        {
            this.SetDigit(Digit.Null, false);
        }

        public void SetDigit(Digit digit, bool hasDecimal = false)
        {
            bool[] digitMask = null;

            switch (digit)
            {
                case Digit.Zero:
                    digitMask = this.digitZeroMask;

                    break;
                case Digit.One:
                    digitMask = this.digitOneMask;

                    break;
                case Digit.Two:
                    digitMask = this.digitTwoMask;

                    break;
                case Digit.Three:
                    digitMask = this.digitThreeMask;

                    break;
                case Digit.Four:
                    digitMask = this.digitFourMask;

                    break;
                case Digit.Five:
                    digitMask = this.digitFiveMask;

                    break;
                case Digit.Six:
                    digitMask = this.digitSixMask;

                    break;
                case Digit.Seven:
                    digitMask = this.digitSevenMask;

                    break;
                case Digit.Eight:
                    digitMask = this.digitEightMask;

                    break;
                case Digit.Nine:
                    digitMask = this.digitNineMask;

                    break;
                case Digit.Null:
                    digitMask = this.digitNullMask;

                    break;
                case Digit.Error:
                    digitMask = this.digitErrorMask;

                    break;
                default:
                    digitMask = this.digitErrorMask;

                    break;
            }

            if (this.behavior == null)
            {
                //A little brittle?
                for (int i = 0; i < this.segments.Count; i++)
                    ((IDigitalComponent)this.segments[i]).SetState(digitMask[i]);

                this.decimalPoint.SetState(hasDecimal);
            }
            else
            {
                this.behavior.Clear();

                for (int i = 0; i < this.segments.Count; i++)
                {
                    var segment = (IDigitalComponent)this.segments[i];
                    bool state = digitMask[i];

                    if (state)
                        this.behavior.Add(segment);
                    else
                        segment.SetState(false);
                }

                if (hasDecimal)
                    this.behavior.Add(this.decimalPoint);
                else
                    this.decimalPoint.SetState(false);

                this.behavior.SetState(true);
            }

            this.Digit = digit;
            this.DecimalPoint = hasDecimal;
        }

        #endregion     
    }
}
