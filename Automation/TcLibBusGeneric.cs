using RBT.Universal;
using RBT.Universal.CanEvalParameters;
using RBT.Universal.CanEvalParameters.MeasurementPoints;
using RBT.Universal.Keywords;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CANTxGenerator
{
    static class TcLibBusGeneric
    {

        #region UD

        /// <summary>
        /// Get Test Script out of project config
        /// </summary>
        /// <returns></returns>
        ///
        private static TestScript GetTScriptPataOFFtoON(string producttype, string palalamp, string thresholdHydrUV, int patanormalpatasignal)
        {
            TestScript _script = new TestScript();
            _script.Name = @"RB_UNIVERSAL_01J.PATA_ON";
            _script.Purpose.ScalarValue = @"PATA function - PATA OFF -> ON";
            if (producttype.Trim().ToLower() == "hw")
            {
                _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

                new TraceStart(),
                new Wait(1000),
                new EcuOn(),
                new Wait(2550),
                new MM6Start("9","Normal"),
                 new Wait(5000),
                new SetModelValues(new Dictionary<string, string>{ {"PATA1", "1"} }),
                new Wait(2000),
                new UndoSetModelValues(),
                new Wait(2000),
                new ReadCanLamps(new ObservableCollection<string>() { "PATA_on" }),
                new ReadCanSignals(new ObservableCollection<string>() {"PATA_on"}),
                new Wait(4000),
               
                new MM6Stop(),
                new EcuOff(),
                new Wait(5000),
                new TraceStop(),

            });

                ObservableCollection<TriggerBase> tempTriggerCollection = new ObservableCollection<TriggerBase>();
                ObservableCollection<DeltaTime> tempDeltaCollection = new ObservableCollection<DeltaTime>();
                Trigger lcUzOn = new Trigger(new SigConditon("LCI_PATA", 1, TriggerConditionSignal.Equal)); //hardcode LC_Info signal
                tempTriggerCollection.Add(lcUzOn);

                var tempLamp = new Trigger(new SigConditon(thresholdHydrUV, patanormalpatasignal, TriggerConditionSignal.Equal));
                tempTriggerCollection.Add(tempLamp);
                tempDeltaCollection.Add(new DeltaTime(lcUzOn, tempLamp, 100 + "-" + 190));


                _script.CanTraceAnalyser = new CanTraceAnalyser()
                {
                    Triggers = tempTriggerCollection,
                    DeltaTimes = tempDeltaCollection,
                };

                _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
                _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
            }
            else
            {
                   _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

                new TraceStart(),
                new Wait(1000),
                new EcuOn(),
                new Wait(2550),
                new MM6Start("9","Normal"),
                 new Wait(5000),
                new SetModelValues(new Dictionary<string, string>{ {"PATA1", "1"} }),
                new Wait(2000),
                new UndoSetModelValues(),
                new Wait(2000),
                new ReadCanLamps(new ObservableCollection<string>() { "PATA_on" }),
                new ReadCanSignals(new ObservableCollection<string>() {"PATA_on"}),
                new Wait(4000),
               
                new MM6Stop(),
                new EcuOff(),
                new Wait(5000),
                new TraceStop(),

            });
                ObservableCollection<TriggerBase> tempTriggerCollection = new ObservableCollection<TriggerBase>();
                ObservableCollection<DeltaTime> tempDeltaCollection = new ObservableCollection<DeltaTime>();
                Trigger lcUzOn = new Trigger(new SigConditon("LCI_PATA", 1, TriggerConditionSignal.Equal)); //hardcode LC_Info signal
                tempTriggerCollection.Add(lcUzOn);

                var tempLamp = new Trigger(new SigConditon(thresholdHydrUV, patanormalpatasignal, TriggerConditionSignal.Equal));
                tempTriggerCollection.Add(tempLamp);
                tempDeltaCollection.Add(new DeltaTime(lcUzOn, tempLamp, 100 + "-" + 190));


                _script.CanTraceAnalyser = new CanTraceAnalyser()
                {
                    Triggers = tempTriggerCollection,
                    DeltaTimes = tempDeltaCollection,
                };

                _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
                _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
            }
            _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
            _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

            return _script;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="isNoFaultCanHshUbatt"></param>
        /// <param name="isNoFaultCanLshGnd"></param>
        /// <param name="productType"></param>
        /// <returns></returns>
        //public static ObservableCollection<TestScript> GetTScriptUD(string producttype, string palalamp, bool includePataspeedLimites, double netundervoltage, string patalampSignal, double pataonabssignalvalue, double pataonebvsignalvalue, double pataonfdrsignalvalue, double patanormalonpalasignalvalue, double speedthreadholdadd1mps = 23, double speedthreadholdminus1mps = 23)
        public static ObservableCollection<TestScript> GetTScriptUD(string producttype, string palalamp, string thresholdHydrUV, int patanormalpatasignal)
        {
            ObservableCollection<TestScript> _tsBusGenUD = new ObservableCollection<TestScript>();
            _tsBusGenUD.Add(GetTScriptPataOFFtoON(producttype, palalamp, thresholdHydrUV, patanormalpatasignal));

            //_tsBusGenUD.Add(GetTScriptCanHIntrpt(productType, isNoFaultErrorPassive));

            //_tsBusGenUD.Add(GetTScriptCanHSh2Gnd(productType, isNoFaultBusOffReset));

            //_tsBusGenUD.Add(GetTScriptCanHSh2Uz(includePataspeedLimites, productType));

            //_tsBusGenUD.Add(GetTScriptCanLIntrpt(productType, isNoFaultErrorPassive));

            //_tsBusGenUD.Add(GetTScriptCanLSh2Uz(productType, isNoFaultBusOffReset));

            //_tsBusGenUD.Add(GetTScriptCanLSh2Gnd(isNoFaultCanLshGnd, productType));

            //_tsBusGenUD.Add(GetTScriptCanHSh2CANL(productType, isNoFaultBusOffReset));

            return _tsBusGenUD;

        }
    }
}
#endregion UD

//    #region BPU
//    /// <summary>
//    /// CAN H interrupt
//    /// </summary>
//    /// <param name="Tdiagstart"></param>
//    /// <param name="productType"></param>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanHIntrptBPU(int Tdiagstart, string productType, bool isNofault)
//    {
//        TestScript _script = new TestScript();
//        _script.Name = @"RB_UNIVERSAL_01J.CanH_Interrupt_BPU";
//        _script.Purpose.ScalarValue = @"CAN line monitoring CAN_H interrupt before power up";

//        if (isNofault)
//        {
//            _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//            new TraceStart(),
//            new Wait(2000),
//            new DoLineManipulation(new ObservableCollection<string>() {"CAN1_High_Line"}),
//            new Wait(2500),
//            new EcuOn(),
//            new Wait(Tdiagstart+200), // add 200ms to let the fault happen
//            new ResetLineManipulation(),
//            new Wait(3000),

//            new ReadLamps(new ObservableCollection<string>() { "Full_system" }),
//            new ReadCanSignals(new ObservableCollection<string>() {"Full_system"}),

//            new Wait(2000),
//            new EcuOff(),
//            new TraceStop(),

//        });
//            // will Add Mandatory faults selection list-> todo
//            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
//            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
//        }
//        else
//        {
//            _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//            new TraceStart(),
//            new Wait(2000),
//            new DoLineManipulation(new ObservableCollection<string>() {"CAN1_High_Line"}),
//            new Wait(2500),
//            new EcuOn(),
//            new Wait(Tdiagstart+200), // add 200ms to let the fault happen
//            new ResetLineManipulation(),
//            new Wait(3000),

