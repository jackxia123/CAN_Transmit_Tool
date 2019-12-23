using RBT.Universal.CanEvalParameters;
using RBT.Universal.Keywords;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RBT.Universal
{
    [DataContract]
    [KnownType(typeof(Trigger))]
    [KnownType(typeof(AndTrigger))]
    public class CanTraceAnalyser:Model,ICloneable
    {
        private ListDependentParameter _defaultSigList = new ListDependentParameter("### default_signal_list", @"This is the signal list that will be used by default if no signal list is men-tioned in the parameter set. The default signal list should be written as shown in the example below. The signal will be printed in the same order in which they appear in the default signal list. This should come at the beginning before the parameter sets start. The names used must be same as in the CAN mapping file. This parameter is mandatory
  e.g.
### default_signal_list                    = @('DR0','DR1','DR2','B_ABS')
", new ObservableCollection<string>(), null);


        private MeasurementPoint _mep;
        private ObservableCollection<TriggerBase> _triggers= new ObservableCollection<TriggerBase> ();
        private ObservableCollection<DeltaTime> _deltaTimes = new ObservableCollection<DeltaTime> ();

        OperationStatePattern _operationStatePattern;
        OperationStatePatternStartTime _operationStatePatternStartTime;
        OperationStatePatternStopTime _operationStatePatternStopTime;

        SigPatternAnalyser _sigPatternAnalyser;
        CycleTimeCheck _cycleTimeCheck;
        FirstFrameCheck _firstFrameCheck;
        FirstFrameCheckAllMsg _firstFrameCheckAllMsg;
        LastFrameCheck _lastFrameCheck;
        MessageCounterCheck _messageCounterCheck;
        MessageChecksumCheck _messageChecksumCheck;
        SignalCountupCalculate _signalCountupCalculate;

        ObservableCollection<ICanEvalParameter> _allParameters;
        string _evalScript;

        public CanTraceAnalyser() { }

        [DataMember]
        public ListDependentParameter DefaultSignalList
        {
            get
            {
                return _defaultSigList;
            }

            set
            {
                _defaultSigList = value;
                SetProperty(ref _defaultSigList, value);

            }
        }
        [DataMember]
        public MeasurementPoint MeasurementPoint
        {
            get
            {
                return _mep; 
            }

            set
            {
                _mep = value;
                SetProperty(ref _mep, value);

            }
        }
        [DataMember]
        public ObservableCollection<TriggerBase> Triggers
        {
            get
            {
                return _triggers;
            }

            set
            {
                _triggers = value;
                SetProperty(ref _triggers, value);

            }
        }

        [DataMember]
        public ObservableCollection<DeltaTime> DeltaTimes
        {
            get
            {
                return _deltaTimes;
            }

            set
            {
                _deltaTimes = value;
                SetProperty(ref _deltaTimes, value);

            }
        }
        [DataMember]
        public OperationStatePattern OperationStatePattern
        {
            get
            {
                return _operationStatePattern;
            }

            set
            {
                _operationStatePattern = value;
                SetProperty(ref _operationStatePattern, value);

            }
        }
        [DataMember]
        public OperationStatePatternStartTime OperationStatePatternStartTime
        {
            get
            {
                return _operationStatePatternStartTime;
            }

            set
            {
                _operationStatePatternStartTime = value;
                SetProperty(ref _operationStatePatternStartTime, value);

            }
        }
        [DataMember]
        public OperationStatePatternStopTime OperationStatePatternStopTime
        {
            get
            {
                return _operationStatePatternStopTime;
            }

            set
            {
                _operationStatePatternStopTime = value;
                SetProperty(ref _operationStatePatternStopTime, value);

            }
        }


        [DataMember]
        public SigPatternAnalyser SigPatternAnalyser
        {
            get
            {
                return _sigPatternAnalyser;
            }

            set
            {
                _sigPatternAnalyser = value;
                SetProperty(ref _sigPatternAnalyser, value);

            }
        }
        [DataMember]
        public CycleTimeCheck CycleTimeCheck
        {
            get
            {
                return _cycleTimeCheck;
            }

            set
            {
                _cycleTimeCheck = value;
                SetProperty(ref _cycleTimeCheck, value);

            }
        }
        [DataMember]
        public FirstFrameCheck FirstFrameCheck
        {
            get
            {
                return _firstFrameCheck;
            }

            set
            {
                _firstFrameCheck = value;
                SetProperty(ref _firstFrameCheck, value);

            }
        }
        [DataMember]
        public FirstFrameCheckAllMsg FirstFrameCheckAllMsg
        {
            get
            {
                return _firstFrameCheckAllMsg;
            }

            set
            {
                _firstFrameCheckAllMsg = value;
                SetProperty(ref _firstFrameCheckAllMsg, value);

            }
        }
        [DataMember]
        public LastFrameCheck LastFrameCheck
        {
            get
            {
                return _lastFrameCheck;
            }

            set
            {
                _lastFrameCheck = value;
                SetProperty(ref _lastFrameCheck, value);

            }
        }
        [DataMember]
        public MessageChecksumCheck MessageChecksumCheck
        {
            get
            {
                return _messageChecksumCheck;
            }

            set
            {
                _messageChecksumCheck = value;
                SetProperty(ref _messageChecksumCheck, value);

            }
        }
        [DataMember]
        public MessageCounterCheck MessageCounterCheck
        {
            get
            {
                return _messageCounterCheck;
            }

            set
            {
                _messageCounterCheck = value;
                SetProperty(ref _messageCounterCheck, value);

            }
        }
        [DataMember]
        public SignalCountupCalculate SignalCountupCalculate
        {
            get
            {
                return _signalCountupCalculate;
            }

            set
            {
                _signalCountupCalculate = value;
                SetProperty(ref _signalCountupCalculate, value);

            }
        }

        [XmlIgnore]
        public ObservableCollection<ICanEvalParameter> AllParameters
        {
            get
            {
                _allParameters = new ObservableCollection<ICanEvalParameter>() {this.CycleTimeCheck,this.FirstFrameCheck,this.FirstFrameCheckAllMsg,this.LastFrameCheck,this.MessageChecksumCheck,this.MessageCounterCheck,
                   this.OperationStatePattern,this.OperationStatePatternStartTime,this.OperationStatePatternStopTime,this.SignalCountupCalculate,this.SigPatternAnalyser,
               this.MeasurementPoint } ;

                foreach (ICanEvalParameter trg in this.Triggers)
                {
                    _allParameters.Add(trg);

                }

                foreach (ICanEvalParameter delta in this.DeltaTimes)
                {
                    _allParameters.Add(delta);

                }

                return _allParameters;

            }
            
            set
            {
                _allParameters = value;
                SetProperty(ref _allParameters, value);

            }

        }
        [DataMember]
        public string EvalScript
        {
            get
            {
                string _evalScript = "#CanTraceAnalyse";

                foreach (ICanEvalParameter par in AllParameters)
                {
                    if (par == null) continue;
                    _evalScript = _evalScript + "\n" + par.EvalParameter.Name + TestScript.SpacesTidy(par.EvalParameter.Name)+" = "+par.EvalParameter.Value;
                    if (par is MeasurementPoint)
                    {
                        _evalScript = _evalScript + "\n" + ((MeasurementPoint)par).SignalList.Name + TestScript.SpacesTidy(((MeasurementPoint)par).SignalList.Name) + " = " + ((MeasurementPoint)par).SignalList.Value;
                        _evalScript = _evalScript + "\n" + ((MeasurementPoint)par).CanOpState.Name + TestScript.SpacesTidy(((MeasurementPoint)par).CanOpState.Name) + " = " + ((MeasurementPoint)par).CanOpState.Value;
                    }

                }
                return _evalScript;

            }

            private set
            {
                _evalScript = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("EvalScript"));

            }
        }        



        public bool ParametersValidated()
        {
            bool paraValidFlag = true;
            foreach (ICanEvalParameter par in AllParameters)
            {
                if (par == null) continue;
                if (par.ArgumentsValidated() == false)
                {
                    paraValidFlag= false;
                    throw new Exception(string.Format("CAN Evaluation Parameter {0} is not set correctly",par.EvalParameter.Name));
                }

            }
            
            //Check  andtrigger reference 
            bool triggerValidFlag = CheckAndTriggerReference();

            //Check delta time reference
            bool deltaValidFlag = CheckDeltaReference();


            //Check signal list availability based on the default signal list provided or not
            bool siglistValidFlag = true;
            //Generate into the heading line of par file the default_signal_list, no need to check in here
            //if (DefaultSignalList.ValueList.Count == 0)
            //{

            //    siglistValidFlag = false;
            //    throw new Exception("Default signal list must be provided!");
            //}

            //Checks for measurement point CanOpState, assign dummy if not provided
            if (MeasurementPoint!=null && MeasurementPoint.CanOpState.ValueList.Count == 0)
            {
                MeasurementPoint.CanOpState.ValueList = new ObservableCollection<string>() { "dummy" };

            }

            //
            if (paraValidFlag && triggerValidFlag && deltaValidFlag && siglistValidFlag)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        private bool CheckAndTriggerReference()
        {

            foreach (AndTrigger trg in this.Triggers.ToList().FindAll(e => e is AndTrigger))
            {
                //check andtrigger

                if (this.Triggers.SingleOrDefault<TriggerBase>(p => p.TriggerName == trg.TriggerA.TriggerName) == null)
                {
                    return false;
                    throw new Exception(string.Format("AndTrigger{0}'s triggerA is not existing ", trg.TriggerID));
                }

                if (this.Triggers.SingleOrDefault<TriggerBase>(p => p.TriggerName == trg.TriggerB.TriggerName) == null)
                {
                    return false;
                    throw new Exception(string.Format("AndTrigger{0}'s triggerB is not existing ", trg.TriggerID));
                }


            }


            return true;

        }

        private bool CheckDeltaReference()
        {
            foreach (DeltaTime dt in this.DeltaTimes)
            {

               
                if (this.Triggers.SingleOrDefault<TriggerBase>(p => p.TriggerName == dt.TriggerA.TriggerName) == null)
                {
                    return false;
                    throw new Exception(string.Format("DeltaTime{0}'s triggerA is not existing ", dt.DeltaID));
                }

                if (this.Triggers.SingleOrDefault<TriggerBase>(p => p.TriggerName == dt.TriggerB.TriggerName) == null)
                {
                    return false;
                    throw new Exception(string.Format("DeltaTime{0}'s triggerB is not existing ", dt.DeltaID));
                }


            }

            return true;
        }
        /// <summary>
        /// DataContractSerializer deep clone
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {

            CanTraceAnalyser canTraceAnalyser;

            MemoryStream memoryStream = new MemoryStream();
            DataContractSerializer formatter = new DataContractSerializer(typeof(CanTraceAnalyser));
            formatter.WriteObject(memoryStream, this);
            memoryStream.Position = 0;
            canTraceAnalyser = (CanTraceAnalyser)formatter.ReadObject(memoryStream);

            return canTraceAnalyser;
        }
    }
}
