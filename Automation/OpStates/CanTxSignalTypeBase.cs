using RBT.Universal;
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
    public class CanTxSignalType : INotifyPropertyChanged
    {
        private string _expValue;
        private string _comment;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            //simplify null-conditional
            PropertyChanged?.Invoke(this, e);
        }

        protected ObservableCollection<CANTxParameter> _parameters = new ObservableCollection<CANTxParameter>();

        [DataMember]
        public virtual string Name { get { return "Base Signal Type"; } set { } }
        public virtual ObservableCollection<OpState> ProposedOpStates { get; set; }
        //public abstract string GetLogicalResult<T>(T opstate) where T : OpState;
        [DataMember]
        public virtual ObservableCollection<CANTxParameter> Parameters
        {
            get { return _parameters; }
            set
            {
                _parameters = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Parameters)));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.CanTraceAnalyser)));
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

        public virtual string ExpectedValue { get { return _expValue; } set { _expValue = value; OnPropertyChanged(new PropertyChangedEventArgs("ExpectedValue")); } }
        //to indicator the selection status
        //public virtual bool IsSelected { get; set; }
        [DataMember]
        public virtual string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Comment)));

            }
        }
        [DataMember]
        public virtual string Category { get; set; }
        [DataMember]
        public virtual string Description { get; set; }
        [DataMember]
        public virtual CanTraceAnalyser CanTraceAnalyser { get; set; }
        public virtual CanTraceAnalyser ParameterizeCanTraceAnalyser(ObservableCollection<CANTxParameter> paraSet) { return null; }


        private static Type[] GetKnownTypes()
        {
            List<Type> type = new List<Type>();
            Type[] allTypes = Assembly.GetAssembly(typeof(CanTxSignalType)).GetExportedTypes();
            foreach (Type t in allTypes)
            {
                if (t.IsSubclassOf(typeof(CanTxSignalType)))
                    type.Add(t);

            }
            return type.ToArray();
        }
    }
}
