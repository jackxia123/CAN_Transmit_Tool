using RBT.Universal;
using RBT.Universal.CanEvalParameters;
using RBT.Universal.CanEvalParameters.ValueTransition;
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
    static class TcLibNetworkPar
    {

        #region Postrun
        /// <summary>
        /// Get Test Script if postrun check in standstill
        /// </summary>
        /// <returns></returns>
        public static TestScript GetTScriptPostrunStandstillTimeCheck(int postrunTime, int delayPostrun, string doorsLink, string indSig, int indValue, string productType, int toleranceMs = 1000)
        {
            TestScript _script = new TestScript();
            _script.Name = @"RB_UNIVERSAL_01J.Postrun_TimeCheck_StandStill";
            _script.Purpose.ScalarValue = @"To check postrun time during standstill, as well as postrun delay time if requested";
            _script.DoorsLink = doorsLink;
            _script.QcFolderPath = @"RBT_CANTx\Postrun";

            if (productType == "ABS")
            {
                _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

                new TraceStart(),
                new EcuOn(),
                new Wait(5000),
                new EcuOff(),
                new Wait(postrunTime+1000),
                new Wait(2000),
                new TraceStop(),

                 });

                _script.CanTraceAnalyser = new CanTraceAnalyser()
                {
                    LastFrameCheck = new LastFrameCheck(postrunTime - toleranceMs + "-" + (postrunTime + toleranceMs)),
                };

            }
            else
            {
                _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

                new TraceStart(),
                new EcuOn(),
                new Wait(5000),
                new EcuOff(),
                new Wait(delayPostrun+3000),
                new ReadCanLamps(new ObservableCollection<string> { "ESP_off"}),
                new ReadCanSignals(new ObservableCollection<string> { "ESP_off"}),
                new Wait(postrunTime-delayPostrun+1000),
                new Wait(2000),
                new TraceStop(),

            });

                Trigger lcUzOff = new Trigger(new SigConditon("LCI_UZ1", 0, TriggerConditionSignal.Equal)); //hardcode LC_Info signal
                Trigger lampOn = new Trigger(new SigConditon(indSig, indValue, TriggerConditionSignal.Equal));
                DeltaTime postrunDelay;
                if (delayPostrun == 0)
                {
                    postrunDelay = new DeltaTime(lcUzOff, lampOn, delayPostrun + "-" + (delayPostrun + toleranceMs));
                }
                else
                {
                    postrunDelay = new DeltaTime(lcUzOff, lampOn, delayPostrun - toleranceMs + "-" + (delayPostrun + toleranceMs));
                }


                _script.CanTraceAnalyser = new CanTraceAnalyser()
                {
                    LastFrameCheck = new LastFrameCheck(postrunTime - toleranceMs + "-" + (postrunTime + toleranceMs)),
                    Triggers = new ObservableCollection<TriggerBase>() { lcUzOff, lampOn },
                    DeltaTimes = new ObservableCollection<DeltaTime>() { postrunDelay },

                };

            }


            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
            _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
            _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

            return _script;
        }

        /// <summary>
        /// Get Test Script if postrun check in vehicle running
        /// </summary>
        /// <returns></returns>
        public static TestScript GetTScriptPostrunningTimeCheck(int postrunTime, int delayPostrun, string doorsLink, string indSig, int indValue, string productType, int toleranceMs = 1000)
        {
            TestScript _script = new TestScript();
            _script.Name = @"RB_UNIVERSAL_01J.Postrun_TimeCheck_Running";
            _script.Purpose.ScalarValue = @"To check postrun time during vehicle running, 10kph by default, as well as postrun delay time if requested";
            _script.DoorsLink = doorsLink;
            _script.QcFolderPath = @"RBT_CANTx\Postrun";

            if (productType == "ABS")
            {
                _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

                new TraceStart(),
                new EcuOn(),
                new Wait(5000),
                new StimuliStart(new ObservableCollection<string>() { "10kph_in_5s.lcs"}),
                new Wait(5000),
                new EcuOff(),
                new Wait(postrunTime+1000),
                new Wait(2000),
                new StimuliStop(),
                new TraceStop(),

                 });

                _script.CanTraceAnalyser = new CanTraceAnalyser()
                {
                    LastFrameCheck = new LastFrameCheck(postrunTime - toleranceMs + "-" + (postrunTime + toleranceMs)),
                };

            }
            else
            {
                _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

                new TraceStart(),
                new EcuOn(),
                new Wait(5000),
                new StimuliStart(new ObservableCollection<string>() { "10kph_in_5s.lcs"}),
                new Wait(5000),
                new EcuOff(),
                new Wait(delayPostrun+3000),
                new ReadCanLamps(new ObservableCollection<string> { "ESP_off"}),
                new ReadCanSignals(new ObservableCollection<string> { "ESP_off"}),
                new Wait(postrunTime-delayPostrun+1000),
                new Wait(2000),
                new StimuliStop(),
                new TraceStop(),

            });

                Trigger lcUzOff = new Trigger(new SigConditon("LCI_UZ1", 0, TriggerConditionSignal.Equal)); //hardcode LC_Info signal
                Trigger lampOn = new Trigger(new SigConditon(indSig, indValue, TriggerConditionSignal.Equal));
                DeltaTime postrunDelay;
                if (delayPostrun == 0)
                {
                    postrunDelay = new DeltaTime(lcUzOff, lampOn, delayPostrun + "-" + (delayPostrun + toleranceMs));
                }
                else
                {
                    postrunDelay = new DeltaTime(lcUzOff, lampOn, delayPostrun - toleranceMs + "-" + (delayPostrun + toleranceMs));
                }


                _script.CanTraceAnalyser = new CanTraceAnalyser()
                {
                    LastFrameCheck = new LastFrameCheck(postrunTime - toleranceMs + "-" + (postrunTime + toleranceMs)),
                    Triggers = new ObservableCollection<TriggerBase>() { lcUzOff, lampOn },
                    DeltaTimes = new ObservableCollection<DeltaTime>() { postrunDelay },

                };

            }


            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
            _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
            _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

            return _script;
        }

        /// <summary>
        /// Get Test Script of postrun speed threshold check
        /// </summary>
        /// <returns></returns>
        public static TestScript GetTScriptPostrunSpdThresholdUpperCheck(int spdTh, int postrunTime, int delayPostrun, string doorsLink, string indSig, int indValue, string productType, int toleranceMs = 1000)
        {
            TestScript _script = new TestScript();
            _script.Name = @"RB_UNIVERSAL_01J.Postrun_Speed_ThresholdUpperCheck";
            _script.Purpose.ScalarValue = @"To check postrun time speed threshold, default value is 1 kph";
            _script.DoorsLink = doorsLink;
            _script.QcFolderPath = @"RBT_CANTx\Postrun";


            Dictionary<string, string> modelValuePstrSpd = new Dictionary<string, string>();



            modelValuePstrSpd.Add("v_Wheel_LF", ((spdTh + 1) / 3.6).ToString("F1"));
            modelValuePstrSpd.Add("v_Wheel_RF", ((spdTh + 1) / 3.6).ToString("F1"));
            modelValuePstrSpd.Add("v_Wheel_LR", ((spdTh + 1) / 3.6).ToString("F1"));
            modelValuePstrSpd.Add("v_Wheel_RR", ((spdTh + 1) / 3.6).ToString("F1"));


            if (productType == "ABS")
            {
                _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

                new TraceStart(),
                new EcuOn(),
                new Wait(5000),
                new SetModelValues(modelValuePstrSpd),
                new Wait(5000),
                new EcuOff(),
                new Wait(postrunTime+1000),
                new Wait(2000),
                new UndoSetModelValues(),
                new TraceStop(),

                 });

                _script.CanTraceAnalyser = new CanTraceAnalyser()
                {
                    LastFrameCheck = new LastFrameCheck(postrunTime - toleranceMs + "-" + (postrunTime + toleranceMs)),
                };

            }
            else
            {
                _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

                new TraceStart(),
                new EcuOn(),
                new Wait(5000),
                new SetModelValues(modelValuePstrSpd),
                new Wait(5000),
                new EcuOff(),
                new Wait(delayPostrun+5000),
                new ReadCanLamps(new ObservableCollection<string> { "ESP_off"}),
                new ReadCanSignals(new ObservableCollection<string> { "ESP_off"}),
                new Wait(postrunTime-delayPostrun+1000),
                new Wait(2000),
                new UndoSetModelValues(),
                new TraceStop(),

            });

                Trigger lcUzOff = new Trigger(new SigConditon("LCI_UZ1", 0, TriggerConditionSignal.Equal)); //hardcode LC_Info signal
                Trigger lampOn = new Trigger(new SigConditon(indSig, indValue, TriggerConditionSignal.Equal));
                DeltaTime postrunDelay;
                if (delayPostrun == 0)
                {
                    postrunDelay = new DeltaTime(lcUzOff, lampOn, delayPostrun + "-" + (delayPostrun + toleranceMs));
                }
                else
                {
                    postrunDelay = new DeltaTime(lcUzOff, lampOn, delayPostrun - toleranceMs + "-" + (delayPostrun + toleranceMs));
                }


                _script.CanTraceAnalyser = new CanTraceAnalyser()
                {
                    LastFrameCheck = new LastFrameCheck(postrunTime - toleranceMs + "-" + (postrunTime + toleranceMs)),
                    Triggers = new ObservableCollection<TriggerBase>() { lcUzOff, lampOn },
                    DeltaTimes = new ObservableCollection<DeltaTime>() { postrunDelay },

                };

            }


            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
            _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
            _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

            return _script;
        }

        /// <summary>
        /// Get Test Script of postrun speed threshold check
        /// </summary>
        /// <returns></returns>
        public static TestScript GetTScriptPostrunSpdThresholdLowerCheck(int spdTh, int postrunTime, int delayPostrun, string doorsLink, string indSig, int indValue, string productType, int toleranceMs = 1000)
        {
            TestScript _script = new TestScript();
            _script.Name = @"RB_UNIVERSAL_01J.Postrun_Speed_ThresholdLowerCheck";
            _script.Purpose.ScalarValue = @"To check postrun time speed threshold, default value is 1 kph";
            _script.DoorsLink = doorsLink;
            _script.QcFolderPath = @"RBT_CANTx\Postrun";


            Dictionary<string, string> modelValuePstrSpd = new Dictionary<string, string>();



            modelValuePstrSpd.Add("v_Wheel_LF", ((spdTh - 1) / 3.6).ToString("F1"));
            modelValuePstrSpd.Add("v_Wheel_RF", ((spdTh - 1) / 3.6).ToString("F1"));
            modelValuePstrSpd.Add("v_Wheel_LR", ((spdTh - 1) / 3.6).ToString("F1"));
            modelValuePstrSpd.Add("v_Wheel_RR", ((spdTh - 1) / 3.6).ToString("F1"));


            if (productType == "ABS")
            {
                _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

                new TraceStart(),
                new EcuOn(),
                new Wait(5000),
                new SetModelValues(modelValuePstrSpd),
                new Wait(5000),
                new EcuOff(),
                new Wait(postrunTime+1000),
                new Wait(2000),
                new UndoSetModelValues(),
                new TraceStop(),

                 });

                _script.CanTraceAnalyser = new CanTraceAnalyser()
                {
                    LastFrameCheck = new LastFrameCheck(postrunTime - toleranceMs + "-" + (postrunTime + toleranceMs)),
                };

            }
            else
            {
                _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

                new TraceStart(),
                new EcuOn(),
                new Wait(5000),
                new SetModelValues(modelValuePstrSpd),
                new Wait(5000),
                new EcuOff(),
                new Wait(delayPostrun+3000),
                new ReadCanLamps(new ObservableCollection<string> { "ESP_off"}),
                new ReadCanSignals(new ObservableCollection<string> { "ESP_off"}),
                new Wait(postrunTime-delayPostrun+1000),
                new Wait(2000),
                new UndoSetModelValues(),
                new TraceStop(),

            });

                Trigger lcUzOff = new Trigger(new SigConditon("LCI_UZ1", 0, TriggerConditionSignal.Equal)); //hardcode LC_Info signal
                Trigger lampOn = new Trigger(new SigConditon(indSig, indValue, TriggerConditionSignal.Equal));
                DeltaTime postrunDelay;
                if (delayPostrun == 0)
                {
                    postrunDelay = new DeltaTime(lcUzOff, lampOn, delayPostrun + "-" + (delayPostrun + toleranceMs));
                }
                else
                {
                    postrunDelay = new DeltaTime(lcUzOff, lampOn, delayPostrun - toleranceMs + "-" + (delayPostrun + toleranceMs));
                }


                _script.CanTraceAnalyser = new CanTraceAnalyser()
                {
                    LastFrameCheck = new LastFrameCheck(postrunTime - toleranceMs + "-" + (postrunTime + toleranceMs)),
                    Triggers = new ObservableCollection<TriggerBase>() { lcUzOff, lampOn },
                    DeltaTimes = new ObservableCollection<DeltaTime>() { postrunDelay },

                };

            }


            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
            _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
            _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

            return _script;
        }

        #endregion Postrun


        #region Init
        /// <summary>
        /// Get Test Script if Ta timing check
        /// </summary>
        /// <returns></returns>
        public static TestScript GetTScriptTxTimingInit(int lowLimit, int highLimit, int lampInit, int lampInitTor, string doorsLink, ObservableCollection<NameValue> lampSigs)
        {
            TestScript _script = new TestScript();
            _script.Name = @"RB_UNIVERSAL_01J.InitTiming_FirstFrameAndLampInit";
            _script.Purpose.ScalarValue = @"To check the first frame transmit time and lamp init time after IG On";
            _script.DoorsLink = doorsLink;
            _script.QcFolderPath = @"RBT_CANTx\InitTiming";

            _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

            new TraceStart(),
            new Wait(2000),
            new EcuOn(),
            new Wait(lampInit+2000),
            new EcuOff(),
            new Wait(3000),
            new TraceStop(),

             });

            ObservableCollection<TriggerBase> tempTriggerCollection = new ObservableCollection<TriggerBase>();
            ObservableCollection<DeltaTime> tempDeltaCollection = new ObservableCollection<DeltaTime>();
            Trigger lcUzOn = new Trigger(new SigConditon("LCI_UZ1", 1, TriggerConditionSignal.Equal)); //hardcode LC_Info signal
            tempTriggerCollection.Add(lcUzOn);

            foreach (var sigLamp in lampSigs)
            {
                var tempLamp = new Trigger(new SigConditon(sigLamp.Name, sigLamp.Value, TriggerConditionSignal.Equal));
                tempTriggerCollection.Add(tempLamp);
                tempDeltaCollection.Add(new DeltaTime(lcUzOn, tempLamp, lampInit - lampInitTor + "-" + (lampInit + lampInitTor)));
            }

            _script.CanTraceAnalyser = new CanTraceAnalyser()
            {
                FirstFrameCheck = new FirstFrameCheck(lowLimit + "-" + highLimit),
                Triggers = tempTriggerCollection,
                DeltaTimes = tempDeltaCollection,

            };


            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
            _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
            _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

            return _script;
        }
        /// <summary>
        /// Tx signal init value check
        /// </summary>
        /// <param name="lowLimit"></param>
        /// <param name="highLimit"></param>
        /// <param name="lampInit"></param>
        /// <param name="lampInitTor"></param>
        /// <param name="doorsLink"></param>
        /// <param name="lampSigs"></param>
        /// <returns></returns>
        public static TestScript GetTScriptTxInitValues(string doorsLink, ObservableCollection<NameValue> sigs)
        {
            TestScript _script = new TestScript();
            _script.Name = @"RB_UNIVERSAL_01J.InitTiming_ReadInitialValues";
            _script.Purpose.ScalarValue = @"To check the initial values of all Tx signals";
            _script.DoorsLink = doorsLink;
            _script.QcFolderPath = @"RBT_CANTx\InitTiming";

            _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

            new TraceStart(),
            new Wait(2000),
            new EcuOn(),
            new Wait(2000),
            new EcuOff(),
            new Wait(1000),
            new TraceStop(),

             });

            ReadInitialValues readInit = new ReadInitialValues();
            readInit.SignalList.ValueList = new ObservableCollection<string>(sigs.Select(x => x.Name));  //Todo: The expected value of init signals need to provide in a smart ways
            readInit.CanOpState.ValueList = new ObservableCollection<string>() { "init" };

            _script.CanTraceAnalyser = new CanTraceAnalyser()
            {

                MeasurementPoint = readInit,

            };


            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
            _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
            _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

            return _script;
        }

        #endregion Init


        #region voltage
        /// <summary>
        /// Get Test Script of net undervoltage voltage set situations
        /// </summary>
        /// <returns></returns>
        public static TestScript GetTScriptNetUnderVoltageSet(double underVoltSet, string doorsLink, string productType, double spdLimit, double toleranceV = 0.5, double toleranceSpd = 1)
        {
            TestScript _script = new TestScript();
            _script.Name = @"RB_UNIVERSAL_01J.NetVolt_Threshold_NetUnderVoltage_Set";
            _script.Purpose.ScalarValue = @"To check the under voltage set threhold and speed limit";
            _script.DoorsLink = doorsLink;
            _script.QcFolderPath = @"RBT_CANTx\VoltageThreshold";

            Dictionary<string, string> modelValueUnderSet = new Dictionary<string, string>();
            modelValueUnderSet.Add("BatteryVoltage", (underVoltSet - toleranceV).ToString("F1"));



            Dictionary<string, string> modelValueUnderSetNo = new Dictionary<string, string>();
            modelValueUnderSetNo.Add("BatteryVoltage", (underVoltSet + toleranceV).ToString("F1"));
            if (spdLimit > 0)
            {

                modelValueUnderSetNo.Add("v_Wheel_LF", ((spdLimit + toleranceSpd) / 3.6).ToString("F1"));
                modelValueUnderSetNo.Add("v_Wheel_RF", ((spdLimit + toleranceSpd) / 3.6).ToString("F1"));
                modelValueUnderSetNo.Add("v_Wheel_LR", ((spdLimit + toleranceSpd) / 3.6).ToString("F1"));
                modelValueUnderSetNo.Add("v_Wheel_RR", ((spdLimit + toleranceSpd) / 3.6).ToString("F1"));
            }


            Dictionary<string, string> modelValueNormal = new Dictionary<string, string>();
            modelValueNormal.Add("BatteryVoltage", "12");


            _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

            new TraceStart(),
            new EcuOn(),
            new Wait(5000),
            new SetModelValues(modelValueUnderSetNo),
            new Wait(3000),
            new ReadCanLamps(new ObservableCollection<string> { "ABS_off"}),
            new ReadCanSignals(new ObservableCollection<string> { "ABS_off"}),
            new SetModelValues(modelValueUnderSet),
            new Wait(3000),
            //productType == "ABS"? new ReadCanLamps(new ObservableCollection<string> { "Full_system"}) : new ReadCanLamps(new ObservableCollection<string> { "ESP_off"}),
            //productType == "ABS"? new ReadCanSignals(new ObservableCollection<string> { "Full_system"}): new ReadCanSignals(new ObservableCollection<string> { "ESP_off"}),
            new ReadCanLamps(new ObservableCollection<string> { "ABS_off"}),
            new ReadCanSignals(new ObservableCollection<string> { "ABS_off"}),
            new Wait(3000),
            new SetModelValues(modelValueNormal),
            new Wait(3000),
            new ReadCanLamps(new ObservableCollection<string> { "Full_system"}),
            new ReadCanSignals(new ObservableCollection<string> { "Full_system"}),
            new Wait(3000),
            new EcuOff(),
            new TraceStop(),
            new UndoSetModelValues(),

            });

            if (productType == "Gen93")
            {
                _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Undervoltage" };
                _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Undervoltage" };
                _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "RBHydraulicSupplyUndervoltage" };
                _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "RBHydraulicSupplyUndervoltage" };
            }
            else
            {
                _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };
                _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };
                _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };
                _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage" };
            }


            _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
            _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

            return _script;
        }

        /// <summary>
        /// Get Test Script of net undervoltage voltage reset situations
        /// </summary>
        /// <returns></returns>
        public static TestScript GetTScriptNetUnderVoltageReset(double underVoltSet, double underVoltReset, string doorsLink, string productType, double spdLimit, double toleranceV = 0.5, double toleranceSpd = 1)
        {
            TestScript _script = new TestScript();
            _script.Name = @"RB_UNIVERSAL_01J.NetVolt_Threshold_NetUnderVoltage_Reset";
            _script.Purpose.ScalarValue = @"To check the under voltage reset threhold and speed limit";
            _script.DoorsLink = doorsLink;
            _script.QcFolderPath = @"RBT_CANTx\VoltageThreshold";

            Dictionary<string, string> modelValueUnderSet = new Dictionary<string, string>();
            modelValueUnderSet.Add("BatteryVoltage", (underVoltSet - toleranceV).ToString("F1"));
            if (spdLimit > 0)
            {
                modelValueUnderSet.Add("v_Wheel_LF", ((spdLimit + toleranceSpd) / 3.6).ToString("F1"));
                modelValueUnderSet.Add("v_Wheel_RF", ((spdLimit + toleranceSpd) / 3.6).ToString("F1"));
                modelValueUnderSet.Add("v_Wheel_LR", ((spdLimit + toleranceSpd) / 3.6).ToString("F1"));
                modelValueUnderSet.Add("v_Wheel_RR", ((spdLimit + toleranceSpd) / 3.6).ToString("F1"));

            }



            Dictionary<string, string> modelValueUnderSetNo = new Dictionary<string, string>();
            modelValueUnderSetNo.Add("BatteryVoltage", (underVoltSet + toleranceV).ToString("F1"));

            Dictionary<string, string> modelValueUnderReset = new Dictionary<string, string>();
            modelValueUnderReset.Add("BatteryVoltage", (underVoltReset + toleranceV).ToString("F1"));

            Dictionary<string, string> modelValueUnderResetNo = new Dictionary<string, string>();
            modelValueUnderResetNo.Add("BatteryVoltage", (underVoltReset - toleranceV).ToString("F1"));

            Dictionary<string, string> modelValueNormal = new Dictionary<string, string>();
            modelValueNormal.Add("BatteryVoltage", "12");

            _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

            new TraceStart(),
            new EcuOn(),
            new Wait(5000),
            new SetModelValues(modelValueUnderSet),
            new Wait(3000),
            new SetModelValues(modelValueUnderResetNo),
            new Wait(3000),
            //productType == "ABS"? new ReadCanLamps(new ObservableCollection<string> { "Full_system"}) : new ReadCanLamps(new ObservableCollection<string> { "ESP_off"}),
            //productType == "ABS"? new ReadCanSignals(new ObservableCollection<string> { "Full_system"}): new ReadCanSignals(new ObservableCollection<string> { "ESP_off"}),
            new ReadCanLamps(new ObservableCollection<string> { "Full_system"}),
            new ReadCanSignals(new ObservableCollection<string> { "Full_system"}),
            new Wait(3000),
            new SetModelValues(modelValueUnderReset),
            new Wait(3000),
            new ReadCanLamps(new ObservableCollection<string> { "Full_system"}),
            new ReadCanSignals(new ObservableCollection<string> { "Full_system"}),
            new Wait(3000),
            new EcuOff(),
            new TraceStop(),
            new UndoSetModelValues(),

            });

            if (productType == "Gen93")
            {
                _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Undervoltage" };
                _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Undervoltage" };
                _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "RBHydraulicSupplyUndervoltage", "RBApbMotorSupplyUndervoltage", "RBHydraulicSupplyHardUndervoltage", "RBApbButtonSupplyUndervolt" };
                _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "RBHydraulicSupplyUndervoltage", "RBApbMotorSupplyUndervoltage", "RBHydraulicSupplyHardUndervoltage", "RBApbButtonSupplyUndervolt" };
            }
            else
            {
                _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };
                _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Undervoltage" };
                _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "ApbMotorSupplyUndervoltage", "HydraulicSupplyHardUndervoltage" };
                _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "HydraulicSupplyUndervoltage", "ApbMotorSupplyUndervoltage", "HydraulicSupplyHardUndervoltage" };
            }

            _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
            _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

            return _script;
        }

        /// <summary>
        /// Get Test Script of net overvoltage voltage set situations
        /// </summary>
        /// <returns></returns>
        /// 
        public static TestScript GetTScriptNetOverVoltageSet(double overVoltSet, string doorsLink, string productType, double spdLimit, double toleranceV = 0.5, double toleranceSpd = 1)
        {
            TestScript _script = new TestScript();
            _script.Name = @"RB_UNIVERSAL_01J.NetVolt_Threshold_NetOverVoltage_Set";
            _script.Purpose.ScalarValue = @"To check the over voltage set threhold and speed limit";
            _script.DoorsLink = doorsLink;
            _script.QcFolderPath = @"RBT_CANTx\VoltageThreshold";

            Dictionary<string, string> modelValueOverSet = new Dictionary<string, string>();
            modelValueOverSet.Add("BatteryVoltage", (overVoltSet + toleranceV).ToString("F1"));

            Dictionary<string, string> modelValueOverSetNo = new Dictionary<string, string>();
            modelValueOverSetNo.Add("BatteryVoltage", (overVoltSet - toleranceV).ToString("F1"));
            if (spdLimit > 0)
            {
                modelValueOverSetNo.Add("v_Wheel_LF", ((spdLimit + toleranceSpd) / 3.6).ToString("F1"));
                modelValueOverSetNo.Add("v_Wheel_RF", ((spdLimit + toleranceSpd) / 3.6).ToString("F1"));
                modelValueOverSetNo.Add("v_Wheel_LR", ((spdLimit + toleranceSpd) / 3.6).ToString("F1"));
                modelValueOverSetNo.Add("v_Wheel_RR", ((spdLimit + toleranceSpd) / 3.6).ToString("F1"));

            }

            Dictionary<string, string> modelValueNormal = new Dictionary<string, string>();
            modelValueNormal.Add("BatteryVoltage", "12");

            _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

            new TraceStart(),
            new EcuOn(),
            new Wait(5000),
            new SetModelValues(modelValueOverSetNo),
            new Wait(3000),
            //new ReadCanLamps(new ObservableCollection<string> { "ABS_off"}),
            //new ReadCanSignals(new ObservableCollection<string> { "ABS_off"}),
            //new Wait(3000),
            new SetModelValues(modelValueOverSet),
            //productType == "ABS"? new ReadCanLamps(new ObservableCollection<string> { "ABS_off"}) : new ReadCanLamps(new ObservableCollection<string> { "ABS_off"}),
            //productType == "ABS"? new ReadCanSignals(new ObservableCollection<string> { "ABS_off"}): new ReadCanSignals(new ObservableCollection<string> { "ABS_off"}),
            new Wait(3000),
            new ReadCanLamps(new ObservableCollection<string> { "ABS_off"}),
            new ReadCanSignals(new ObservableCollection<string> { "ABS_off"}),
            new Wait(3000),
            new SetModelValues(modelValueNormal),
            new Wait(3000),
            new ReadCanLamps(new ObservableCollection<string> { "Full_system"}),
            new ReadCanSignals(new ObservableCollection<string> { "Full_system"}),
            new Wait(3000),
            new EcuOff(),
            new TraceStop(),
            new UndoSetModelValues(),

            });
            if (productType == "Gen93")
            {
                _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Overvoltage" };
                _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Overvoltage" };

                _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "RBSupplyOvervoltage", "RBApbMotorSupplyOvervoltage" };
                _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "RBSupplyOvervoltage", "RBApbMotorSupplyOvervoltage" };
            }
            else
            {
                _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
                _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

                _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
                _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
            }

            _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
            _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

            return _script;
        }

        /// <summary>
        /// Get Test Script of net overvoltage voltage reset situations
        /// </summary>
        /// <returns></returns>
        public static TestScript GetTScriptNetOverVoltageReset(double overVoltSet, double overVoltReset, string doorsLink, string productType, double spdLimit, double toleranceV = 0.5, double toleranceSpd = 1)
        {
            TestScript _script = new TestScript();
            _script.Name = @"RB_UNIVERSAL_01J.NetVolt_Threshold_NetOverVoltage_Reset";
            _script.Purpose.ScalarValue = @"To check the over voltage reset threhold and speed limit";
            _script.DoorsLink = doorsLink;
            _script.QcFolderPath = @"RBT_CANTx\VoltageThreshold";
            /*
            Dictionary<string, string> modelValueUnderSet = new Dictionary<string, string>();
            modelValueUnderSet.Add("BatteryVoltage", (underVoltSet - toleranceV).ToString("F1"));
            
            Dictionary<string, string> modelValueUnderSetNo = new Dictionary<string, string>();
            modelValueUnderSetNo.Add("BatteryVoltage", (underVoltSet + toleranceV).ToString("F1"));

            Dictionary<string, string> modelValueUnderReset = new Dictionary<string, string>();
            modelValueUnderReset.Add("BatteryVoltage", (underVoltReset + toleranceV).ToString("F1"));

            Dictionary<string, string> modelValueUnderResetNo = new Dictionary<string, string>();
            modelValueUnderResetNo.Add("BatteryVoltage", (underVoltReset - toleranceV).ToString("F1"));
            */

            Dictionary<string, string> modelValueOverSet = new Dictionary<string, string>();
            modelValueOverSet.Add("BatteryVoltage", (overVoltSet + toleranceV).ToString("F1"));

            if (spdLimit > 0)
            {
                modelValueOverSet.Add("v_Wheel_LF", ((spdLimit + toleranceSpd) / 3.6).ToString("F1"));
                modelValueOverSet.Add("v_Wheel_RF", ((spdLimit + toleranceSpd) / 3.6).ToString("F1"));
                modelValueOverSet.Add("v_Wheel_LR", ((spdLimit + toleranceSpd) / 3.6).ToString("F1"));
                modelValueOverSet.Add("v_Wheel_RR", ((spdLimit + toleranceSpd) / 3.6).ToString("F1"));
            }


            Dictionary<string, string> modelValueOverSetNo = new Dictionary<string, string>();
            modelValueOverSetNo.Add("BatteryVoltage", (overVoltSet - toleranceV).ToString("F1"));


            Dictionary<string, string> modelValueOverReset = new Dictionary<string, string>();
            modelValueOverReset.Add("BatteryVoltage", (overVoltReset + toleranceV).ToString("F1"));

            Dictionary<string, string> modelValueOverResetNo = new Dictionary<string, string>();
            modelValueOverResetNo.Add("BatteryVoltage", (overVoltReset - toleranceV).ToString("F1"));


            Dictionary<string, string> modelValueNormal = new Dictionary<string, string>();
            modelValueNormal.Add("BatteryVoltage", "12");

            _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

            new TraceStart(),
            new EcuOn(),
            new Wait(5000),
            new SetModelValues(modelValueOverSet),
            new Wait(3000),
            new SetModelValues(modelValueOverResetNo),
            //productType == "ABS"? new ReadCanLamps(new ObservableCollection<string> { "Full_system"}) : new ReadCanLamps(new ObservableCollection<string> { "ESP_off"}),
            //productType == "ABS"? new ReadCanSignals(new ObservableCollection<string> { "Full_system"}): new ReadCanSignals(new ObservableCollection<string> { "ESP_off"}),
            new Wait(3000),
             new ReadCanLamps(new ObservableCollection<string> { "Full_system"}),
            new ReadCanSignals(new ObservableCollection<string> { "Full_system"}),
            new Wait(3000),
            new SetModelValues(modelValueOverReset),
            new Wait(3000),
            new ReadCanLamps(new ObservableCollection<string> { "Full_system"}),
            new ReadCanSignals(new ObservableCollection<string> { "Full_system"}),
            new Wait(3000),
            new EcuOff(),
            new TraceStop(),
            new UndoSetModelValues(),

            });

            if (productType == "Gen93")
            {
                _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Overvoltage" };
                _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBNET_Overvoltage" };

                _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "RBSupplyOvervoltage", "RBApbMotorSupplyOvervoltage" };
                _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "RBSupplyOvervoltage", "RBApbMotorSupplyOvervoltage" };
            }

            else
            {
                _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };
                _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NET_Overvoltage" };

                _script.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
                _script.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "SupplyOvervoltage", "ApbMotorSupplyOvervoltage" };
            }
            _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
            _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

            return _script;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="underVoltSet"></param>
        /// <param name="doorsLink"></param>
        /// <param name="productType"></param>
        /// <param name="spdLimit"></param>
        /// <param name="toleranceV"></param>
        /// <param name="toleranceSpd"></param>
        /// <returns></returns>
        public static TestScript GetTScriptNetUnderVoltageSpdNotReached(double underVoltSet, string doorsLink, string productType, double spdLimit, double toleranceV = 0.5, double toleranceSpd = 1)
        {
            TestScript _script = new TestScript();
            _script.Name = @"RB_UNIVERSAL_01J.NetVolt_NetUnderVoltage_SpeedNotReached";
            _script.Purpose.ScalarValue = @"To check the over voltage reset threhold and speed limit";
            _script.DoorsLink = doorsLink;
            _script.QcFolderPath = @"RBT_CANTx\VoltageThreshold";


            Dictionary<string, string> modelValueUnderSet = new Dictionary<string, string>();
            modelValueUnderSet.Add("BatteryVoltage", (underVoltSet - toleranceV).ToString("F1"));
            modelValueUnderSet.Add("v_Wheel_LF", ((spdLimit - toleranceSpd) / 3.6).ToString("F1"));
            modelValueUnderSet.Add("v_Wheel_RF", ((spdLimit - toleranceSpd) / 3.6).ToString("F1"));
            modelValueUnderSet.Add("v_Wheel_LR", ((spdLimit - toleranceSpd) / 3.6).ToString("F1"));
            modelValueUnderSet.Add("v_Wheel_RR", ((spdLimit - toleranceSpd) / 3.6).ToString("F1"));


            Dictionary<string, string> modelValueUnderSetNo = new Dictionary<string, string>();
            modelValueUnderSetNo.Add("BatteryVoltage", (underVoltSet + toleranceV).ToString("F1"));


            Dictionary<string, string> modelValueNormal = new Dictionary<string, string>();
            modelValueNormal.Add("BatteryVoltage", "12");


            _script.TestSequence = new TestSequence(new ObservableCollection<Keyword>() {

            new TraceStart(),
            new EcuOn(),
            new Wait(5000),
            new SetModelValues(modelValueUnderSet),
            new Wait(3000),
            new ReadCanLamps(new ObservableCollection<string> { "Full_system"}),
            new ReadCanSignals(new ObservableCollection<string> { "Full_system"}),
            new Wait(3000),
            new SetModelValues(modelValueNormal),
            new Wait(3000),
            new ReadCanLamps(new ObservableCollection<string> { "Full_system"}),
            new ReadCanSignals(new ObservableCollection<string> { "Full_system"}),
            new Wait(3000),
            new EcuOff(),
            new TraceStop(),
            new UndoSetModelValues(),

            });

            _script.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };
            _script.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "NoFault" };

            _script.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
            _script.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

            return _script;
        }
        #endregion
    }
}