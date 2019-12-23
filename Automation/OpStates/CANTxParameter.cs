using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;

namespace OpStates
{
    [DataContract]
    public class CANTxParameter : INotifyPropertyChanged, ICloneable
    {
        private string _value;

        private string _name;
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public CANTxParameter() { }
        public CANTxParameter(string name, string desc, string value, Type valueType)
        {
            Name = name;
            Description = desc;
            Value = value;
            ValueType = valueType;

        }
        //[DataMember]
        //public string Name { get; set; }

        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {

                _name = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Name)));
            }

        }

        [DataMember]
        public string Description { get; private set; }

        public Type ValueType { get; private set; }
        [DataMember]
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Value)));
            }
        }
        /// <summary>
        ///  DataContractSerializer realize deep clone
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            CANTxParameter txParameter;

            MemoryStream memoryStream = new MemoryStream();
            DataContractSerializer formatter = new DataContractSerializer(typeof(CANTxParameter));
            formatter.WriteObject(memoryStream, this);
            memoryStream.Position = 0;
            txParameter = (CANTxParameter)formatter.ReadObject(memoryStream);

            return txParameter;
        }
    }
}