//            productType == "ABS"?new ReadLamps(new ObservableCollection<string>() { "Full_system" }):new ReadLamps(new ObservableCollection<string>() { "ESP_off" }),
//             productType == "ABS"? new ReadCanSignals(new ObservableCollection<string>() {"Full_system"}):new ReadCanSignals(new ObservableCollection<string>() {"ESP_off"}),
//            new Wait(2000),

//            new EcuOff(),
//            new TraceStop(),

//        });

//            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_ErrorPassivNetwork0" };
//            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_ErrorPassivNetwork0" };
//        }

//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;
//    }

//    /// <summary>
//    /// CANH interrupt before power up within Tdiag
//    /// </summary>
//    /// <param name="Tdiagstart"></param>
//    /// <returns> will remove
//    /// </returns>
//    private static TestScript GetTScriptCanHIntrptBPUNoFault(int Tdiagstart)
//    {
//        TestScript _script = new TestScript();
//        _script.Name = @"RB_UNIVERSAL_01J.CanH_Interrupt_BPU_NoFault";
//        _script.Purpose.ScalarValue = @"CAN line monitoring CAN_H interrupt before power up and recover within diag start time";
//        _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//            new TraceStart(),
//            new Wait(2000),  // add 200ms to let the fault happen
//            new DoLineManipulation(new ObservableCollection<string>() {"CAN1_High_Line"}),
//            new Wait(2000),
//            new EcuOn(),
//            new Wait(Tdiagstart - 200),  // recover in 200ms before fault happen
//            new ResetLineManipulation(),
//            new Wait(3000),

//            new ReadLamps(new ObservableCollection<string>() { "Full_system" }),
//            new ReadCanSignals(new ObservableCollection<string>() {"Full_system"}),
//            new Wait(2000),

//            new EcuOff(),
//            new TraceStop(),

//        });

//        _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
//        _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;
//    }

//    /// <summary>
//    /// CANH short to Uz
//    /// </summary>
//    /// <param name="isNoFault"></param>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanHSh2UzBPU(bool isNoFault, int Tdiag, string productType)
//    {

//        TestScript _script = new TestScript();
//        _script.Name = @"RB_UNIVERSAL_01J.CanH_Short_Ubatt_BPU";
//        _script.Purpose.ScalarValue = @"CAN line monitoring CAN_H short to Ubatt before power up";

//        if (isNoFault)
//        {

//            _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//            new TraceStart(),
//            new Wait(2000),
//            new DoLineManipulation(null,new ObservableCollection<string>() {"CAN1_High_Line","UBATT"}),
//            new Wait(2000),
//            new EcuOn(),
//            new Wait(Tdiag+200),
//            new ResetLineManipulation(),
//            new Wait(3000),

//            new ReadLamps(new ObservableCollection<string>() { "Full_system" }),
//            new ReadCanSignals(new ObservableCollection<string>() {"Full_system"}),
//            new Wait(2000),

//            new EcuOff(),
//            new TraceStop(),

//           });

//            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
//            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };



//        }
//        else
//        {
//            _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//             new TraceStart(),
//            new Wait(2000),
//            new DoLineManipulation(null,new ObservableCollection<string>() {"CAN1_High_Line","UBATT"}),
//            new Wait(2000),
//            new EcuOn(),
//            new Wait(Tdiag+200),
//            new ResetLineManipulation(),
//            new Wait(3000),

//            productType == "ABS"? new ReadLamps(new ObservableCollection<string>() { "Full_system" }):new ReadLamps(new ObservableCollection<string>() { "ESP_off" }),
//             productType == "ABS"?  new ReadCanSignals(new ObservableCollection<string>() {"Full_system"}):new ReadCanSignals(new ObservableCollection<string>() {"ESP_off"}),
//            new Wait(2000),

//            new EcuOff(),
//            new TraceStop(),

//        });

//            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_ErrorPassivNetwork0" };
//            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_ErrorPassivNetwork0" };


//        }

//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;


//    }

//    /// <summary>
//    /// CAN H short Uz
//    /// </summary>
//    /// <param name="Tdiag"></param>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanHSh2UzBPUNoFault(int Tdiag)
//    {

//        TestScript _script = new TestScript();
//        _script.Name = @"RB_UNIVERSAL_01J.CanH_Short_Ubatt_BPU_NoFault";
//        _script.Purpose.ScalarValue = @"CAN line monitoring CAN_H short to Ubatt before power up within Tdiag 200ms";


//        _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//        new TraceStart(),
//        new Wait(2000),
//        new DoLineManipulation(null,new ObservableCollection<string>() {"CAN1_High_Line","UBATT"}),
//        new Wait(2000),
//        new EcuOn(),
//        new Wait(Tdiag-200),
//        new ResetLineManipulation(),
//        new Wait(3000),

//        new ReadLamps(new ObservableCollection<string>() { "Full_system" }),
//        new ReadCanSignals(new ObservableCollection<string>() {"Full_system"}),
//        new Wait(2000),

//        new EcuOff(),
//        new TraceStop(),

//        });

//        _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
//        _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };




//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;


//    }

//    /// <summary>
//    /// CAN H short to GND
//    /// </summary>
//    /// <param name="Tdiag"></param>
//    /// <param name="productType"></param>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanHSh2GndBPU(int Tdiag, string productType, bool isNofault)
//    {
//        TestScript _script = new TestScript();
//        _script.Name = @"RB_UNIVERSAL_01J.CanH_Short_Gnd_BPU";
//        _script.Purpose.ScalarValue = @"CAN line monitoring CAN_H short to Gnd before power up";

//        if (isNofault)
//        {
//            _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//        new TraceStart(),
//        new Wait(2000),
//        new DoLineManipulation(null,new ObservableCollection<string>() {"CAN1_High_Line","GND"}),
//        new Wait(2000),
//        new EcuOn(),
//        new Wait(Tdiag+200),
//        new ResetLineManipulation(),
//        new Wait(3000),
//        new ReadLamps(new ObservableCollection<string>() { "Full_system" }),
//        new ReadCanSignals(new ObservableCollection<string>() {"Full_system"}),
//        new EcuOff(),
//        new TraceStop(),

//          });

//        }
//        else
//        {
//            _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//        new TraceStart(),
//        new Wait(2000),
//        new DoLineManipulation(null,new ObservableCollection<string>() {"CAN1_High_Line","GND"}),
//        new Wait(2000),
//        new EcuOn(),
//        new Wait(Tdiag+200),
//        new ResetLineManipulation(),
//        new Wait(3000),

//        productType == "ABS" ? new ReadLamps(new ObservableCollection<string>() { "Full_system" }):new ReadLamps(new ObservableCollection<string>() { "ESP_off" }),
//        productType == "ABS" ? new ReadCanSignals(new ObservableCollection<string>() {"Full_system"}):new ReadCanSignals(new ObservableCollection<string>() {"ESP_off"}),
//        new Wait(2000),

//        new EcuOff(),
//        new TraceStop(),

//           });


//        }

//        if (productType == "Gen93")
//        {
//            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "CANSM_NETWORK_0_E_BUSOFF" };
//            _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "CANSM_NETWORK_1_E_BUSOFF" };
//            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "CANSM_NETWORK_0_E_BUSOFF" };
//            _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "CANSM_NETWORK_1_E_BUSOFF" };
//        }
//        else
//        {
//            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_BusOffNetwork0" };
//            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_BusOffNetwork0" };
//        }

//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };

//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;

//    }

//    /// <summary>
//    /// CAN H short to GND
//    /// </summary>
//    /// <param name="Tdiag"></param>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanHSh2GndBPUNoFault(int Tdiag)
//    {
//        TestScript _script = new TestScript();
//        _script.Name = @"RB_UNIVERSAL_01J.CanH_Short_Gnd_BPU_NoFault";
//        _script.Purpose.ScalarValue = @"CAN line monitoring CAN_H short to Gnd before power up within Tdiag";


