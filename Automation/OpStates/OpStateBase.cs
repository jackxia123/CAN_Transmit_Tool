using RBT.Universal;
using RBT.Universal.CanEvalParameters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace OpStates
{
    [DataContract]
    [KnownType("GetKnownTypes")]
    public class OpState : INotifyPropertyChanged
    {
        protected static string opSigVal = "";

        public event PropertyChangedEventHandler PropertyChanged;
        protected ItemsChangeObservableCollection<CANTxParameter> _parameters = new ItemsChangeObservableCollection<CANTxParameter>();
        public ItemsChangeObservableCollection<CANTxParameter> _operationStateSignals = new ItemsChangeObservableCollection<CANTxParameter>();

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
        [DataMember]
        public virtual string Name { get; set; }
        public virtual ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals { get; set; }
        [DataMember]
        public virtual string Category { get; set; }
        [DataMember]
        public virtual string Description { get; set; }

        public virtual TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet,
            ref TestScript testScript)
        { return null; }

        public virtual void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet) { }
        [DataMember]
        public virtual ItemsChangeObservableCollection<CANTxParameter> Parameters
        {
            get { return _parameters; }
            set
            {
                _parameters = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Parameters)));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.OperationStateSignals)));
            }


        }

        public void AddParameters(CANTxParameter par)
        {
            if (Parameters.FirstOrDefault(x => x.Name == par.Name) == null)
            {
                Parameters.Add(par);

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Parameters)));

            }

        }
        private static Type[] GetKnownTypes()
        {
            List<Type> type = new List<Type>();
            Type[] allTypes = Assembly.GetAssembly(typeof(OpState)).GetExportedTypes();
            foreach (Type t in allTypes)
            {
                if (t.IsSubclassOf(typeof(OpState)))
                    type.Add(t);

            }
            return type.ToArray();
        }


        public void AddSig2OpState(string sigName, string sigValue)
        {
            OperationStateSignals.Add(new CANTxParameter(sigName, "", sigValue, typeof(string)));

            OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.OperationStateSignals)));
        }
        public void RemoveSig2OpState(CANTxParameter par)
        {
            OperationStateSignals.Remove(par);

            OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.OperationStateSignals)));
        }

        [DataMember]
        public ItemsChangeObservableCollection<CANTxParameter> OperationStateSignals
        {
            get
            {
                return _operationStateSignals;
            }
            set
            {
                _operationStateSignals = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.OperationStateSignals)));
            }
        }

        public virtual MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            return null;

        }

        protected void AddDefaultSignal2OpState(ObservableCollection<CANTxParameter> paraSet, string opName)
        {
            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + opName) != null)
            {
                if (OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value) == null)
                {

                    AddSig2OpState(paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value, paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + opName).Value);
                    OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + opName).Value;
                    opSigVal = paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + opName).Value;
                }
                else
                {
                    OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + opName).Value;
                }
            }

        }
    }
}