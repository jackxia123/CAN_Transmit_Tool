using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DBCHandling
{
    [DataContract]
    public class DbcMessage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        private int cycleTime;
        private int dlc;
        private ObservableCollection<String> _transmitters = new ObservableCollection<string>();
        private ObservableCollection<String> _receivers = new ObservableCollection<string>();
        private Decimal id;
        private string name;
        private ObservableCollection<DbcSignal> signals = new ObservableCollection<DbcSignal>();
        private MsgSendType _sendtype;
        private DbcSignal _multiplexer;

        [DataMember]
        public Decimal ID
        {
            get { return id; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("id <=0 not allowed");
                }
                id = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.ID)));
            }
        }
        [DataMember]
        public string Name
        {
            get { return name; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentOutOfRangeException("Name should not be empty");
                }

                name = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Name)));
            }
        }
        [DataMember]
        public int CycleTime
        {
            get { return cycleTime; }
            set
            {
                if ((value < 0) && (_sendtype == 0))
                {
                    throw new ArgumentOutOfRangeException("Cycle Time <=0 not allowed for periodic message");
                }
                cycleTime = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.CycleTime)));
            }
        }
        [DataMember]
        public int DLC
        {
            get { return dlc; }
            set
            {
                //if (value <= 0 || value>8)
                //{
                //    throw new ArgumentOutOfRangeException("DLC value should be 1-8");
                //}

                dlc = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.DLC)));
            }
        }
        [DataMember]
        public ObservableCollection<string> Transmitters
        {
            get { return _transmitters; }
            set
            {
                _transmitters = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Transmitters)));
            }
        }

        [DataMember]
        public ObservableCollection<string> Receivers
        {
            get { return _receivers; }
            set
            {
                _receivers = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Receivers)));
            }
        }
        [DataMember]
        public ObservableCollection<DbcSignal> Signals
        {
            get { return signals; }
            set
            {
                signals = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Signals)));
            }


        }
        [DataMember]
        public DbcSignal Multiplexer
        {
            get { return _multiplexer; }
            set
            {
                _multiplexer = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Multiplexer)));
            }


        }
        [DataMember]
        public MsgSendType SendType
        {
            get { return _sendtype; }
            set
            {
                _sendtype = value;

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.SendType)));
            }

        }

    }

    public enum MsgSendType
    {
        Periodic = 0,
        Event = 1,
        Mixed = 2,
        Cyclic = 3,
        cyclicAndEvent = 4,
        cyclicAndSpontaneousWithDelay = 5,
        spontaneousWithRepetition = 6,
        cyclicIfActiveAndSpontaneousWD = 7,
        NoMsgSendType = 8,
    }
}