//        _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//        new TraceStart(),
//        new Wait(2000),
//        new DoLineManipulation(null,new ObservableCollection<string>() {"CAN1_High_Line","GND"}),
//        new Wait(2000),
//        new EcuOn(),
//        new Wait(Tdiag-200),
//        new ResetLineManipulation(),
//        new Wait(3000),

//        new ReadLamps(new ObservableCollection<string>() { "Full_system" }),
//        new ReadCanSignals(new ObservableCollection<string>() {"Full_system"}),
//        new Wait(2000),

//        new EcuOff(),
//        new TraceStop(),

//    });

//        _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
//        _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };


//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;

//    }

//    /// <summary>
//    /// CAN L interrupt
//    /// </summary>
//    /// <param name="Tdiag"></param>
//    /// <param name="productType"></param>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanLIntrptBPU(int Tdiag, string productType, bool isNofault)
//    {
//        TestScript _script = new TestScript();
//        _script.Name = @"RB_UNIVERSAL_01J.CanL_Interrupt_BPU";
//        _script.Purpose.ScalarValue = @"CAN line monitoring CAN_L interrupt before power up";
//        if (isNofault)
//        {
//            _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//            new TraceStart(),
//            new Wait(2000),

//            new DoLineManipulation(new ObservableCollection<string>() {"CAN1_Low_Line"}),
//            new Wait(2000),
//             new EcuOn(),
//             new Wait(Tdiag+ 200),
//            new ResetLineManipulation(),
//            new Wait(3000),
//            new ReadLamps(new ObservableCollection<string>() { "Full_system" }),
//            new ReadCanSignals(new ObservableCollection<string>() {"Full_system"}),
//            new Wait(2000),
//            new EcuOff(),
//            new TraceStop(),

//        });
//            //-> todo : will add messgaes timeout in future
//            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
//            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
//        }

//        else

//        {
//            _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//            new TraceStart(),
//            new Wait(2000),

//            new DoLineManipulation(new ObservableCollection<string>() {"CAN1_Low_Line"}),
//            new Wait(2000),
//             new EcuOn(),
//             new Wait(Tdiag+ 200),
//            new ResetLineManipulation(),
//            new Wait(3000),

//            productType == "ABS"? new ReadLamps(new ObservableCollection<string>() { "Full_system" }):new ReadLamps(new ObservableCollection<string>() { "ESP_off" }),
//            productType == "ABS"? new ReadCanSignals(new ObservableCollection<string>() {"Full_system"}): new ReadCanSignals(new ObservableCollection<string>() {"ESP_off"}),
//            new Wait(2000),

//            new EcuOff(),
//            new TraceStop(),

//        });

//            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_ErrorPassivNetwork0" };
//            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_ErrorPassivNetwork0" };
//        }
//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;
//    }

//    /// <summary>
//    /// CAN L Interrupt
//    /// </summary>
//    /// <param name="Tdiag"></param>
//    /// <returns>todo->MM6 will realize</returns>
//    private static TestScript GetTScriptCanLIntrptBPUNoFault(int Tdiag)
//    {
//        TestScript _script = new TestScript();
//        _script.Name = @"RB_UNIVERSAL_01J.CanL_Interrupt_BPU_NoFault";
//        _script.Purpose.ScalarValue = @"CAN line monitoring CAN_L interrupt before power up within Tdiag";
//        _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//            new TraceStart(),
//            new Wait(2000),

//            new DoLineManipulation(new ObservableCollection<string>() {"CAN1_Low_Line"}),
//            new Wait(2000),
//             new EcuOn(),
//             new Wait(Tdiag - 200),
//            new ResetLineManipulation(),
//            new Wait(3000),

//            new ReadLamps(new ObservableCollection<string>() { "Full_system" }),
//            new ReadCanSignals(new ObservableCollection<string>() {"Full_system"}),
//            new Wait(2000),

//            new EcuOff(),
//            new TraceStop(),

//        });

//        _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
//        _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;
//    }

//    /// <summary>
//    /// CANH short to Uz
//    /// </summary>
//    /// <param name="isNoFault"></param>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanLSh2GndBPU(bool isNoFault, int Tdiag, string productType)
//    {

//        TestScript _script = new TestScript();
//        _script.Name = @"RB_UNIVERSAL_01J.CanL_Short_Gnd_BPU";
//        _script.Purpose.ScalarValue = @"CAN line monitoring CAN_L short to Gnd before power up";

//        if (isNoFault)
//        {

//            _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//            new TraceStart(),
//            new Wait(2000),

//            new DoLineManipulation(null,new ObservableCollection<string>() { "CAN1_Low_Line", "GND"}),
//            new Wait(2000),
//            new EcuOn(),
//            new Wait(Tdiag +200),
//            new ResetLineManipulation(),
//            new Wait(3000),

//            new ReadLamps(new ObservableCollection<string>() { "Full_system" }),
//            new ReadCanSignals(new ObservableCollection<string>() {"Full_system"}),
//            new Wait(2000),

//            new EcuOff(),
//            new TraceStop(),

//           });

//            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
//            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };



//        }
//        else
//        {
//            _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//            new TraceStart(),
//            new Wait(2000),

//            new DoLineManipulation(null,new ObservableCollection<string>() { "CAN1_Low_Line", "GND"}),
//            new Wait(2000),
//            new EcuOn(),
//            new Wait(Tdiag +200),
//            new ResetLineManipulation(),
//            new Wait(3000),

//            productType == "ABS"? new ReadLamps(new ObservableCollection<string>() { "Full_system" }):new ReadLamps(new ObservableCollection<string>() { "ESP_off" }),
//            productType == "ABS"? new ReadCanSignals(new ObservableCollection<string>() {"Full_system"}):new ReadCanSignals(new ObservableCollection<string>() {"ESP_off"}),
//            new Wait(2000),

//            new EcuOff(),
//            new TraceStop(),

//        });

//            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_ErrorPassivNetwork0" };
//            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_ErrorPassivNetwork0" };


//        }

//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;


//    }

//    /// <summary>
//    /// CAN L short to GND
//    /// </summary>
//    /// <param name="Tdiag"></param>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanLSh2GndBPUNoFault(int Tdiag)
//    {

//        TestScript _script = new TestScript();
//        _script.Name = @"RB_UNIVERSAL_01J.CanL_Short_Gnd_BPU_NoFault";
//        _script.Purpose.ScalarValue = @"CAN line monitoring CAN_L short to Gnd before power up within Tdiag";
//        _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//            new TraceStart(),
//            new Wait(2000),

//            new DoLineManipulation(null,new ObservableCollection<string>() { "CAN1_Low_Line", "GND"}),
//            new Wait(2000),
//            new EcuOn(),
//            new Wait(Tdiag -200),
//            new ResetLineManipulation(),
//            new Wait(3000),

//            new ReadLamps(new ObservableCollection<string>() { "Full_system" }),
//            new ReadCanSignals(new ObservableCollection<string>() {"Full_system"}),
//            new Wait(2000),

//            new EcuOff(),
//            new TraceStop(),

//           });

//        _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
//        _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };

//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;

//    }

//    /// <summary>
//    /// CANL short to Uz before BPU
//    /// </summary>
//    /// <param name="Tdiag"></param>
//    /// <param name="productType"></param>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanLSh2UzBPU(int Tdiag, string productType, bool isNofault)
//    {
//        TestScript _script = new TestScript();
//        _script.Name = @"RB_UNIVERSAL_01J.CanL_Short_Ubatt_BPU";
//        _script.Purpose.ScalarValue = @"CAN line monitoring CAN_L short to Ubatt before power up";

//        if (isNofault)
//        {
//            _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//        new TraceStart(),
//        new Wait(2000),
//        new DoLineManipulation(null,new ObservableCollection<string>() { "CAN1_Low_Line", "UBATT"}),
//        new Wait(2500),
//        new EcuOn(),
//        new Wait(Tdiag +200),
//        new ResetLineManipulation(),
//        new Wait(3000),

