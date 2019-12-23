using RBT.Universal.Keywords;
using RBT.Universal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace CANTxGenerator
{
    [DataContract]
    public class BusGeneric : Model
    {
        private bool _isNoFaultCanHUbattSh = false;
        private bool _isNoFaultCanLGndSh = false;
        private bool _isIncludeBusGeneric = false;
        private bool _IsNoFaultErrorPassive = false;
        private bool _IsNoFaultBusOffReset = false;
        private int _tDiagstartMs = 1500;
        private bool _isRegression = false;
        private double _thNUV = 7.5;
        private string _thHydrUV = "PATA";
        private double _thHydrHardUV = 0;
        private double _thOV = 0;
        private double _pataonFDRSignal = 0;
        private int _patanormalPataSignal = 0;
        private double _speedThreadholdadd1mps = 23;
        private double _speedThreadholdminus1mps = 23;
        private string _doorsLink = "";
        private string _productType = "CAN";
        private string _pataLampType = "CAN";


        private ObservableCollection<TestScript> _busGenericTScript = new ObservableCollection<TestScript>();


        public BusGeneric()
        {


        }
        [DataMember]
        public bool IsNoFaultCanHUbattSh
        {
            get { return _isNoFaultCanHUbattSh; }
            set { SetProperty(ref _isNoFaultCanHUbattSh, value); RaisePropertyChanged(nameof(BusGenericTScript)); }

        }

        [DataMember]
        public bool IsNoFaultCanLGndSh
        {
            get { return _isNoFaultCanLGndSh; }
            set { SetProperty(ref _isNoFaultCanLGndSh, value); RaisePropertyChanged(nameof(BusGenericTScript)); }

        }

        [DataMember]
        public bool IsIncludeBusGeneric
        {
            get { return _isIncludeBusGeneric; }
            set
            {
                SetProperty(ref _isIncludeBusGeneric, value);
                RaisePropertyChanged(nameof(BusGenericTScript));

            }

        }

        // Special Projects have special requirements
        [DataMember]
        public bool IsNoFaultErrorPassive
        {
            get
            {
                return _IsNoFaultErrorPassive;
            }
            set
            {
                SetProperty(ref _IsNoFaultErrorPassive, value);
                RaisePropertyChanged(nameof(BusGenericTScript));
            }
        }

        // Special Projects have special requirements
        [DataMember]
        public bool IsNoFaultBusOffReset
        {
            get
            {
                return _IsNoFaultBusOffReset;
            }
            set
            {
                SetProperty(ref _IsNoFaultBusOffReset, value);
                RaisePropertyChanged(nameof(BusGenericTScript));
            }
        }


        [DataMember]
        public int TdiagstartMs
        {
            get { return _tDiagstartMs; }
            set { SetProperty(ref _tDiagstartMs, value); RaisePropertyChanged(nameof(BusGenericTScript)); }

        }

        [DataMember]
        public bool IsRegression
        {
            get { return _isRegression; }
            set { SetProperty(ref _isRegression, value); }

        }
        [DataMember]
        public double ThresholdNUV
        {
            get { return _thNUV; }
            set
            {
                SetProperty(ref _thNUV, value); RaisePropertyChanged(nameof(BusGenericTScript));
            }

        }
        [DataMember]
        public string ThresholdHydrUV
        {
            get { return _thHydrUV; }
            set { SetProperty(ref _thHydrUV, value); RaisePropertyChanged(nameof(BusGenericTScript)); }

        }
        [DataMember]
        public double ThresholdHydrHardUV
        {
            get { return _thHydrHardUV; }
            set { SetProperty(ref _thHydrHardUV, value); RaisePropertyChanged(nameof(BusGenericTScript)); }

        }
        [DataMember]
        public double ThresholdOV
        {
            get { return _thOV; }
            set { SetProperty(ref _thOV, value); RaisePropertyChanged(nameof(BusGenericTScript)); }

        }

        [DataMember]
        public double PataonFDRSignal
        {
            get { return _pataonFDRSignal; }
            set { SetProperty(ref _pataonFDRSignal, value); RaisePropertyChanged(nameof(BusGenericTScript)); }

        }

        [DataMember]
        public int PataNormalPataSignal
        {
            get { return _patanormalPataSignal; }
            set { SetProperty(ref _patanormalPataSignal, value); RaisePropertyChanged(nameof(BusGenericTScript)); }

        }

        [DataMember]
        public double SpeedThreadHoldAdd1mps
        {
            get { return _speedThreadholdadd1mps; }
            set { SetProperty(ref _speedThreadholdadd1mps, value); RaisePropertyChanged(nameof(BusGenericTScript)); }

        }

        [DataMember]
        public double SpeedThreadHoldMinus1mps
        {
            get { return _speedThreadholdminus1mps; }
            set { SetProperty(ref _speedThreadholdminus1mps, value); RaisePropertyChanged(nameof(BusGenericTScript)); }

        }

        [DataMember]
        public string DoorsLink
        {
            get { return _doorsLink; }
            set { SetProperty(ref _doorsLink, value); RaisePropertyChanged(nameof(BusGenericTScript)); }

        }

        [DataMember]
        public string ProductType
        {
            get { return _productType; }
            set { SetProperty(ref _productType, value); RaisePropertyChanged(nameof(BusGenericTScript)); }

        }

        //Jack
        [DataMember]
        public string PataLampType
        {
            get { return _pataLampType; }
            set { SetProperty(ref _pataLampType, value); RaisePropertyChanged(nameof(BusGenericTScript)); }

        }

        [DataMember]
        public ObservableCollection<TestScript> BusGenericTScript
        {

            get
            {
                _busGenericTScript.Clear();
                if (IsIncludeBusGeneric)
                {
                    if (IsRegression)
                    {
                        //Todo: define the regression test
                    }
                    else
                    {
                        //var temp = TcLibBusGeneric.GetTScriptUD(ProductType, PataLampType, IsNoFaultCanHUbattSh, ThresholdNUV, ThresholdHydrUV, ThresholdHydrHardUV, ThresholdOV, PataonFDRSignal, PataNormalPataSignal);
                        var temp = TcLibBusGeneric.GetTScriptUD(ProductType, PataLampType,ThresholdHydrUV,PataNormalPataSignal);
                        //_busGenericTScript = new ObservableCollection<TestScript>(
                        //    temp.Concat(TcLibBusGeneric.GetTScriptBPU(IsNoFaultCanHUbattSh, IsNoFaultCanLGndSh, IsNoFaultErrorPassive, IsNoFaultBusOffReset, TdiagstartMs, ProductType).Concat(
                        //        TcLibBusGeneric.GetTScriptController()).Concat(TcLibBusGeneric.GetTScriptVoltCondition(ThresholdNUV, ThresholdHydrHardUV, ThresholdOV, ProductType))));
                        _busGenericTScript = new ObservableCollection<TestScript>(temp);
                    }

                    foreach (var ts in _busGenericTScript)
                    {
                        ts.DoorsLink = DoorsLink;
                        ts.QcFolderPath = @"RBT_CAN\PATA_" + DateTime.Now.ToString("yyyy_MMM_ddd_HHmmss");
                    }
                }

                return _busGenericTScript;

            }
            set { SetProperty(ref _busGenericTScript, value); }

        }

    }
}
