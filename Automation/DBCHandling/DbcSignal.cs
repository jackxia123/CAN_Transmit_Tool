using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DBCHandling
{
    [DataContract]
    public class DbcSignal : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        private int _startbit;
        private int _length;
        private ObservableCollection<string> _transmitter = new ObservableCollection<string>();
        private ObservableCollection<string> _receiver = new ObservableCollection<string>();
        private string _name;
        private double _factor;
        private double _offset;
        private string _unit;
        private ObservableCollection<KeyValuePair<double, string>> _coding = new ObservableCollection<KeyValuePair<double, string>>();
        private string _byteorder;
        private string _valuetype;
        private double _minimum;
        private double _maximum;
        private double _initvalue;
        private bool _isextvaltype;
        private bool _ismultiplexor;
        private string _multiplex_value;
        private decimal _inmsgid;

        public DbcSignal()
        {

        }
        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentOutOfRangeException("Name should not be empty");
                }

                _name = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Name)));
            }
        }

        public bool IsMultiPlexor
        {
            get { return _ismultiplexor; }
            set
            {
                _ismultiplexor = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.IsMultiPlexor)));
                throw new NotImplementedException("Multiplex is not supported");

            }


        }

        public string MultiPlex_value
        {
            get { return _multiplex_value; }
            set
            {
                _multiplex_value = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.MultiPlex_value)));
                throw new NotImplementedException("Multiplex is not supported");

            }


        }
        [DataMember]
        public int StartBit
        {
            get { return _startbit; }
            set
            {
                if (value < 0 || value > 63)
                {
                    throw new ArgumentOutOfRangeException("Startbit <=0 not allowed");
                }
                _startbit = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.StartBit)));
            }
        }
        [DataMember]
        public int Length
        {
            get { return _length; }
            set
            {
                if (value <= 0 || value > 64)
                {
                    throw new ArgumentOutOfRangeException("Length value should be 0-64");
                }

                _length = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Length)));
            }
        }
        [DataMember]
        public double Factor
        {
            get { return _factor; }
            set
            {
                _factor = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Factor)));
            }
        }
        [DataMember]
        public double Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Offset)));
            }
        }
        [DataMember]
        public string Unit
        {
            get { return _unit; }
            set
            {
                _unit = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Unit)));
            }
        }

        [DataMember]
        public ObservableCollection<KeyValuePair<double, string>> Coding
        {
            get { return _coding; }
            set
            {
                _coding = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Coding)));
            }
        }
        [DataMember]
        public string ByteOrder
        {
            get { return _byteorder; }
            set
            {
                _byteorder = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.ByteOrder)));
            }
        }
        [DataMember]
        public string ValueType
        {
            get { return _valuetype; }
            set
            {
                _valuetype = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.ValueType)));
            }
        }
        [DataMember]
        public bool IsExtValType
        {
            get { return _isextvaltype; }
            set
            {
                _isextvaltype = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.IsExtValType)));
            }
        }

        [DataMember]
        public double Maximum
        {
            get { return _maximum; }
            set
            {
                _maximum = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Maximum)));
            }
        }
        [DataMember]
        public double Minimum
        {
            get { return _minimum; }
            set
            {
                _minimum = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Minimum)));
            }
        }
        [DataMember]
        public double InitValue
        {
            get { return _initvalue; }
            set
            {
                _initvalue = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.InitValue)));
            }
        }

        [DataMember]
        public ObservableCollection<string> Transmitters
        {
            get { return _transmitter; }
            set
            {
                _transmitter = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Transmitters)));
            }
        }

        [DataMember]
        public ObservableCollection<string> Receivers
        {
            get { return _receiver; }
            set
            {
                _receiver = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Receivers)));
            }
        }
        [DataMember]
        public decimal InMessage
        {
            get { return _inmsgid; }
            set
            {
                _inmsgid = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.InMessage)));
            }
        }

    }
}