//        new ReadLamps(new ObservableCollection<string>() { "Full_system" }),
//        new ReadCanSignals(new ObservableCollection<string>() {"Full_system"}),
//        new Wait(2000),

//        new EcuOff(),
//        new TraceStop(),

//         });
//        }
//        else
//        {
//            _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//        new TraceStart(),
//        new Wait(2000),
//        new DoLineManipulation(null,new ObservableCollection<string>() { "CAN1_Low_Line", "UBATT"}),
//        new Wait(2500),
//        new EcuOn(),
//        new Wait(Tdiag +200),
//        new ResetLineManipulation(),
//        new Wait(3000),

//        productType == "ABS"?new ReadLamps(new ObservableCollection<string>() { "Full_system" }):new ReadLamps(new ObservableCollection<string>() { "ESP_off" }),
//        productType == "ABS"?new ReadCanSignals(new ObservableCollection<string>() {"Full_system"}):new ReadCanSignals(new ObservableCollection<string>() {"ESP_off"}),
//        new Wait(2000),

//        new EcuOff(),
//        new TraceStop(),

//         });
//        }
//        if (productType == "Gen93")
//        {
//            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "CANSM_NETWORK_0_E_BUSOFF" };
//            _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "CANSM_NETWORK_1_E_BUSOFF" };
//            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "CANSM_NETWORK_0_E_BUSOFF" };
//            _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "CANSM_NETWORK_1_E_BUSOFF" };
//        }
//        else
//        {
//            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_BusOffNetwork0" };
//            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_BusOffNetwork0" };
//        }

//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;
//    }

//    /// <summary>
//    /// CANL short to Uz BPU
//    /// </summary>
//    /// <param name="Tdiag"></param>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanLSh2UzBPUNoFault(int Tdiag)
//    {
//        TestScript _script = new TestScript();
//        _script.Name = @"RB_UNIVERSAL_01J.CanL_Short_Ubatt_BPU_NoFault";
//        _script.Purpose.ScalarValue = @"CAN line monitoring CAN_L short to Ubatt before power up with in Tdiag";


//        _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//        new TraceStart(),
//        new Wait(2000),
//        new DoLineManipulation(null,new ObservableCollection<string>() { "CAN1_Low_Line", "UBATT"}),
//        new Wait(2500),
//        new EcuOn(),
//        new Wait(Tdiag - 200),
//        new ResetLineManipulation(),
//        new Wait(3000),

//        new ReadLamps(new ObservableCollection<string>() { "Full_system" }),
//        new ReadCanSignals(new ObservableCollection<string>() {"Full_system"}),
//        new Wait(2000),

//        new EcuOff(),
//        new TraceStop(),

//    });

//        _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
//        _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };


//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;
//    }

//    /// <summary>
//    /// CANH short to CANL BPU
//    /// </summary>
//    /// <param name="Tdiag"></param>
//    /// <param name="productType"></param>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanHSh2CANLBPU(int Tdiag, string productType, bool isNofault)
//    {
//        TestScript _script = new TestScript();
//        _script.Name = @"RB_UNIVERSAL_01J.CanH_Short_CanL_BPU";
//        _script.Purpose.ScalarValue = @"CAN line monitoring CAN_H short to CAN_L before power up";

//        if (isNofault)
//        {
//            _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//        new TraceStart(),
//        new Wait(2000),
//        new DoLineManipulation(null,new ObservableCollection<string>() { "CAN1_High_Line", "CAN1_Low_Line"}),
//        new Wait(2000),
//        new EcuOn(),
//        new Wait(Tdiag + 200),
//        new ResetLineManipulation(),
//        new Wait(3000),
//        new ReadLamps(new ObservableCollection<string>() { "Full_system" }),
//        new ReadCanSignals(new ObservableCollection<string>() {"Full_system"}),
//        new Wait(2000),

//        new EcuOff(),
//        new TraceStop(),

//    });
//        }

//        else
//        {
//            _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//        new TraceStart(),
//        new Wait(2000),
//        new DoLineManipulation(null,new ObservableCollection<string>() { "CAN1_High_Line", "CAN1_Low_Line"}),
//        new Wait(2000),
//        new EcuOn(),
//        new Wait(Tdiag + 200),
//        new ResetLineManipulation(),
//        new Wait(3000),

//        productType == "ABS"? new ReadLamps(new ObservableCollection<string>() { "Full_system" }):new ReadLamps(new ObservableCollection<string>() { "ESP_off" }),
//        productType == "ABS"? new ReadCanSignals(new ObservableCollection<string>() {"Full_system"}): new ReadCanSignals(new ObservableCollection<string>() {"ESP_off"}),
//        new Wait(2000),

//        new EcuOff(),
//        new TraceStop(),

//    });
//        }
//        _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_BusOffNetwork0" };
//        _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_BusOffNetwork0" };


//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;


//    }

//    /// <summary>
//    /// CANH short to CANL before BPU
//    /// </summary>
//    /// <param name="Tdiag"></param>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanHSh2CANLBPUNoFault(int Tdiag)
//    {
//        TestScript _script = new TestScript();
//        _script.Name = @"RB_UNIVERSAL_01J.CanH_Short_CanL_BPU_NoFault";
//        _script.Purpose.ScalarValue = @"CAN line monitoring CAN_H short to CAN_L before power up with in Tdiag";


//        _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//        new TraceStart(),
//        new Wait(2000),
//        new DoLineManipulation(null,new ObservableCollection<string>() { "CAN1_High_Line", "CAN1_Low_Line"}),
//        new Wait(2000),
//        new EcuOn(),
//        new Wait(Tdiag - 200),
//        new ResetLineManipulation(),
//        new Wait(3000),

//        new ReadLamps(new ObservableCollection<string>() { "Full_system" }),
//        new ReadCanSignals(new ObservableCollection<string>() {"Full_system"}),
//        new Wait(2000),

//        new EcuOff(),
//        new TraceStop(),

//    });

//        _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
//        _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };


//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;


//    }

//    public static ObservableCollection<TestScript> GetTScriptBPU(bool isNoFaultCanHshUbatt, bool isNoFaultCanLshGnd, bool isNoFaultErrorPassive, bool isNoFaultBusOffReset, int Tdiag, string productType)
//    {
//        ObservableCollection<TestScript> _tsBusGenBPU = new ObservableCollection<TestScript>();
//        _tsBusGenBPU.Add(GetTScriptCanHIntrptBPU(Tdiag, productType, isNoFaultErrorPassive));
//        //_tsBusGenBPU.Add(GetTScriptCanHIntrptBPUNoFault(Tdiag));
//        _tsBusGenBPU.Add(GetTScriptCanHSh2GndBPU(Tdiag, productType, isNoFaultBusOffReset));
//        //_tsBusGenBPU.Add(GetTScriptCanHSh2GndBPUNoFault(Tdiag));
//        _tsBusGenBPU.Add(GetTScriptCanHSh2UzBPU(isNoFaultCanHshUbatt, Tdiag, productType));
//        //_tsBusGenBPU.Add(GetTScriptCanHSh2UzBPUNoFault(Tdiag));
//        _tsBusGenBPU.Add(GetTScriptCanLIntrptBPU(Tdiag, productType, isNoFaultErrorPassive));
//        //_tsBusGenBPU.Add(GetTScriptCanLIntrptBPUNoFault(Tdiag));
//        _tsBusGenBPU.Add(GetTScriptCanLSh2UzBPU(Tdiag, productType, isNoFaultBusOffReset));
//        //_tsBusGenBPU.Add(GetTScriptCanLSh2UzBPUNoFault(Tdiag));
//        _tsBusGenBPU.Add(GetTScriptCanLSh2GndBPU(isNoFaultCanLshGnd, Tdiag, productType));
//        //_tsBusGenBPU.Add(GetTScriptCanLSh2GndBPUNoFault(Tdiag));
//        _tsBusGenBPU.Add(GetTScriptCanHSh2CANLBPU(Tdiag, productType, isNoFaultBusOffReset));
//        //_tsBusGenBPU.Add(GetTScriptCanHSh2CANLBPUNoFault(Tdiag));

//        return _tsBusGenBPU;

//    }

//    #endregion

//    #region NUV & OV
//    /// <summary>
//    /// All test case of CANH interrupt in various Ubatt
//    /// </summary>
//    /// <param name="volt"></param>
//    /// <param name="threshNUV"></param>
//    /// <param name="threshOV"></param>
//    /// <param name="spdLimitKph"></param>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanHIntrptVoltCondition(double volt, double threshNUV, double threshHydrHardUV, double threshOV, string productType, int spdLimitKph = 10)
//    {
//        TestScript _script = new TestScript();
//        if (volt <= threshNUV)
//        {

//            _script.Name = @"RB_UNIVERSAL_01J.CanH_Interrupt_NUV";
//            _script.Purpose.ScalarValue = @"CAN line monitoring CAN_H interrupt during undervoltage";
//        }
//        else if (volt >= threshOV)
//        {
//            _script.Name = @"RB_UNIVERSAL_01J.CanH_Interrupt_OV";
//            _script.Purpose.ScalarValue = @"CAN line monitoring CAN_H interrupt during overvoltage";
//        }




//        _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//        new TraceStart(),
//        new EcuOn(),
//        new Wait(5000),
//        new StimuliStart(new ObservableCollection<string>() { "10kph_in_5s.lcs"}),
//        new Wait(5000),
//        new SetModelValues(new Dictionary<string, string>{ {"BatteryVoltage", volt.ToString()} }),
//        new Wait(2000),
//        new DoLineManipulation(new ObservableCollection<string>() { "CAN1_High_Line"}),
//        new Wait(2000),
//        new ResetLineManipulation(),
//        new Wait(2000),

//        new ReadLamps(GetStrategyStr( volt,  threshNUV,   threshHydrHardUV,  threshOV,  productType)),
//        new ReadCanSignals(GetStrategyStr( volt,  threshNUV,  threshHydrHardUV,  threshOV,  productType)),

//        new Wait(2000),

//        new EcuOff(),
//        new StimuliStop(),
//        new UndoSetModelValues(),
//        new TraceStop(),

//    });
//        if (volt < threshHydrHardUV)
//        {
//            switch (productType)
//            {
//                case "APBMi5064":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage", "ApbMotorSupplyUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "ApbMotorSupplyUndervoltage" };
//                    break;

//                case "APBMi5065":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage", "ApbMotorSupplyUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "ApbMotorSupplyUndervoltage" };
//                    break;

//                case "ESP":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };
//                    break;
//                case "ABS":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };
//                    break;
//                case "Gen93":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "RBHydraulicSupplyUndervoltage", "RBHydraulicSupplyHardUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "RBHydraulicSupplyUndervoltage", "RBHydraulicSupplyHardUndervoltage" };
//                    break;

//            }

//        }
//        else if (volt > threshOV)
//        {

//            switch (productType)
//            {
//                case "APBMi5064":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    break;
//                case "APBMi5065":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    break;

//                case "ESP":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    break;

//                case "ABS":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    break;
//                case "Gen93":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    break;

//            }

//        }
//        else if (volt < threshNUV)
//        {
//            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };
//            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//            _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", };
//            _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };

//        }


//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;


//    }
//    /// <summary>
//    /// All test case of CANL interrupt in various Ubatt 
//    /// </summary>
//    /// <param name="volt"></param>
//    /// <param name="threshNUV"></param>
//    /// <param name="threshOV"></param>
//    /// <param name="spdLimitKph"></param>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanLIntrptVoltCondition(double volt, double threshNUV, double threshHydrHardUV, double threshOV, string productType, int spdLimitKph = 10)
//    {
//        TestScript _script = new TestScript();
//        if (volt <= threshNUV)
//        {

//            _script.Name = @"RB_UNIVERSAL_01J.CanL_Interrupt_NUV";
//            _script.Purpose.ScalarValue = @"CAN line monitoring CAN_L interrupt during undervoltage";
//        }
//        else if (volt >= threshOV)
//        {
//            _script.Name = @"RB_UNIVERSAL_01J.CanL_Interrupt_OV";
//            _script.Purpose.ScalarValue = @"CAN line monitoring CAN_L interrupt during overvoltage";
//        }




//        _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//        new TraceStart(),
//        new EcuOn(),
//        new Wait(5000),
//        new StimuliStart(new ObservableCollection<string>() { "10kph_in_5s.lcs"}),
//        new Wait(5000),
//        new SetModelValues(new Dictionary<string, string>{ {"BatteryVoltage", volt.ToString()} }),
//        new Wait(2000),
//        new DoLineManipulation(new ObservableCollection<string>() { "CAN1_Low_Line"}),
//        new Wait(2000),
//        new ResetLineManipulation(),
//        new Wait(2000),
//        new ReadLamps(GetStrategyStr( volt,  threshNUV,   threshHydrHardUV,  threshOV,  productType)),
//        new ReadCanSignals(GetStrategyStr( volt,  threshNUV,  threshHydrHardUV,  threshOV,  productType)),
//        new Wait(2000),

//        new EcuOff(),
//        new StimuliStop(),
//        new UndoSetModelValues(),
//        new TraceStop(),

//    });
//        if (volt < threshHydrHardUV)
//        {

//            switch (productType)
//            {
//                case "APBMi5064":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage", "ApbMotorSupplyUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "ApbMotorSupplyUndervoltage" };
//                    break;

//                case "APBMi5065":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage", "ApbMotorSupplyUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "ApbMotorSupplyUndervoltage" };
//                    break;

//                case "ESP":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };
//                    break;
//                case "ABS":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };
//                    break;
//                case "Gen93":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "RBHydraulicSupplyUndervoltage", "RBHydraulicSupplyHardUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "RBHydraulicSupplyUndervoltage",
//                    "RBHydraulicSupplyHardUndervoltage"};
//                    break;

//            }
//        }
//        else if (volt > threshOV)
//        {

//            switch (productType)
//            {
//                case "APBMi5064":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    break;
//                case "APBMi5065":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    break;

//                case "ESP":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    break;

//                case "ABS":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    break;
//                case "Gen93":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "RBSupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "RBSupplyOvervoltage" };
//                    break;
//            }


//        }
//        else if (volt < threshNUV)
//        {
//            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };
//            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//            _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", };
//            _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };

//        }

//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;


//    }

//    /// <summary>
//    /// Check CANH short to Ubatt in various voltage condition 
//    /// </summary>
//    /// <param name="volt"></param>
//    /// <param name="threshNUV"></param>
//    /// <param name="threshOV"></param>
//    /// <param name="spdLimitKph"></param>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanHSh2UbattVoltCondition(double volt, double threshNUV, double threshHydrHardUV, double threshOV, string productType, int spdLimitKph = 10)
//    {
//        TestScript _script = new TestScript();
//        if (volt <= threshNUV)
//        {

//            _script.Name = @"RB_UNIVERSAL_01J.CanH_Short_Ubatt_NUV";
//            _script.Purpose.ScalarValue = @"CAN line monitoring CAN_H short to Ubatt during undervoltage";
//        }
//        else if (volt >= threshOV)
//        {
//            _script.Name = @"RB_UNIVERSAL_01J.CanH_Short_Ubatt_OV";
//            _script.Purpose.ScalarValue = @"CAN line monitoring CAN_H short to Ubatt during overvoltage";
//        }




//        _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//        new TraceStart(),
//        new EcuOn(),
//        new Wait(5000),
//        new StimuliStart(new ObservableCollection<string>() { "10kph_in_5s.lcs"}),
//        new Wait(5000),
//        new SetModelValues(new Dictionary<string, string>{ {"BatteryVoltage", volt.ToString()} }),
//        new Wait(2000),
//        new DoLineManipulation(null,new ObservableCollection<string>() { "CAN1_High_Line","UBATT"}),
//        new Wait(2000),
//        new ResetLineManipulation(),
//        new Wait(2000),
//        new ReadLamps(GetStrategyStr( volt,  threshNUV,    threshHydrHardUV,  threshOV,  productType)),
//        new ReadCanSignals(GetStrategyStr( volt,  threshNUV,   threshHydrHardUV,  threshOV,  productType)),
//        new Wait(2000),

//        new EcuOff(),
//        new StimuliStop(),
//        new UndoSetModelValues(),
//        new TraceStop(),

//    });
//        if (volt < threshHydrHardUV)
//        {
//            switch (productType)
//            {
//                case "APBMi5064":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage", "ApbMotorSupplyUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "ApbMotorSupplyUndervoltage" };
//                    break;

//                case "APBMi5065":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage", "ApbMotorSupplyUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "ApbMotorSupplyUndervoltage" };
//                    break;

//                case "ESP":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };
//                    break;
//                case "ABS":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };
//                    break;
//                case "Gen93":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };
//                    break;

//            }
//        }
//        else if (volt > threshOV)
//        {

//            switch (productType)
//            {
//                case "APBMi5064":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    break;
//                case "APBMi5065":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    break;

//                case "ESP":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    break;

//                case "ABS":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    break;
//                case "Gen93":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    break;
//            }

//        }
//        else if (volt < threshNUV)
//        {
//            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };
//            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//            _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", };
//            _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };

//        }

//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;

//    }

//    /// <summary>
//    ///  Check CANH short to Gnd in various voltage condition
//    /// </summary>
//    /// <param name="volt"></param>
//    /// <param name="threshNUV"></param>
//    /// <param name="threshOV"></param>
//    /// <param name="spdLimitKph"></param>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanHSh2GndVoltCondition(double volt, double threshNUV, double threshHydrHardUV, double threshOV, string productType, int spdLimitKph = 10)
//    {
//        TestScript _script = new TestScript();
//        if (volt <= threshNUV)
//        {

//            _script.Name = @"RB_UNIVERSAL_01J.CanH_Short_Gnd_NUV";
//            _script.Purpose.ScalarValue = @"CAN line monitoring CAN_H short to Gnd during undervoltage";
//        }
//        else if (volt >= threshOV)
//        {
//            _script.Name = @"RB_UNIVERSAL_01J.CanH_Short_Gnd_OV";
//            _script.Purpose.ScalarValue = @"CAN line monitoring CAN_H short to Gnd during overvoltage";
//        }




//        _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//        new TraceStart(),
//        new EcuOn(),
//        new Wait(5000),
//        new StimuliStart(new ObservableCollection<string>() { "10kph_in_5s.lcs"}),
//        new Wait(5000),
//        new SetModelValues(new Dictionary<string, string>{ {"BatteryVoltage", volt.ToString()} }),
//        new Wait(2000),
//        new DoLineManipulation(null,new ObservableCollection<string>() { "CAN1_High_Line","GND"}),
//        new Wait(2000),
//        new ResetLineManipulation(),
//        new Wait(2000),

//        new ReadLamps(GetStrategyStr( volt,  threshNUV,   threshHydrHardUV,  threshOV,  productType)),
//        new ReadCanSignals(GetStrategyStr( volt,  threshNUV,    threshHydrHardUV,  threshOV,  productType)),
//        new Wait(2000),

//        new EcuOff(),
//        new StimuliStop(),
//        new UndoSetModelValues(),
//        new TraceStop(),

//    });
//        if (volt < threshHydrHardUV)
//        {
//            switch (productType)
//            {
//                case "APBMi5064":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage", "ApbMotorSupplyUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "ApbMotorSupplyUndervoltage" };
//                    break;

//                case "APBMi5065":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage", "ApbMotorSupplyUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "ApbMotorSupplyUndervoltage" };
//                    break;

//                case "ESP":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };
//                    break;
//                case "ABS":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };
//                    break;
//                case "Gen93":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "RBHydraulicSupplyUndervoltage", "RBHydraulicSupplyHardUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "RbHydraulicSupplyUndervoltage", "RBHydraulicSupplyHardUndervoltage" };
//                    break;
//            }
//        }
//        else if (volt > threshOV)
//        {

//            switch (productType)
//            {
//                case "APBMi5064":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    break;
//                case "APBMi5065":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    break;

//                case "ESP":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    break;

//                case "ABS":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    break;
//                case "Gen93":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "RBSupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "RBSupplyOvervoltage" };
//                    break;
//            }

//        }
//        else if (volt < threshNUV)
//        {
//            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };
//            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//            _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", };
//            _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };

//        }

//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;

//    }


//    /// <summary>
//    /// Check CANL short to Ubatt in various voltage condition 
//    /// </summary>
//    /// <param name="volt"></param>
//    /// <param name="threshNUV"></param>
//    /// <param name="threshOV"></param>
//    /// <param name="spdLimitKph"></param>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanLSh2UbattVoltCondition(double volt, double threshNUV, double threshHydrHardUV, double threshOV, string productType, int spdLimitKph = 10)
//    {
//        TestScript _script = new TestScript();
//        if (volt <= threshNUV)
//        {

//            _script.Name = @"RB_UNIVERSAL_01J.CanL_Short_Ubatt_NUV";
//            _script.Purpose.ScalarValue = @"CAN line monitoring CAN_L short to Ubatt during undervoltage";
//        }
//        else if (volt >= threshOV)
//        {
//            _script.Name = @"RB_UNIVERSAL_01J.CanL_Short_Ubatt_OV";
//            _script.Purpose.ScalarValue = @"CAN line monitoring CAN_L short to Ubatt during overvoltage";
//        }




//        _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//        new TraceStart(),
//        new EcuOn(),
//        new Wait(5000),
//        new StimuliStart(new ObservableCollection<string>() { "10kph_in_5s.lcs"}),
//        new Wait(5000),
//        new SetModelValues(new Dictionary<string, string>{ {"BatteryVoltage", volt.ToString()} }),
//        new Wait(2000),
//        new DoLineManipulation(null,new ObservableCollection<string>() { "CAN1_Low_Line","UBATT"}),
//        new Wait(2000),
//        new ResetLineManipulation(),
//        new Wait(2000),
//        new ReadLamps(GetStrategyStr( volt,  threshNUV,   threshHydrHardUV,  threshOV,  productType)),
//        new ReadCanSignals(GetStrategyStr( volt,  threshNUV,   threshHydrHardUV,  threshOV,  productType)),
//        new Wait(2000),

//        new EcuOff(),
//        new StimuliStop(),
//        new UndoSetModelValues(),
//        new TraceStop(),

//    });
//        if (volt < threshHydrHardUV)
//        {
//            switch (productType)
//            {
//                case "APBMi5064":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage", "ApbMotorSupplyUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "ApbMotorSupplyUndervoltage" };
//                    break;

//                case "APBMi5065":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage", "ApbMotorSupplyUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "ApbMotorSupplyUndervoltage" };
//                    break;

//                case "ESP":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };
//                    break;
//                case "ABS":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };
//                    break;
//                case "Gen93":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "RBHydraulicSupplyUndervoltage", "RBHydraulicSupplyHardUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "RBHydraulicSupplyUndervoltage", "RBHydraulicSupplyHardUndervoltage" };
//                    break;
//            }
//        }
//        else if (volt > threshOV)
//        {

//            switch (productType)
//            {
//                case "APBMi5064":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    break;
//                case "APBMi5065":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    break;

//                case "ESP":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    break;

//                case "ABS":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    break;
//                case "Gen93":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "RBSupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "RBSupplyOvervoltage" };
//                    break;
//            }


//        }
//        else if (volt < threshNUV)
//        {
//            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };
//            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//            _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", };
//            _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };

//        }

//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;

//    }

//    /// <summary>
//    ///  Check CANL short to Gnd in various voltage condition
//    /// </summary>
//    /// <param name="volt"></param>
//    /// <param name="threshNUV"></param>
//    /// <param name="threshOV"></param>
//    /// <param name="spdLimitKph"></param>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanLSh2GndVoltCondition(double volt, double threshNUV, double threshHydrHardUV, double threshOV, string productType, int spdLimitKph = 10)
//    {
//        TestScript _script = new TestScript();
//        if (volt <= threshNUV)
//        {

//            _script.Name = @"RB_UNIVERSAL_01J.CanL_Short_Gnd_NUV";
//            _script.Purpose.ScalarValue = @"CAN line monitoring CAN_L short to Gnd during undervoltage";
//        }
//        else if (volt >= threshOV)
//        {
//            _script.Name = @"RB_UNIVERSAL_01J.CanL_Short_Gnd_OV";
//            _script.Purpose.ScalarValue = @"CAN line monitoring CAN_L short to Gnd during overvoltage";
//        }


//        _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//        new TraceStart(),
//        new EcuOn(),
//        new Wait(5000),
//        new StimuliStart(new ObservableCollection<string>() { "10kph_in_5s.lcs"}),
//        new Wait(5000),
//        new SetModelValues(new Dictionary<string, string>{ {"BatteryVoltage", volt.ToString()} }),
//        new Wait(2000),
//        new DoLineManipulation(null,new ObservableCollection<string>() { "CAN1_Low_Line","GND"}),
//        new Wait(2000),
//        new ResetLineManipulation(),
//        new Wait(2000),
//        new ReadLamps(GetStrategyStr( volt,  threshNUV,   threshHydrHardUV,  threshOV,  productType)),
//        new ReadCanSignals(GetStrategyStr( volt,  threshNUV,    threshHydrHardUV,  threshOV,  productType)),
//        new Wait(2000),

//        new EcuOff(),
//        new StimuliStop(),
//        new UndoSetModelValues(),
//        new TraceStop(),

//    });
//        if (volt < threshHydrHardUV)
//        {
//            switch (productType)
//            {
//                case "APBMi5064":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage", "ApbMotorSupplyUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "ApbMotorSupplyUndervoltage" };
//                    break;

//                case "APBMi5065":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage", "ApbMotorSupplyUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "ApbMotorSupplyUndervoltage" };
//                    break;

//                case "ESP":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };
//                    break;
//                case "ABS":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };
//                    break;
//                case "Gen93":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "RBHydraulicSupplyUndervoltage", "RBHydraulicSupplyHardUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "RBHydraulicSupplyUndervoltage", "RBHydraulicSupplyHardUndervoltage" };
//                    break;
//            }
//        }
//        else if (volt > threshOV)
//        {

//            switch (productType)
//            {
//                case "APBMi5064":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    break;
//                case "APBMi5065":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    break;

//                case "ESP":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    break;

//                case "ABS":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    break;
//                case "Gen93":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "RBSupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "RBSupplyOvervoltage" };
//                    break;
//            }

//        }
//        else if (volt < threshNUV)
//        {
//            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };
//            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//            _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", };
//            _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };

//        }

//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;

//    }
//    /// <summary>
//    /// Check CANL short to CANL in various voltage condition
//    /// </summary>
//    /// <param name="volt"></param>
//    /// <param name="threshNUV"></param>
//    /// <param name="threshOV"></param>
//    /// <param name="spdLimitKph"></param>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanHSh2CanLVoltCondition(double volt, double threshNUV, double threshHydrHardUV, double threshOV, string productType, int spdLimitKph = 10)
//    {
//        TestScript _script = new TestScript();
//        if (volt <= threshNUV)
//        {

//            _script.Name = @"RB_UNIVERSAL_01J.CanH_Short_CanL_NUV";
//            _script.Purpose.ScalarValue = @"CAN line monitoring CAN_H short to CAN_L during undervoltage";
//        }
//        else if (volt >= threshOV)
//        {
//            _script.Name = @"RB_UNIVERSAL_01J.CanH_Short_CanL_OV";
//            _script.Purpose.ScalarValue = @"CAN line monitoring CAN_H short to CAN_L during overvoltage";
//        }




//        _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//        new TraceStart(),
//        new EcuOn(),
//        new Wait(5000),
//        new StimuliStart(new ObservableCollection<string>() { "10kph_in_5s.lcs"}),
//        new Wait(5000),
//        new SetModelValues(new Dictionary<string, string>{ {"BatteryVoltage", volt.ToString()} }),
//        new Wait(2000),
//        new DoLineManipulation(null,new ObservableCollection<string>() { "CAN1_High_Line","CAN1_Low_Line"}),
//        new Wait(2000),
//        new ResetLineManipulation(),
//        new Wait(2000),

//        new ReadLamps(GetStrategyStr( volt,  threshNUV,  threshHydrHardUV,  threshOV,  productType)),
//        new ReadCanSignals(GetStrategyStr( volt,  threshNUV,   threshHydrHardUV,  threshOV,  productType)),

//        new Wait(2000),

//        new EcuOff(),
//        new StimuliStop(),
//        new UndoSetModelValues(),
//        new TraceStop(),

//    });
//        if (volt < threshHydrHardUV)
//        {
//            switch (productType)
//            {
//                case "APBMi5064":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage", "ApbMotorSupplyUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "ApbMotorSupplyUndervoltage" };
//                    break;

//                case "APBMi5065":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage", "ApbMotorSupplyUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "ApbMotorSupplyUndervoltage" };
//                    break;

//                case "ESP":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };
//                    break;
//                case "ABS":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "HydraulicSupplyHardUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };
//                    break;
//                case "Gen93":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Undervoltage" };

//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Undervoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "RBHydraulicSupplyUndervoltage", "RBHydraulicSupplyHardUndervoltage" };

//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "RBHydraulicSupplyUndervoltage", "RBHydraulicSupplyHardUndervoltage" };
//                    break;
//            }
//        }
//        else if (volt > threshOV)
//        {

//            switch (productType)
//            {
//                case "APBMi5064":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    break;
//                case "APBMi5065":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
//                    break;

//                case "ESP":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    break;

//                case "ABS":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage" };
//                    break;
//                case "Gen93":
//                    _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Overvoltage" };
//                    _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Overvoltage" };

//                    _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "RBSupplyOvervoltage" };
//                    _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "RBSupplyOvervoltage" };
//                    break;
//            }

//        }
//        else if (volt < threshNUV)
//        {
//            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };
//            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };

//            _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", };
//            _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };

//        }

//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;

//    }

//    private static ObservableCollection<string> GetStrategyStr(double volt, double threshNUV, double threshHydrHardUV, double threshOV, string productType)
//    {
//        ObservableCollection<string> lampStr = new ObservableCollection<string>();


//        if (volt <= threshHydrHardUV)
//        {
//            lampStr = new ObservableCollection<string>() { "EBV_off" };
//        }
//        else if (volt >= threshOV)
//        {
//            lampStr = new ObservableCollection<string>() { "ABS_off" };
//        }

//        else if (volt <= threshNUV)
//        {
//            lampStr = (productType == "ABS") ? new ObservableCollection<string>() { "Full_system" } : new ObservableCollection<string>() { "ESP_off" };
//        }
//        else
//        {
//            lampStr = new ObservableCollection<string>() { "Full_system" };
//        }

//        return lampStr;

//    }


//    public static ObservableCollection<TestScript> GetTScriptVoltCondition(double threshNUV, double threshHydrHardUV, double threshOV, string productType, int spdLimitKph = 10)
//    {

//        ObservableCollection<TestScript> _tsBusGenBPU = new ObservableCollection<TestScript>();
//        _tsBusGenBPU.Add(GetTScriptCanHIntrptVoltCondition(threshNUV - 0.5, threshNUV, threshHydrHardUV, threshOV, productType, spdLimitKph));
//        _tsBusGenBPU.Add(GetTScriptCanLIntrptVoltCondition(threshNUV - 0.5, threshNUV, threshHydrHardUV, threshOV, productType, spdLimitKph));
//        _tsBusGenBPU.Add(GetTScriptCanHSh2UbattVoltCondition(threshNUV - 0.5, threshNUV, threshHydrHardUV, threshOV, productType, spdLimitKph));
//        _tsBusGenBPU.Add(GetTScriptCanHSh2GndVoltCondition(threshNUV - 0.5, threshNUV, threshHydrHardUV, threshOV, productType, spdLimitKph));
//        _tsBusGenBPU.Add(GetTScriptCanLSh2UbattVoltCondition(threshNUV - 0.5, threshNUV, threshHydrHardUV, threshOV, productType, spdLimitKph));
//        _tsBusGenBPU.Add(GetTScriptCanLSh2GndVoltCondition(threshNUV - 0.5, threshNUV, threshHydrHardUV, threshOV, productType, spdLimitKph));
//        _tsBusGenBPU.Add(GetTScriptCanHSh2CanLVoltCondition(threshNUV - 0.5, threshNUV, threshHydrHardUV, threshOV, productType, spdLimitKph));

//        _tsBusGenBPU.Add(GetTScriptCanHIntrptVoltCondition(threshOV + 0.5, threshNUV, threshHydrHardUV, threshOV, productType, spdLimitKph));
//        _tsBusGenBPU.Add(GetTScriptCanLIntrptVoltCondition(threshOV + 0.5, threshNUV, threshHydrHardUV, threshOV, productType, spdLimitKph));
//        _tsBusGenBPU.Add(GetTScriptCanHSh2UbattVoltCondition(threshOV + 0.5, threshNUV, threshHydrHardUV, threshOV, productType, spdLimitKph));
//        _tsBusGenBPU.Add(GetTScriptCanHSh2GndVoltCondition(threshOV + 0.5, threshNUV, threshHydrHardUV, threshOV, productType, spdLimitKph));
//        _tsBusGenBPU.Add(GetTScriptCanLSh2UbattVoltCondition(threshOV + 0.5, threshNUV, threshHydrHardUV, threshOV, productType, spdLimitKph));
//        _tsBusGenBPU.Add(GetTScriptCanLSh2GndVoltCondition(threshOV + 0.5, threshNUV, threshHydrHardUV, threshOV, productType, spdLimitKph));
//        _tsBusGenBPU.Add(GetTScriptCanHSh2CanLVoltCondition(threshOV + 0.5, threshNUV, threshHydrHardUV, threshOV, productType, spdLimitKph));

//        return _tsBusGenBPU;


//    }
//    #endregion

//    #region Controller active
//    /// <summary>
//    /// Get test script for CANH interrupt when ABS active
//    /// </summary>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanHSh2CanLAbsActive()
//    {
//        TestScript _script = new TestScript();
//        _script.Name = @"RB_UNIVERSAL_01J.CanH_Short_CanL_AbsActive";
//        _script.Purpose.ScalarValue = @"CAN line monitoring CAN_H short to CAN_L during ABS Active";



//        _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//        new TraceStart(),
//        new EcuOn(),
//        new Wait(5000),
//        new StimuliStart(new ObservableCollection<string>() { "ABS.lcs"}),
//        new Wait(5000),

//        new Wait(2000),
//        new DoLineManipulation(null,new ObservableCollection<string>() { "CAN1_High_Line","CAN1_Low_Line"}),
//        new Wait(2000),
//        new ResetLineManipulation(),
//        new Wait(2000),

//        new ReadLamps(new ObservableCollection<string>() { "ESP_off" }),

//        new ReadCanSignals(new ObservableCollection<string>() { "ESP_off" }),
//        new Wait(2000),
//        new EcuOff(),
//        new StimuliStop(),

//        new TraceStop(),

//    });

//        _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_BusOffNetwork0" };
//        _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_BusOffNetwork0" };


//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;

//    }

//    /// <summary>
//    /// Get test script for CANH and CANL short when TCS active
//    /// </summary>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanHIntrptTcsActive()
//    {
//        TestScript _script = new TestScript();
//        _script.Name = @"RB_UNIVERSAL_01J.CanH_Interrupt_TcsActive";
//        _script.Purpose.ScalarValue = @"CAN line monitoring CAN_H interrupt during TCS active";



//        _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//        new TraceStart(),
//        new EcuOn(),
//        new Wait(5000),
//        new StimuliStart(new ObservableCollection<string>() { "TCS.lcs"}),
//        new Wait(5000),

//        new Wait(2000),
//        new DoLineManipulation(new ObservableCollection<string>() { "CAN1_High_Line"}),
//        new Wait(2000),
//        new ResetLineManipulation(),
//        new Wait(2000),

//        new ReadLamps(new ObservableCollection<string>() { "ESP_off" }),

//        new ReadCanSignals(new ObservableCollection<string>() { "ESP_off" }),
//        new Wait(2000),
//        new EcuOff(),
//        new StimuliStop(),

//        new TraceStop(),

//    });

//        _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_ErrorPassivNetwork0" };
//        _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_ErrorPassivNetwork0" };


//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;

//    }
//    /// <summary>
//    /// Get test script for CANL interrupt when TCS active
//    /// </summary>
//    /// <returns></returns>
//    private static TestScript GetTScriptCanLIntrptVdcActive()
//    {
//        TestScript _script = new TestScript();
//        _script.Name = @"RB_UNIVERSAL_01J.CanH_Interrupt_VdcActive";
//        _script.Purpose.ScalarValue = @"CAN line monitoring CAN_L interrupt during TCS active";



//        _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

//        new TraceStart(),
//        new EcuOn(),
//        new Wait(5000),
//        new StimuliStart(new ObservableCollection<string>() { "VDC.lcs"}),
//        new Wait(5000),

//        new Wait(2000),
//        new DoLineManipulation(new ObservableCollection<string>() { "CAN1_Low_Line"}),
//        new Wait(2000),
//        new ResetLineManipulation(),
//        new Wait(2000),

//        new ReadLamps(new ObservableCollection<string>() { "ESP_off" }),

//        new ReadCanSignals(new ObservableCollection<string>() { "ESP_off" }),
//        new Wait(2000),
//        new EcuOff(),
//        new StimuliStop(),

//        new TraceStop(),

//    });

//        _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_ErrorPassivNetwork0" };
//        _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_ErrorPassivNetwork0" };


//        _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
//        _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

//        return _script;

//    }


//    public static ObservableCollection<TestScript> GetTScriptController()
//    {
//        // Why remove, will ask Liansheng
//        ObservableCollection<TestScript> _tsBusGenBPU = new ObservableCollection<TestScript>();
//        //_tsBusGenBPU.Add(GetTScriptCanLIntrptVdcActive());
//        //_tsBusGenBPU.Add(GetTScriptCanHIntrptTcsActive());   //remove the controller active condition
//        //_tsBusGenBPU.Add(GetTScriptCanHSh2CanLAbsActive());
//        return _tsBusGenBPU;


//    }

//    #endregion
//}
//}