using RBT.Universal;
using RBT.Universal.CanEvalParameters;
using RBT.Universal.CanEvalParameters.MeasurementPoints;
using RBT.Universal.Keywords;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace OpStates
{

    public class ProductPar
    {
        private static string _productType = "ESP";

        public static string ProductType
        {
            get { return _productType; }
            set { _productType = value; }

        }
    }
    #region base opstate types, concrete opstate inherit here
    /// <summary>
    /// for all kinds of speed check
    /// </summary>
    public class OpSpeedSetModelValues : OpState
    {

        protected OpSpeedSetModelValues()
        {
            Category = "BaseOperationStateType";
            Description = "This operation state is a base opstate type to manipulate speed";
            Parameters.Add(new CANTxParameter("TargetSpeed_mps", "Target speed to set constant in mps,use ; to seperate values for FL,FR,RL,RR", "1", typeof(int)));
            Parameters.Add(new CANTxParameter("Wheel2Set", "Input 'All' for 4 wheels, single wheel eg.'v_Wheel_LF', multiple use ; to seperate", "v_Wheel_LF", typeof(string)));
            Parameters.Add(new CANTxParameter("StartTime", "Input 'All' for 4 wheels, single wheel eg.'v_Wheel_LF', multiple use ; to seperate", "8000", typeof(int)));

        }
        public override string Name
        {
            get { return "SpeedSetModelValues"; }
        }



        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverTime(8000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }


        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {
            string speed = paraSet.SingleOrDefault(x => x.Name == "TargetSpeed_mps").Value;
            string whl2set = paraSet.SingleOrDefault(x => x.Name == "Wheel2Set").Value;

            //wheel speed calculation
            if (paraSet.SingleOrDefault(x => x.Name == "SignalUnit") != null && paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage") != null
                && paraSet.SingleOrDefault(x => x.Name == "Wheel") != null && paraSet.SingleOrDefault(x => x.Name == "Factor").Value != null && paraSet.SingleOrDefault(x => x.Name == "Offset").Value != null)
            {
                double factor = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "Factor").Value);
                double offset = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "Offset").Value);

                if (whl2set == "All" || Regex.IsMatch(whl2set, paraSet.SingleOrDefault(x => x.Name == "Wheel").Value))
                {


                    if (Regex.IsMatch(paraSet.SingleOrDefault(x => x.Name == "SignalUnit").Value, "kph"))
                    {


                        OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = (int)(((Convert.ToDouble(speed)) * 3.6 - offset) / factor) + "%" + paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value;

                    }
                    else if (Regex.IsMatch(paraSet.SingleOrDefault(x => x.Name == "SignalUnit").Value, "mps"))
                    {

                        OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = (int)(((Convert.ToDouble(speed)) - offset) / factor) + "%" + paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value;
                    }
                }
                //vehicle speed calculation
            }
            else if (paraSet.SingleOrDefault(x => x.Name == "SignalUnit") != null && paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage") != null
              && paraSet.SingleOrDefault(x => x.Name == "DrivenType") != null && paraSet.SingleOrDefault(x => x.Name == "Factor").Value != null && paraSet.SingleOrDefault(x => x.Name == "Offset").Value != null)
            {

                double factor = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "Factor").Value);
                double offset = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "Offset").Value);
                string drivenType = paraSet.SingleOrDefault(x => x.Name == "DrivenType").Value;

                // string speeds = paraSet.SingleOrDefault(x => x.Name == "TargetSpeed_mps").Value;
                string[] speeds = paraSet.SingleOrDefault(x => x.Name == "TargetSpeed_mps").Value.Split(new char[] { ';' });
                double vehSped = 0;


                if (drivenType == "FWD")
                {
                    switch (speeds.Length)
                    {
                        case 1:
                            vehSped = Convert.ToDouble(speeds[0]);
                            break;
                        case 4:
                            vehSped = (Convert.ToDouble(speeds[0]) + Convert.ToDouble(speeds[1])) / 2;
                            break;

                    }
                    // vehSped = (Convert.ToDouble(speeds[0]) + Convert.ToDouble(speeds[1])) / 2;
                }

                else if (drivenType == "RWD")
                {
                    switch (speeds.Length)
                    {
                        case 1:
                            vehSped = Convert.ToDouble(speeds[0]);
                            break;
                        case 4:
                            vehSped = (Convert.ToDouble(speeds[0]) + Convert.ToDouble(speeds[1])) / 2;
                            break;
                    }
                    // vehSped = (Convert.ToDouble(speeds[2]) + Convert.ToDouble(speeds[3])) / 2;
                }


                if (Regex.IsMatch(paraSet.SingleOrDefault(x => x.Name == "SignalUnit").Value, "kph"))
                {

                    //为什么需要除以 factor,只是计算车速.
                    OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = (int)(((Convert.ToDouble(vehSped)) * 3.6 - offset) / factor) + "%" + paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value;

                }
                else if (Regex.IsMatch(paraSet.SingleOrDefault(x => x.Name == "SignalUnit").Value, "mps"))
                {

                    OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = (int)(((Convert.ToDouble(vehSped)) - offset) / factor) + "%" + paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value;
                }
            }
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {

            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            string whl2set = paraSet.SingleOrDefault(x => x.Name == "Wheel2Set").Value;

            #region Add Mandatory and optional
            //Add RB_mandatory_faults, CU_mandatory_faults, RB_optional_faults ,CU_optional_faults -> 05/21/2018: Xia Jack, Bug 329
            if (whl2set.ToUpper() == "ALL")
            {
                //Assign fault evaluation
                //testScript.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "Wss_MonRange_FL", "Wss_MonRange_FR", "Wss_MonRange_RL", "Wss_MonRange_RR" };
                //testScript.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "Wss_MonRange_FL", "Wss_MonRange_FR", "Wss_MonRange_RL", "Wss_MonRange_RR" };
                testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "Wss_MonRange_FL", "Wss_MonRange_FR", "Wss_MonRange_RL", "Wss_MonRange_RR", "Wss_SignalLost_FL", "Wss_SignalLost_FR", "Wss_SignalLost_RL", "Wss_SignalLost_RR", "Wss_MoreThanOneSuspected", "Wss_MonGenericTempFail" };
                testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "Wss_MonRange_FL", "Wss_MonRange_FR", "Wss_MonRange_RL", "Wss_MonRange_RR", "Wss_SignalLost_FL", "Wss_SignalLost_FR", "Wss_SignalLost_RL", "Wss_SignalLost_RR", "Wss_MoreThanOneSuspected", "Wss_MonGenericTempFail" };
            }
            #endregion
            //redecorate the test script
            testScript.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
            testScript.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

            //adjust the unit convertion
            AssignParameterValues(paraSet);

            //Parameter check
            if (paraSet.SingleOrDefault(x => x.Name == "TargetSpeed_mps") == null || paraSet.SingleOrDefault(x => x.Name == "Wheel2Set") == null)
            {
                throw new FormatException(string.Format("Error when parmeterizng operation state {0}: TargetSpeed is not provided", Name));

            }
            else
            {
                string speedsStr = paraSet.SingleOrDefault(x => x.Name == "TargetSpeed_mps").Value;
                string wheels = paraSet.SingleOrDefault(x => x.Name == "Wheel2Set").Value;


                string[] speed = speedsStr.Split(Char.Parse(";"));
                string[] wheels2Set = wheels.Split(Char.Parse(";"));

                SetModelValues setModelValue;

                Dictionary<string, string> dict2Set = new Dictionary<string, string>();

                if (speed.Count() == wheels2Set.Count())
                {
                    if (Regex.IsMatch(wheels, "All", RegexOptions.IgnoreCase))
                    {
                        dict2Set.Add("v_Wheel_LF", speed[0]);
                        dict2Set.Add("v_Wheel_RF", speed[0]);
                        dict2Set.Add("v_Wheel_LR", speed[0]);
                        dict2Set.Add("v_Wheel_RR", speed[0]);
                    }
                    else
                    {
                        for (int i = 0; i < wheels2Set.Count(); i++)
                        {
                            dict2Set.Add(wheels2Set[i], speed[i]);
                        }
                    }

                }
                else
                {
                    if (speed.Count() == 1)
                    {
                        for (int i = 0; i < wheels2Set.Count(); i++)
                        {
                            dict2Set.Add(wheels2Set[i], speed[0]);

                        }

                    }

                }


                setModelValue = new SetModelValues(dict2Set);
                #region Gear R
                Dictionary<string, string> gearModelValue = new Dictionary<string, string>();
                if (Name.Equals("VehicleSpeedTypicalReverseGear"))
                {
                    gearModelValue = new Dictionary<string, string>()
                    {
                        { "model_gear","R"},
                    };
                }
                else
                {
                    gearModelValue = new Dictionary<string, string>()
                    {
                        { "model_gear","D"},
                    };
                }
                #endregion Gear R

                // in order to avoid wss signal lost error threw
                Dictionary<string, string> modelValueNormal = new Dictionary<string, string>
                {
                    { "BatteryVoltage", "0" }
                };

                return new TestSequence(new ObservableCollection<Keyword>()
                {
                    new TraceStart(),
                    new EcuOn(),
                    new Wait(5000),
                    new MM6Start("9","Normal"),
                    new SetModelValues(gearModelValue),
                    new Wait(5000),
                    setModelValue,
                    new Wait(5000),
                    new MM6Stop(),
                    new TraceStop(),
                    new EcuOff(),
                    new Wait(5000),
                    new SetModelValues(modelValueNormal),
                    new Wait(5000),
                    new UndoSetModelValues(),
                 });
            }
            //Parameterize

        }
    }

    /// <summary>
    /// for all kinds of Ax value check
    /// </summary>
    public class OpAxSetModelValues : OpState
    {


        protected OpAxSetModelValues()
        {
            Category = "BaseOperationStateType";
            Description = "This operation state is a base opstate type to manipulate Ax value";
            Parameters.Add(new CANTxParameter("TargetAx_ms2", "Target Ax to set constant in ms2", "1", typeof(int)));


        }
        public override string Name
        {
            get { return "AxSetModelValues"; }
        }



        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverTime(8000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {


            string speed = paraSet.SingleOrDefault(x => x.Name == "TargetAx_ms2").Value;


            if (paraSet.SingleOrDefault(x => x.Name == "SignalUnit") != null && paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage") != null) //to indicate it is a value check
            {
                double factor = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "Factor").Value);
                double offset = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "Offset").Value);
                if (Regex.IsMatch(paraSet.SingleOrDefault(x => x.Name == "SignalUnit").Value, "g"))
                {


                    OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = (int)((Convert.ToDouble(speed) / 10 - offset) / factor) + "%" + paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value;

                }
                else if (Regex.IsMatch(paraSet.SingleOrDefault(x => x.Name == "SignalUnit").Value, "ms2"))
                {

                    OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = (int)((Convert.ToDouble(speed) - offset) / factor) + "%" + paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value;
                }

            }


        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {

            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);

            //redecorate the test script
            testScript.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
            testScript.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

            //adjust the unit convertion
            AssignParameterValues(paraSet);

            //Parameter check
            if (paraSet.SingleOrDefault(x => x.Name == "TargetAx_ms2") == null)
            {
                throw new FormatException(string.Format("Error when parmeterizng operation state {0}: TargetAx_ms2 is not provided", Name));

            }
            else
            {
                string AxStr = paraSet.SingleOrDefault(x => x.Name == "TargetAx_ms2").Value;

                Dictionary<string, string> dict2Set = new Dictionary<string, string>() { { "ax", AxStr } };

                SetModelValues setModelValue = new SetModelValues(dict2Set);

                // EvalECUSignals evalECUSignals = new EvalECUSignals(dict2Set);


                // Bug:338 -> Xia jack fix 05/21/2018
                if (ProductPar.ProductType == "ESP" || ProductPar.ProductType == "ABS" || ProductPar.ProductType == "APBMi5064" || ProductPar.ProductType == "APBMi5065")
                {
                    testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "Axs_MonRange" };
                    testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "Axs_MonRange" };
                }
                else
                {
                    testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "A1A2Range" };
                    testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "A1A2Range" };
                }

                return new TestSequence(new ObservableCollection<Keyword>()
                {
                    new TraceStart(),
                    new EcuOn(),
                    new Wait(5000),
                    new MM6Start("9","Normal"),
                    setModelValue,
                    //evalECUSignals,
                    new Wait(5000),
                    new MM6Stop(),
                    new TraceStop(),
                    new UndoSetModelValues(),
                    new EcuOff(),
                 });
            }
            //Parameterize

        }
    }

    /// <summary>
    /// For all kinds of Ax offset value check
    /// </summary>
    public class OpAxOffsetSetModelValues : OpState
    {


        protected OpAxOffsetSetModelValues()
        {
            Category = "BaseOperationStateType";
            Description = "This operation state is a base opstate type to manipulate Ax offset value";
            Parameters.Add(new CANTxParameter("TargetAx_ms2", "Target Ax to set constant in ms2", "1", typeof(int)));


        }
        public override string Name
        {
            get { return "AxOffsetSetModelValues"; }
        }



        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverTime(15000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {


            string speed = paraSet.SingleOrDefault(x => x.Name == "TargetAx_ms2").Value;


            if (paraSet.SingleOrDefault(x => x.Name == "SignalUnit") != null && paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage") != null) //to indicate it is a value check
            {
                double factor = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "Factor").Value);
                double offset = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "Offset").Value);
                if (Regex.IsMatch(paraSet.SingleOrDefault(x => x.Name == "SignalUnit").Value, "g"))
                {


                    OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = (int)((Convert.ToDouble(speed) / 10 - offset) / factor) + "%" + paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value;

                }
                else if (Regex.IsMatch(paraSet.SingleOrDefault(x => x.Name == "SignalUnit").Value, "ms2"))
                {

                    OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = (int)((Convert.ToDouble(speed) - offset) / factor) + "%" + paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value;
                }

            }


        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {

            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);

            //redecorate the test script
            testScript.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
            testScript.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

            //adjust the unit convertion
            AssignParameterValues(paraSet);

            //Parameter check
            if (paraSet.SingleOrDefault(x => x.Name == "TargetAx_ms2") == null)
            {
                throw new FormatException(string.Format("Error when parmeterizng operation state {0}: TargetAx_ms2 is not provided", Name));

            }
            else
            {
                string AxStr = paraSet.SingleOrDefault(x => x.Name == "TargetAx_ms2").Value;

                Dictionary<string, string> dict2Set = new Dictionary<string, string>() { { "ax", AxStr } };

                SetModelValues setModelValue = new SetModelValues(dict2Set);

                if (ProductPar.ProductType == "ESP" || ProductPar.ProductType == "ABS" || ProductPar.ProductType == "APBMi5064" || ProductPar.ProductType == "APBMi5065")
                {
                    testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "Axs_MonRange" };
                    testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "Axs_MonRange" };
                }
                else
                {
                    testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "A1A2Range" };
                    testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "A1A2Range" };
                }
                ObservableCollection<string> sendRequest = new ObservableCollection<string>();
                ObservableCollection<string> Response = new ObservableCollection<string>();
                //Add sending request command and response code
                if (double.Parse(AxStr) == 0.00)
                {
                    sendRequest = new ObservableCollection<string>() { "3D 03 EA 0F A8 00 00 07 D0" };

                    Response = new ObservableCollection<string>() { "7D 03 EA 0F A8" };
                }

                else if (double.Parse(AxStr) == 1.00)
                {
                    sendRequest = new ObservableCollection<string>() { "3D 03 EA 0F A8 04 00 07 D0" };

                    Response = new ObservableCollection<string>() { "7D 03 EA 0F A8" };
                }
                else if (double.Parse(AxStr) == 2.00)
                {
                    sendRequest = new ObservableCollection<string>() { "3D 03 EA 0F A8 08 00 07 D0" };

                    Response = new ObservableCollection<string>() { "7D 03 EA 0F A8" };
                }
                else if (double.Parse(AxStr) == 3.00)
                {
                    sendRequest = new ObservableCollection<string>() { "3D 03 EA 0F A8 0C 00 07 D0" };

                    Response = new ObservableCollection<string>() { "7D 03 EA 0F A8" };
                }

                else if (double.Parse(AxStr) == -1.00)
                {
                    sendRequest = new ObservableCollection<string>() { "3D 03 EA 0F A8 FB FF 07 D0" };

                    Response = new ObservableCollection<string>() { "7D 03 EA 0F A8" };
                }

                else if (double.Parse(AxStr) == -2.00)
                {
                    sendRequest = new ObservableCollection<string>() { "3D 03 EA 0F A8 F7 FF 07 D0" };

                    Response = new ObservableCollection<string>() { "7D 03 EA 0F A8" };
                }

                else if (double.Parse(AxStr) == -3.00)
                {
                    sendRequest = new ObservableCollection<string>() { "3D 03 EA 0F A8 F3 FF 07 D0" };

                    Response = new ObservableCollection<string>() { "7D 03 EA 0F A8" };
                }

                return new TestSequence(new ObservableCollection<Keyword>()
                {
                    new TraceStart(),
                    new EcuOn(),
                    new Wait(5000),
                    new MM6Start("9","Normal"),
                    new StartDiagComRB(),
                    new SendRequest(sendRequest,Response),
                    //setModelValue,
                    new Wait(5000),
                    new EcuOff(),
                    new Wait(3000),
                    new EcuOn(),
                    new Wait(5000),
                    new MM6Stop(),
                    new TraceStop(),
                    new Wait(2000),
                    new StopDiagComRB(),
                    //new UndoSetModelValues(),
                    new EcuOff(),
                 });
            }
        }
    }

    /// <summary>
    /// For all kinids of Ay offset value check
    /// </summary>
    public class OpAyOffsetSetModelValues : OpState
    {


        protected OpAyOffsetSetModelValues()
        {
            Category = "BaseOperationStateType";
            Description = "This operation state is a base opstate type to manipulate Ay offset value";
            Parameters.Add(new CANTxParameter("TargetAx_ms2", "Target Ay to set constant in ms2", "1", typeof(int)));


        }
        public override string Name
        {
            get { return "AyOffsetSetModelValues"; }
        }



        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverTime(15000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {


            string speed = paraSet.SingleOrDefault(x => x.Name == "TargetAx_ms2").Value;


            if (paraSet.SingleOrDefault(x => x.Name == "SignalUnit") != null && paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage") != null) //to indicate it is a value check
            {
                double factor = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "Factor").Value);
                double offset = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "Offset").Value);
                if (Regex.IsMatch(paraSet.SingleOrDefault(x => x.Name == "SignalUnit").Value, "g"))
                {


                    OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = (int)((Convert.ToDouble(speed) / 10 - offset) / factor) + "%" + paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value;

                }
                else if (Regex.IsMatch(paraSet.SingleOrDefault(x => x.Name == "SignalUnit").Value, "ms2"))
                {

                    OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = (int)((Convert.ToDouble(speed) - offset) / factor) + "%" + paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value;
                }

            }


        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {

            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);

            //redecorate the test script
            testScript.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
            testScript.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

            //adjust the unit convertion
            AssignParameterValues(paraSet);

            //Parameter check
            if (paraSet.SingleOrDefault(x => x.Name == "TargetAx_ms2") == null)
            {
                throw new FormatException(string.Format("Error when parmeterizng operation state {0}: TargetAx_ms2 is not provided", Name));

            }
            else
            {
                string AxStr = paraSet.SingleOrDefault(x => x.Name == "TargetAx_ms2").Value;

                Dictionary<string, string> dict2Set = new Dictionary<string, string>() { { "ax", AxStr } };

                SetModelValues setModelValue = new SetModelValues(dict2Set);

                if (ProductPar.ProductType == "ESP" || ProductPar.ProductType == "ABS" || ProductPar.ProductType == "APBMi5064" || ProductPar.ProductType == "APBMi5065")
                {
                    testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "Axs_MonRange" };
                    testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "Axs_MonRange" };
                }
                else
                {
                    testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "A1A2Range" };
                    testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "A1A2Range" };
                }

                //Add sending request command and response code
                ObservableCollection<string> sendRequest = new ObservableCollection<string>();
                ObservableCollection<string> Response = new ObservableCollection<string>();
                if (double.Parse(AxStr) == -2.00)
                {
                    sendRequest = new ObservableCollection<string>() { "3D 03 EA 0F A9 00 80" };

                    Response = new ObservableCollection<string>() { "7D 03 EA 0F A9" };

                }
                else if (double.Parse(AxStr) == 0.00)
                {
                    sendRequest = new ObservableCollection<string>() { "3D 03 EA 0F A9 00 00" };

                    Response = new ObservableCollection<string>() { "7D 03 EA 0F A9" };
                }

                else if (double.Parse(AxStr) == -1.00)
                {
                    sendRequest = new ObservableCollection<string>() { "3D 03 EA 0F A9 00 40" };

                    Response = new ObservableCollection<string>() { "7D 03 EA 0F A9" };

                }

                else if (double.Parse(AxStr) == -3.00)
                {
                    sendRequest = new ObservableCollection<string>() { "3D 03 EA 0F A9 00 C0" };

                    Response = new ObservableCollection<string>() { "7D 03 EA 0F A9" };

                }

                else if (double.Parse(AxStr) == 1.00)
                {
                    sendRequest = new ObservableCollection<string>() { "3D 03 EA 0F A9 FF BF" };

                    Response = new ObservableCollection<string>() { "7D 03 EA 0F A9" };

                }

                else if (double.Parse(AxStr) == 2.00)
                {
                    sendRequest = new ObservableCollection<string>() { "3D 03 EA 0F A9 FF 7F" };

                    Response = new ObservableCollection<string>() { "7D 03 EA 0F A9" };

                }


                else if (double.Parse(AxStr) == 3.00)
                {
                    sendRequest = new ObservableCollection<string>() { "3D 03 EA 0F A9 FF 3F" };

                    Response = new ObservableCollection<string>() { "7D 03 EA 0F A9" };

                }

                return new TestSequence(new ObservableCollection<Keyword>()
                {
                  new TraceStart(),
                    new EcuOn(),
                    new Wait(5000),
                    new MM6Start("9","Normal"),
                    new StartDiagComRB(),
                    new SendRequest(sendRequest,Response),
                    //setModelValue,
                    new Wait(5000),
                    new EcuOff(),
                    new Wait(3000),
                    new EcuOn(),
                    new Wait(5000),
                    new TraceStop(),
                    new MM6Stop(),
                    new Wait(2000),
                    new StopDiagComRB(),
                    //new UndoSetModelValues(),
                    new EcuOff(),
                 });
            }
        }
    }


    public class OpOffsetYawrateSetModelValues : OpState
    {


        protected OpOffsetYawrateSetModelValues()
        {
            Category = "BaseOperationStateType";
            Description = "This operation state is a base opstate type to manipulate yawrate offset value";
            Parameters.Add(new CANTxParameter("TargetYawrate_dps", "Target yawrate offset to set constant in degree per second", "1", typeof(int)));


        }
        public override string Name
        {
            get { return "YawrateOffSetModelValues"; }
        }



        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverTime(15000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {


            string speed = paraSet.SingleOrDefault(x => x.Name == "TargetYawrate_dps").Value;


            if (paraSet.SingleOrDefault(x => x.Name == "SignalUnit") != null && paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage") != null) //to indicate it is a value check
            {
                double factor = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "Factor").Value);
                double offset = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "Offset").Value);

                if (Regex.IsMatch(paraSet.SingleOrDefault(x => x.Name == "SignalUnit").Value, "rps"))
                {


                    OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = (int)((Convert.ToDouble(speed) / 57.3 - offset) / factor) + "%" + paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value;

                }
                else if (Regex.IsMatch(paraSet.SingleOrDefault(x => x.Name == "SignalUnit").Value, "dps"))
                {

                    OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = (int)((Convert.ToDouble(speed) - offset) / factor) + "%" + paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value;
                }

            }
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {

            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);

            //redecorate the test script
            testScript.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
            testScript.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

            //adjust the unit convertion
            AssignParameterValues(paraSet);

            //Parameter check
            if (paraSet.SingleOrDefault(x => x.Name == "TargetYawrate_dps") == null)
            {
                throw new FormatException(string.Format("Error when parmeterizng operation state {0}: TargetAy_ms2 is not provided", Name));

            }
            else
            {
                string yawStr = paraSet.SingleOrDefault(x => x.Name == "TargetYawrate_dps").Value;

                Dictionary<string, string> dict2Set = new Dictionary<string, string>() { { "yawRate", yawStr } };
                SetModelValues setModelValue = new SetModelValues(dict2Set);

                testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "YrsPlaus" };
                testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "YrsPlaus" };
                ObservableCollection<string> sendRequest = new ObservableCollection<string>();
                ObservableCollection<string> Response = new ObservableCollection<string>();
                if (double.Parse(yawStr) == 0.00)
                {
                    sendRequest = new ObservableCollection<string>() { "3D 03 EA 0F AB 00 00" };

                    Response = new ObservableCollection<string>() { "7D 03 EA 0F AB" };
                }

                else if (double.Parse(yawStr) == 0.05)
                {
                    sendRequest = new ObservableCollection<string>() { "3D 03 EA 0F AB FF CB" };

                    Response = new ObservableCollection<string>() { "7D 03 EA 0F AB" };
                }
                else if (double.Parse(yawStr) == 0.09)
                {
                    sendRequest = new ObservableCollection<string>() { "3D 03 EA 0F AB FF A2" };

                    Response = new ObservableCollection<string>() { "7D 03 EA 0F AB" };
                }

                else if (double.Parse(yawStr) == 0.10)
                {
                    sendRequest = new ObservableCollection<string>() { "3D 03 EA 0F AB FF 98" };

                    Response = new ObservableCollection<string>() { "7D 03 EA 0F AB" };
                }

                else if (double.Parse(yawStr) == -0.05)
                {
                    sendRequest = new ObservableCollection<string>() { "3D 03 EA 0F AB 00 33" };

                    Response = new ObservableCollection<string>() { "7D 03 EA 0F AB" };
                }

                else if (double.Parse(yawStr) == -0.09)
                {
                    sendRequest = new ObservableCollection<string>() { "3D 03 EA 0F AB 00 5C" };

                    Response = new ObservableCollection<string>() { "7D 03 EA 0F AB" };
                }

                else if (double.Parse(yawStr) == -0.10)
                {
                    sendRequest = new ObservableCollection<string>() { "3D 03 EA 0F AB 00 66" };

                    Response = new ObservableCollection<string>() { "7D 03 EA 0F AB" };
                }

                return new TestSequence(new ObservableCollection<Keyword>()
                {
                  new TraceStart(),
                    new EcuOn(),
                    new Wait(5000),
                    new MM6Start("9","Normal"),
                    new StartDiagComRB(),
                    new SendRequest(sendRequest,Response),
                    //setModelValue,
                    new Wait(5000),
                    new EcuOff(),
                    new Wait(3000),
                    new EcuOn(),
                    new Wait(5000),
                    new MM6Stop(),
                    new TraceStop(),
                    new Wait(2000),
                    new StopDiagComRB(),
                    //new UndoSetModelValues(),
                    new EcuOff(),
                 });
            }
            //Parameterize

        }
    }

    /// <summary>
    /// for all kinds of Ay value check
    /// </summary>
    public class OpAySetModelValues : OpState
    {


        protected OpAySetModelValues()
        {
            Category = "BaseOperationStateType";
            Description = "This operation state is a base opstate type to manipulate Ay value";
            Parameters.Add(new CANTxParameter("TargetAy_ms2", "Target Ay to set constant in ms2", "1", typeof(int)));


        }
        public override string Name
        {
            get { return "AySetModelValues"; }
        }


        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverTime(7000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {


            string speed = paraSet.SingleOrDefault(x => x.Name == "TargetAy_ms2").Value;


            if (paraSet.SingleOrDefault(x => x.Name == "SignalUnit") != null && paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage") != null) //to indicate it is a value check
            {
                double factor = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "Factor").Value);
                double offset = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "Offset").Value);
                if (Regex.IsMatch(paraSet.SingleOrDefault(x => x.Name == "SignalUnit").Value, "g"))
                {


                    OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = (int)((Convert.ToDouble(speed) / 10 - offset) / factor) + "%" + paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value;

                }
                else if (Regex.IsMatch(paraSet.SingleOrDefault(x => x.Name == "SignalUnit").Value, "ms2"))
                {

                    OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = (int)((Convert.ToDouble(speed) - offset) / factor) + "%" + paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value;
                }

            }
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {

            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);

            //redecorate the test script
            testScript.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
            testScript.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

            //adjust the unit convertion
            AssignParameterValues(paraSet);

            //Parameter check
            if (paraSet.SingleOrDefault(x => x.Name == "TargetAy_ms2") == null)
            {
                throw new FormatException(string.Format("Error when parmeterizng operation state {0}: TargetAy_ms2 is not provided", Name));

            }
            else
            {
                string AyStr = paraSet.SingleOrDefault(x => x.Name == "TargetAy_ms2").Value;

                Dictionary<string, string> dict2Set = new Dictionary<string, string>() { { "ay", AyStr } };
                SetModelValues setModelValue = new SetModelValues(dict2Set);
                // Bug:338 -> Xia jack fix 05/21/2018
                testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "A1A2Range" };
                testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "A1A2Range" };

                return new TestSequence(new ObservableCollection<Keyword>()
                {
                    new TraceStart(),
                    new EcuOn(),
                    new Wait(5000),
                    new MM6Start("9","Normal"),
                    setModelValue,
                    new Wait(5000),
                    new MM6Stop(),
                    new TraceStop(),
                    new UndoSetModelValues(),
                    new EcuOff(),
                 });
            }
            //Parameterize

        }
    }

    /// <summary>
    /// for all kinds of yawrate value check
    /// </summary>
    public class OpYawrateSetModelValues : OpState
    {


        protected OpYawrateSetModelValues()
        {
            Category = "BaseOperationStateType";
            Description = "This operation state is a base opstate type to manipulate yawrate value";
            Parameters.Add(new CANTxParameter("TargetYawrate_dps", "Target yawrate to set constant in degree per second", "1", typeof(int)));
        }
        public override string Name
        {
            get { return "YawrateSetModelValues"; }
        }


        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverTime(7000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {


            string speed = paraSet.SingleOrDefault(x => x.Name == "TargetYawrate_dps").Value;


            if (paraSet.SingleOrDefault(x => x.Name == "SignalUnit") != null && paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage") != null) //to indicate it is a value check
            {
                double factor = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "Factor").Value);
                double offset = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "Offset").Value);

                if (Regex.IsMatch(paraSet.SingleOrDefault(x => x.Name == "SignalUnit").Value, "rps"))
                {


                    OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = (int)((Convert.ToDouble(speed) / 57.3 - offset) / factor) + "%" + paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value;

                }
                else if (Regex.IsMatch(paraSet.SingleOrDefault(x => x.Name == "SignalUnit").Value, "dps"))
                {

                    OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = (int)((Convert.ToDouble(speed) - offset) / factor) + "%" + paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value;
                }

            }
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {

            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);

            //redecorate the test script
            testScript.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
            testScript.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

            //adjust the unit convertion
            AssignParameterValues(paraSet);

            //Parameter check
            if (paraSet.SingleOrDefault(x => x.Name == "TargetYawrate_dps") == null)
            {
                throw new FormatException(string.Format("Error when parmeterizng operation state {0}: TargetAy_ms2 is not provided", Name));

            }
            else
            {
                string yawStr = paraSet.SingleOrDefault(x => x.Name == "TargetYawrate_dps").Value;

                Dictionary<string, string> dict2Set = new Dictionary<string, string>() { { "yawRate", yawStr } };
                SetModelValues setModelValue = new SetModelValues(dict2Set);

                // Bug:338 -> Xia jack fix 05/21/2018
                testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "YrsPlaus" };
                testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "YrsPlaus" };

                return new TestSequence(new ObservableCollection<Keyword>()
                {
                    new TraceStart(),
                    new EcuOn(),
                    new MM6Start("9","Normal"),
                    new Wait(5000),
                    setModelValue,
                    new Wait(5000),
                    new MM6Stop(),
                    new TraceStop(),
                    new UndoSetModelValues(),
                    new EcuOff(),
                 });
            }
            //Parameterize

        }
    }

    /// <summary>
    /// for all kinds of MC pressure value check
    /// </summary>
    public class OpPressureSetModelValues : OpState
    {


        protected OpPressureSetModelValues()
        {
            Category = "BaseOperationStateType";
            Description = "This operation state is a base opstate type to manipulate pressure value";
            Parameters.Add(new CANTxParameter("TargetPressure_bar", "Target pressure to set constant in degree per second", "1", typeof(int)));


        }
        public override string Name
        {
            get { return "PressureSetModelValues"; }
        }



        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverTime(12000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {


            string speed = paraSet.SingleOrDefault(x => x.Name == "TargetPressure_bar").Value;


            if (paraSet.SingleOrDefault(x => x.Name == "SignalUnit") != null && paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage") != null) //to indicate it is a value check
            {
                double factor = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "Factor").Value);
                double offset = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "Offset").Value);

                if (Regex.IsMatch(paraSet.SingleOrDefault(x => x.Name == "SignalUnit").Value, "pa"))
                {


                    OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = (int)((Convert.ToDouble(speed) * 100000 - offset) / factor) + "%" + paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value;

                }
                else if (Regex.IsMatch(paraSet.SingleOrDefault(x => x.Name == "SignalUnit").Value, "bar"))
                {

                    OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = (int)((Convert.ToDouble(speed) - offset) / factor) + "%" + paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value;
                }

            }
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {

            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);

            //redecorate the test script
            testScript.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
            testScript.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

            //adjust the unit convertion
            AssignParameterValues(paraSet);

            //Parameter check
            if (paraSet.SingleOrDefault(x => x.Name == "TargetPressure_bar") == null)
            {
                throw new FormatException(string.Format("Error when parmeterizng operation state {0}: TargetPressure_bar is not provided", Name));

            }
            else
            {
                string ptr = paraSet.SingleOrDefault(x => x.Name == "TargetPressure_bar").Value;

                Dictionary<string, string> dict2Set = new Dictionary<string, string>() { { "p_MBC", ptr } };
                SetModelValues setModelValue = new SetModelValues(dict2Set);

                // Fix th3 issue, if not set BLS to 1 , pressureMBC  not matches BLS -> Jack
                Dictionary<string, string> modelValueNormal = new Dictionary<string, string>();
                modelValueNormal.Add("BLS", "1");

                // Bug:340 -> Xia jack fix 05/21/2018
                testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "PressSensMC1Line" };
                testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "PressSensMC1Line" };


                return new TestSequence(new ObservableCollection<Keyword>()
                {
                    new TraceStart(),
                    new EcuOn(),
                    new Wait(5000),
                    new MM6Start("9","Normal"),
                    new SetModelValues(modelValueNormal),
                    new Wait(5000),
                    setModelValue,
                    new Wait(5000),
                    new MM6Stop(),
                    new TraceStop(),
                    new UndoSetModelValues(),
                    new EcuOff(),
                 });
            }
            //Parameterize

        }
    }

    /// <summary>
    /// for all kinds of MC pressure value check
    /// </summary>
    public class OpBrakeSetModelValues : OpState
    {


        protected OpBrakeSetModelValues()
        {
            Category = "BaseOperationStateType";
            Description = "This operation state is a base opstate type to manipulate brake";
            Parameters.Add(new CANTxParameter("TargetBrake_perone", "Target Brake to set constant 0-1", "1", typeof(int)));


        }
        public override string Name
        {
            get { return "BrakeSetModelValues"; }
        }



        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverTime(8000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {


            string speed = paraSet.SingleOrDefault(x => x.Name == "TargetBrake_perone").Value;


            if (paraSet.SingleOrDefault(x => x.Name == "SignalUnit") != null && paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage") != null) //to indicate it is a value check
            {
                double factor = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "Factor").Value);
                double offset = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "Offset").Value);

                if (Regex.IsMatch(paraSet.SingleOrDefault(x => x.Name == "SignalUnit").Value, "percentage"))
                {


                    OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = (int)((Convert.ToDouble(speed) * 100 - offset) / factor) + "%" + paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value;

                }
                else if (Regex.IsMatch(paraSet.SingleOrDefault(x => x.Name == "SignalUnit").Value, "perone"))
                {

                    OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = (int)((Convert.ToDouble(speed) - offset) / factor) + "%" + paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value;
                }

            }
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {

            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);

            //redecorate the test script
            testScript.InitDiagSequence.ValueList = new ObservableCollection<string>() { "Initial.seq" };
            testScript.InitModelValues.KeyValuePairList = new Dictionary<string, string>() { { "BLS", "0" }, { "p_C1", "0" }, { "p_MBC", "0" }, };

            //adjust the unit convertion
            AssignParameterValues(paraSet);

            //Parameter check
            if (paraSet.SingleOrDefault(x => x.Name == "TargetBrake_perone") == null)
            {
                throw new FormatException(string.Format("Error when parmeterizng operation state {0}: TargetBrake_perone is not provided", Name));

            }
            else
            {
                string brk = paraSet.SingleOrDefault(x => x.Name == "TargetBrake_perone").Value;

                Dictionary<string, string> dict2Set = new Dictionary<string, string>() { { "BrakePedal", brk } };
                SetModelValues setModelValue = new SetModelValues(dict2Set);

                return new TestSequence(new ObservableCollection<Keyword>()
                {
                    new TraceStart(),
                    new EcuOn(),
                    new Wait(5000),
                    new MM6Start("9","Normal"),
                    setModelValue,
                    new Wait(5000),
                    new MM6Stop(),
                    new TraceStop(),
                    new UndoSetModelValues(),
                    new EcuOff(),
                 });
            }
            //Parameterize

        }
    }
    #endregion base opstate types, concrete opstate inherit here

    #region Normal Driving Category

    public sealed class OpNormalDriving10kph : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();

        private static readonly OpNormalDriving10kph _instance = new OpNormalDriving10kph();


        private OpNormalDriving10kph()
        {
            Category = "NormalDriving";
            Description = "This operation state is Normal Driving at 10kph,speed reached in after 7s in trace";
        }
        public override string Name
        {
            get { return "NormalDriving10kph"; }
        }

        public static OpNormalDriving10kph GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverTime(10000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(2000),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string>() {"10kph_in_5s.lcs" }),
                new Wait(10000),
                new StimuliStop(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    public sealed class OpNormalDriving50kph : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpNormalDriving50kph _instance = new OpNormalDriving50kph();


        private OpNormalDriving50kph()
        {
            Category = "NormalDriving";
            Description = "This operation state is to Normal Driving at 50kph,speed reached in after 12s in trace";
        }
        public override string Name
        {
            get { return "NormalDriving50kph"; }
        }

        public static OpNormalDriving50kph GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverTime(15000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(2000),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string>() {"50kph_in_10s.lcs" }),
                new Wait(15000),
                new StimuliStop(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    public sealed class OpNormalDriving100kph : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpNormalDriving100kph _instance = new OpNormalDriving100kph();


        private OpNormalDriving100kph()
        {
            Category = "NormalDriving";
            Description = "This operation state is Normal Driving at 100kph, speed reached in after 21s in trace";
        }
        public override string Name
        {
            get { return "NormalDriving100kph"; }
        }


        public static OpNormalDriving100kph GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverTime(21000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(2000),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string>() {"100kph_in_16s.lcs" }),
                new Wait(21000),
                new StimuliStop(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    public sealed class OpNormalDrivingInit0S : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpNormalDrivingInit0S _instance = new OpNormalDrivingInit0S();


        private OpNormalDrivingInit0S()
        {
            Category = "NormalDriving";
            Description = "This operation state is Normal Driving at init phase";
        }
        public override string Name
        {
            get { return "Init0s"; }
        }


        public static OpNormalDrivingInit0S GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverTime(0);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new TraceStart(),
                new EcuOn(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new Wait(1000),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    public sealed class OpNormalDrivingInit1S : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpNormalDrivingInit1S _instance = new OpNormalDrivingInit1S();


        private OpNormalDrivingInit1S()
        {
            Category = "NormalDriving";
            Description = "This operation state is Normal Driving at init phase";
        }
        public override string Name
        {
            get { return "Init1s"; }
        }


        public static OpNormalDrivingInit1S GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverTime(1000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new TraceStart(),
                new EcuOn(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new Wait(1000),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    public sealed class OpNormalDrivingInit2S : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpNormalDrivingInit2S _instance = new OpNormalDrivingInit2S();


        private OpNormalDrivingInit2S()
        {
            Category = "NormalDriving";
            Description = "This operation state is Normal Driving at init phase";
        }
        public override string Name
        {
            get { return "Init2s"; }
        }


        public static OpNormalDrivingInit2S GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverTime(2000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new TraceStart(),
                new EcuOn(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new Wait(1000),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    public sealed class OpNormalDrivingInit3S : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpNormalDrivingInit3S _instance = new OpNormalDrivingInit3S();


        private OpNormalDrivingInit3S()
        {
            Category = "NormalDriving";
            Description = "This operation state is Normal Driving at init phase";
        }
        public override string Name
        {
            get { return "Init3s"; }
        }


        public static OpNormalDrivingInit3S GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverTime(3000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new TraceStart(),
                new EcuOn(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new Wait(1000),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    public sealed class OpNormalDrivingInit4S : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpNormalDrivingInit4S _instance = new OpNormalDrivingInit4S();


        private OpNormalDrivingInit4S()
        {
            Category = "NormalDriving";
            Description = "This operation state is Normal Driving at init phase";
        }
        public override string Name
        {
            get { return "Init4s"; }
        }


        public static OpNormalDrivingInit4S GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverTime(4000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new TraceStart(),
                new EcuOn(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new Wait(1000),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    public sealed class OpNormalDrivingInit5S : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpNormalDrivingInit5S _instance = new OpNormalDrivingInit5S();


        private OpNormalDrivingInit5S()
        {
            Category = "NormalDriving";
            Description = "This operation state is Normal Driving at init phase";
        }
        public override string Name
        {
            get { return "Init5s"; }
        }


        public static OpNormalDrivingInit5S GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverTime(5000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new TraceStart(),
                new EcuOn(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new Wait(1000),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    public sealed class OpPostrunStandstill : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpPostrunStandstill _instance = new OpPostrunStandstill();


        private OpPostrunStandstill()
        {
            Category = "NormalDriving";
            Description = "This operation state is Normal Driving at postrun phase ,standstill";
        }
        public override string Name
        {
            get { return "PostrunStandstill"; }
        }


        public static OpPostrunStandstill GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon("LCI_UZ1", 0, TriggerConditionSignal.Equal));

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new Wait(1000),
                new MM6Stop(),
                new EcuOff(),
                new Wait(5000),
                new TraceStop(),

            });
        }
    }

    public sealed class OpPostrun50kph : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpPostrun50kph _instance = new OpPostrun50kph();


        private OpPostrun50kph()
        {
            Category = "NormalDriving";
            Description = "This operation state is Normal Driving at postrun phase ,50kph";
        }
        public override string Name
        {
            get { return "Postrun50kph"; }
        }


        public static OpPostrun50kph GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon("LCI_UZ1", 0, TriggerConditionSignal.Equal));

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string> () {"50kph_in_10s.lcs" }),
                new Wait(10000),
                new MM6Stop(),
                new EcuOff(),
                new Wait(5000),
                new TraceStop(),
                new StimuliStop(),

            });
        }
    }
    #endregion Normal Driving Category

    #region ControllerIntervention
    public sealed class OpABSActive : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpABSActive _instance = new OpABSActive();


        private OpABSActive()
        {
            Category = "ControllerIntervention";
            Description = "This operation state is to activate ABS controller,brake at speed 100kph";
        }
        public override string Name
        {
            get { return "ABSActive"; }
        }

        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigABSActive.GetInstance()); }
        }

        public static OpABSActive GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            //LC info DBC , change Signal LCI_I_MV_LF_UP_A greater 0.5 -> Jack 
            //MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon("LCI_I_MV_LF_UP_A", 0.5m, TriggerConditionSignal.Greater), 10000, 1000);
            // change abs active signal condition based on User selection: e,g typical is ABS_Active
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon(Parameters[1].Value, 1, TriggerConditionSignal.Equal));
            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation
            testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "AcmBtm_OverheatDuringActiveBraking", "Abs_EmergencyBrake" };
            testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "AcmBtm_OverheatDuringActiveBraking", "Abs_EmergencyBrake" };
            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string>() {"ABS.lcs" }),
                new Wait(30000),
                new StimuliStop(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    public sealed class OpEBDActive : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();

        private static readonly OpEBDActive _instance = new OpEBDActive();

        private OpEBDActive()
        {
            Category = "ControllerIntervention";
            Description = "This operation state is to activate EBD controller,trigger EBD lcs file, then brake at speed 100kph";
        }
        public override string Name
        {
            get { return "EBDActive"; }
        }
        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigEBDActive.GetInstance()); }
        }

        public static OpEBDActive GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            //Change evaluation pattern
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon(Parameters[1].Value, 1, TriggerConditionSignal.Equal));
            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }


        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Parameterize
            //testScript.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "WssFLLineGnd" };
            //testScript.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "WssFLLineGnd" };
            testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "WssFLNoEdge", "AcmBtm_OverheatDuringActiveBraking", "Abs_EmergencyBrake" };
            testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "WssFLNoEdge", "AcmBtm_OverheatDuringActiveBraking", "Abs_EmergencyBrake" };
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string>() {"EBD.lcs" }),
                new Wait(30000),
                new StimuliStop(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    public sealed class OpTCSActive : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpTCSActive _instance = new OpTCSActive();

        private OpTCSActive()
        {
            Category = "ControllerIntervention";
            Description = "This operation state is to activate TCS controller (AMR+BMR),and check the related signals";
        }
        public override string Name
        {
            get { return "TCSActive"; }
        }
        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigTCSActive.GetInstance()); }
        }

        public static OpTCSActive GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {

            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon(Parameters[1].Value, 1, TriggerConditionSignal.Equal));

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;

        }


        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string>() {"TCS_AMR_BMR.lcs" }),
                new Wait(30000),
                new MM6Stop(),
                new StimuliStop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    public sealed class OpAMRActive : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpAMRActive _instance = new OpAMRActive();

        private OpAMRActive()
        {
            Category = "ControllerIntervention";
            Description = "This operation state is to activate AMR controller,and check the related signals";
        }
        public override string Name
        {
            get { return "AMRActive"; }
        }
        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigTCSActive.GetInstance()); }
        }

        public static OpAMRActive GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            Trigger amr = new Trigger(new SigConditon("LCI_B_ASR_fast", 1, TriggerConditionSignal.Equal), 15000, 100);
            testScript.CanTraceAnalyser.Triggers.Add(amr);
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(amr);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }


        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string>() {"TCS_AMR.lcs" }),
                new Wait(30000),
                new StimuliStop(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    public sealed class OpESPFuncOff : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpESPFuncOff _instance = new OpESPFuncOff();

        private OpESPFuncOff()
        {
            Category = "ControllerIntervention";
            Description = "This operation state is to deactivate ESP (PATA On mode), and check the related signals";
        }
        public override string Name
        {
            get { return "ESPFuncOff"; }
        }
        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigVDCActive.GetInstance()); }
        }

        public static OpESPFuncOff GetInstance()
        {
            return _instance;
        }


        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            Trigger pala = new Trigger(new SigConditon("PALA", 1, TriggerConditionSignal.Equal), 5000, 100);
            testScript.CanTraceAnalyser.Triggers.Add(pala);
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(pala);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }

        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string>() {"PATA_2s_press.lcs" }),
                new Wait(3000),
                new StimuliStop(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    public sealed class OpPataStuck : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpPataStuck _instance = new OpPataStuck();

        private OpPataStuck()
        {
            Category = "ControllerIntervention";
            Description = "This operation state is to PATA Handbag mode, and check the related signals,e.g press PATA button more than 10s";
        }
        public override string Name
        {
            get { return "PataStuck"; }
        }
        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigVDCActive.GetInstance()); }
        }

        public static OpPataStuck GetInstance()
        {
            return _instance;
        }


        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            Trigger pala = new Trigger(new SigConditon("PALA", 1, TriggerConditionSignal.Equal), 18000, 100);
            testScript.CanTraceAnalyser.Triggers.Add(pala);
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(pala);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }

        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string>() {"PATA_stuck.lcs" }),
                new Wait(3000),
                new StimuliStop(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    public sealed class OpVDCActive : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpVDCActive _instance = new OpVDCActive();

        private OpVDCActive()
        {
            Category = "ControllerIntervention";
            Description = "This operation state is to activate VDC controller, and check the related signals";
        }
        public override string Name
        {
            get { return "VDCActive"; }
        }
        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigVDCActive.GetInstance()); }
        }

        public static OpVDCActive GetInstance()
        {
            return _instance;
        }


        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon(Parameters[1].Value, 1, TriggerConditionSignal.Equal));

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };

            return tempMEP;

        }

        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check
            testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "AcmBtm_OverheatDuringActiveBraking", "AcmBtm_OverheatWarningDeactivated", "Abs_EmergencyBrake" };
            testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "AcmBtm_OverheatDuringActiveBraking", "AcmBtm_OverheatWarningDeactivated", "Abs_EmergencyBrake" };
            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string>() {"VDC_22mps.lcs" }),
                new Wait(30000),
                new StimuliStop(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    #endregion ControllerIntervention

    #region FailureState
    public sealed class OpOneWssInterrupt : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpOneWssInterrupt _instance = new OpOneWssInterrupt();

        private OpOneWssInterrupt()
        {
            Category = "FailureState";
            Description = "This operation state is to interrupt specified Wss sensor, and check the related signals";
            Parameters.Add(new CANTxParameter("OpState_Wss2Interrupt", "Constant speed driving for 4 wheels in mps", "WSSFL_line", typeof(int)));
        }
        public override string Name
        {
            get { return "OneWssInterrupt"; }
        }
        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigWheelSpdValid.GetInstance()); }
        }

        public static OpOneWssInterrupt GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            //Trigger flag = new Trigger(new SigConditon("ABS_SILA", 1, TriggerConditionSignal.Equal), 5000, 100);
            //testScript.CanTraceAnalyser.Triggers.Add(flag);
            //MeasurementPoint tempMEP = new ReadCanSignalOverCondition(flag);

            //tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            //tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            //return tempMEP;



            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon("ABS_SILA", 1, TriggerConditionSignal.Equal), 5000, 100);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }

        int flag = 0;
        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Assign fault evaluation
            string line = paraSet.SingleOrDefault(x => x.Name == "OpState_Wss2Interrupt").Value;
            // Bug:332 fix -> 06/27/2018
            //Pattern is 0->1 if your projects are not Gen9.3, only one time ,if you remove the connection, it will recover to the default
            if (!ProductPar.ProductType.EndsWith("Gen93") && flag == 0)
            {
                IEnumerable<CANTxParameter> signalPatterns = paraSet.Where(dr => dr.Name == "SignalPattern");
                foreach (CANTxParameter pattern in signalPatterns)
                {
                    flag++;
                    pattern.Value = "0->1";
                    break;
                }

            }

            string fault = "";
            string optionalFault = "";
            switch (line)
            {
                case "WSSFL_line":
                    if (ProductPar.ProductType == "Gen93")
                    {
                        fault = "RBWssFLLineGnd";
                        optionalFault = "RBWssFLNoEdge";
                    }
                    else
                    {
                        fault = "WssFLLineGnd";
                        optionalFault = "WssFLNoEdge";
                    }
                    break;
                case "WSSFR_line":
                    if (ProductPar.ProductType == "Gen93")
                    {
                        fault = "RBWssFRLineGnd";
                        optionalFault = "RBWssFRNoEdge";
                    }
                    else
                    {
                        fault = "WssFRLineGnd";
                        optionalFault = "WssFRNoEdge";
                    }
                    break;
                case "WSSRL_line":
                    if (ProductPar.ProductType == "Gen93")
                    {
                        fault = "RBWssRLLineGnd";
                        optionalFault = "RBWssRLNoEdge";
                    }
                    else
                    {
                        fault = "WssRLLineGnd";
                        optionalFault = "WssRLNoEdge";
                    }
                    break;
                case "WSSRR_line":
                    if (ProductPar.ProductType == "Gen93")
                    {
                        fault = "RBWssRRLineGnd";
                        optionalFault = "RBWssRRNoEdge";
                    }
                    else
                    {
                        fault = "WssRRLineGnd";
                        optionalFault = "WssRRNoEdge";
                    }
                    break;
                default:
                    fault = "Error";
                    break;
            }

            if (fault != "Error")
            {
                testScript.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { fault };

                testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { optionalFault };

                testScript.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { fault };

                testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { optionalFault };
            }

            //Parameterize

            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new InterruptLines(new Dictionary<string, string>() { {"line1", line }, }),
                new Wait(3000),
                new UndoInterruptLines(new Dictionary<string, string>() { {"line1", line }, }),
                //Need to wait 12s
                new Wait(15000),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    public sealed class Op4WssInterrupt : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly Op4WssInterrupt _instance = new Op4WssInterrupt();

        private Op4WssInterrupt()
        {
            Category = "FailureState";
            Description = "This operation state is to interrupt 4 Wss sensor, and check the related signals";
            //Parameters.Add(new CANTxParameter("OpState_Wss2Interrupt", "Constant speed driving for 4 wheels in mps", "WssFL_Line", typeof(int)));
        }
        public override string Name
        {
            get { return "4WssInterrupt"; }
        }
        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigWheelSpdValid.GetInstance()); }
        }

        public static Op4WssInterrupt GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {

            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon("ABS_SILA", 1, TriggerConditionSignal.Equal), 5000, 1000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;

        }
        int flag = 0;
        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Assign fault evaluation

            //Pattern is 0->1 if your projects are not Gen9.3, only one time ,if you remove the connection, it will recover to the default
            if (!ProductPar.ProductType.EndsWith("Gen93") && flag == 0)
            {
                IEnumerable<CANTxParameter> signalPatterns = paraSet.Where(dr => dr.Name == "SignalPattern");
                foreach (CANTxParameter pattern in signalPatterns)
                {
                    flag++;
                    pattern.Value = "0->1";
                    break;
                }

            }

            if (ProductPar.ProductType == "Gen93" || ProductPar.ProductType == "ABS_Gen93")
            {
                testScript.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBWssFLLineGnd", "RBWssFRLineGnd", "RBWssRLLineGnd", "RBWssRRLineGnd" };
                testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "RBWssFLNoEdge", "RBWssFRNoEdge", "RBWssRLNoEdge", "RBWssRRNoEdge", "Wss_MoreThanOneSuspected" };
                testScript.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBWssFLLineGnd", "RBWssFRLineGnd", "RBWssRLLineGnd", "RBWssRRLineGnd" };
                testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "RBWssFLNoEdge", "RBWssFRNoEdge", "RBWssRLNoEdge", "RBWssRRNoEdge", "Wss_MoreThanOneSuspected" };
            }
            else
            {
                testScript.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "WssFLLineGnd", "WssFRLineGnd", "WssRLLineGnd", "WssRRLineGnd" };
                testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "WssFLNoEdge", "WssFRNoEdge", "WssRLNoEdge", "WssRRNoEdge", "Wss_MoreThanOneSuspected" };
                testScript.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "WssFLLineGnd", "WssFRLineGnd", "WssRLLineGnd", "WssRRLineGnd" };
                testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "WssFLNoEdge", "WssFRNoEdge", "WssRLNoEdge", "WssRRNoEdge", "Wss_MoreThanOneSuspected" };
            }

            //Parameterize

            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new InterruptLines(new Dictionary<string, string>() { {"line1","WSSFL_line" },{"line2","WSSFR_line" },{"line3","WSSRL_line" },{"line4","WSSRR_line" }, }),
                new Wait(3000),
                new UndoInterruptLines(new Dictionary<string, string>() { {"line1","WSSFL_line" },{"line2","WSSFR_line" },{"line3","WSSRL_line" },{"line4","WSSRR_line" }, }),
                new Wait(15000),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    // will ask yijie, what is th RB madantory faults -todo
    public sealed class Op4WssDyanamic : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly Op4WssDyanamic _instance = new Op4WssDyanamic();

        private Op4WssDyanamic()
        {
            Category = "FailureState";
            Description = "This operation state is to interrupt 4 Wss sensor signal lost every 2 seconds, and check the related signals";
            Parameters.Add(new CANTxParameter("OpState_Wss2Interrupt", "Constant speed driving for 4 wheels in mps", "WSSFL_Line", typeof(int)));
        }
        public override string Name
        {
            get { return "4WssDyanamic"; }
        }
        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigWheelSpdValid.GetInstance()); }
        }

        public static Op4WssDyanamic GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            //Trigger flag = new Trigger(new SigConditon("ABS_SILA", 1, TriggerConditionSignal.Equal), 5000, 2000);
            //testScript.CanTraceAnalyser.Triggers.Add(flag);
            //MeasurementPoint tempMEP = new ReadCanSignalOverCondition(flag);

            //tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            //tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            //return tempMEP;
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon("FDR_SILA", 1, TriggerConditionSignal.Equal), 5000, 1000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }

        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Assign fault evaluation

            testScript.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "Wss_MonVDiff_FL", "Wss_MonVDiff_FR", "Wss_MonVDiff_RL", "Wss_MonVDiff_RR" };

            testScript.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "Wss_MonVDiff_FL", "Wss_MonVDiff_FR", "Wss_MonVDiff_RL", "Wss_MonVDiff_RR" };


            //Parameterize

            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string> () { "WSS_Dynamic_FLFRRLRR.lcs"}),
                new Wait(36000),
                new StimuliStop(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    public sealed class OpOneValveInterrupt : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpOneValveInterrupt _instance = new OpOneValveInterrupt();

        private OpOneValveInterrupt()
        {
            Category = "FailureState";
            Description = "This operation state is to interrupt specified valve sensor, and check the related signals";
            Parameters.Add(new CANTxParameter("Valve2Interrupt", "valve to be interrupted,eg.EVFL,HSV1,USV2 ", "EVFL", typeof(int)));


        }
        public override string Name
        {
            get { return "OneValveInterrupt"; }
        }
        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigWheelSpdValid.GetInstance()); }
        }

        public static OpOneValveInterrupt GetInstance()
        {
            return _instance;
        }
        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {

            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon("ABS_SILA", 1, TriggerConditionSignal.Equal), 8000, 1000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }
        int flag = 0;
        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            // Bug 333 -> fixed by Xia Jack 05/21/2018
            //Pattern is 0->1 if your projects are not Gen9.3, only one time ,if you remove the connection, it will recover to the default
            if (!ProductPar.ProductType.EndsWith("Gen93") && flag == 0)
            {
                IEnumerable<CANTxParameter> signalPatterns = paraSet.Where(dr => dr.Name == "SignalPattern");
                foreach (CANTxParameter pattern in signalPatterns)
                {
                    flag++;
                    pattern.Value = "0->1";
                    break;
                }

            }


            string line = paraSet.SingleOrDefault(x => x.Name == "Valve2Interrupt").Value;

            string fault = "";
            switch (line)
            {
                case "EVFL":
                    fault = "ValveEvFlGeneric";
                    break;
                case "EVFR":
                    fault = "ValveEvFrGeneric";
                    break;
                case "EVRL":
                    fault = "ValveEvRlGeneric";
                    break;
                case "EVRR":
                    fault = "ValveEvRrGeneric";
                    break;
                case "AVFL":
                    fault = "ValveAvFlGeneric";
                    break;
                case "AVFR":
                    fault = "ValveAvFrGeneric";
                    break;
                case "AVRL":
                    fault = "ValveAvRlGeneric";
                    break;
                case "AVRR":
                    fault = "ValveAvRrGeneric";
                    break;
                case "USV1":
                    fault = "ValveUsv1Generic";
                    break;
                case "HSV1":
                    fault = "ValveHsv1Generic";
                    break;
                case "USV2":
                    fault = "ValveUsv2Generic";
                    break;
                case "HSV2":
                    fault = "ValveHsv2Generic";
                    break;
                default:
                    fault = "Error";
                    break;
            }

            if (fault != "Error")
            {
                testScript.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { fault };
                testScript.CUMandatoryFaults.ValueList = new ObservableCollection<string>()
                {
                    fault
                };
            }

            //Parameterize

            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new InterruptLines(new Dictionary<string, string>() { {"line1", line }, }),
                new Wait(5000),
                new UndoInterruptLines(new Dictionary<string, string>() { {"line1", line }, }),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    // will ask yijie, what is RB madantory faults -> todo
    public sealed class OpMRAInterrupt : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpMRAInterrupt _instance = new OpMRAInterrupt();

        private OpMRAInterrupt()
        {
            Category = "FailureState";
            Description = "This operation state is to interrupt specified motor, and check the related signals";

        }
        public override string Name
        {
            get { return "MRAInterrupt"; }
        }
        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigWheelSpdValid.GetInstance()); }
        }

        public static OpMRAInterrupt GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon("ABS_SILA", 1, TriggerConditionSignal.Equal), 5000, 1000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }

        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Assign fault evaluation
            testScript.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "RfpMotorPathTestFailed" };
            testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "RfpMRscError" };
            testScript.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "RfpMotorPathTestFailed" };
            testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "RfpMRscError" };

            // Set 4 wheel speed to 20pkh  -> in order to satisfy MRA trigger condition
            Dictionary<string, string> modelValueNormal = new Dictionary<string, string>();
            modelValueNormal.Add("v_Wheel_LF", "6.7");
            modelValueNormal.Add("v_Wheel_RF", "6.7");
            modelValueNormal.Add("v_Wheel_LR", "6.7");
            modelValueNormal.Add("v_Wheel_RR", "6.7");

            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new Wait(3000),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new InterruptLines(new Dictionary<string, string>() { {"line1", "MRA_line" }, }),
                new Wait(5000),
                new SetModelValues(modelValueNormal),
                new Wait(10000),
                new UndoInterruptLines(new Dictionary<string, string>() { { "line1", "MRA_line" }, }),
                new Wait(3000),
                new UndoSetModelValues(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    public sealed class OpVRAInterrupt : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpVRAInterrupt _instance = new OpVRAInterrupt();

        private OpVRAInterrupt()
        {
            Category = "FailureState";
            Description = "This operation state is to interrupt specified motor, and check the related signals";

        }
        public override string Name
        {
            get { return "VRAInterrupt"; }
        }
        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigWheelSpdValid.GetInstance()); }
        }

        public static OpVRAInterrupt GetInstance()
        {
            return _instance;
        }
        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            //Trigger flag = new Trigger(new SigConditon("ABS_SILA", 1, TriggerConditionSignal.Equal), 5000, 1000);
            //testScript.CanTraceAnalyser.Triggers.Add(flag);
            //MeasurementPoint tempMEP = new ReadCanSignalOverCondition(flag);

            //tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            //tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            //return tempMEP;
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon("ABS_SILA", 1, TriggerConditionSignal.Equal), 8000, 1000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }
        int flag = 0;
        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);

            // Bug 334 -> fixed by Xia Jack 05/21/2018
            //Pattern is 0->1 if your projects are not Gen9.3, only one time ,if you remove the connection, it will recover to the default
            if (!ProductPar.ProductType.EndsWith("Gen93") && flag == 0)
            {
                IEnumerable<CANTxParameter> signalPatterns = paraSet.Where(dr => dr.Name == "SignalPattern");
                foreach (CANTxParameter pattern in signalPatterns)
                {
                    flag++;
                    pattern.Value = "0->1";
                    break;
                }

            }
            //Assign fault evaluation
            testScript.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "VrOnFailsContinuous" };  // to be confirmed

            testScript.CUMandatoryFaults.ValueList = new ObservableCollection<string>()
            {
                "VrOnFailsContinuous"
            };
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new Wait(3000),
                new MM6Start("9","Normal"),
                new TraceStart(),
                new Wait(5000),
                new InterruptLines(new Dictionary<string, string>() { {"line1", "VRA_line" }, }),
                new Wait(20000),
                new UndoInterruptLines(new Dictionary<string, string>() { {"line1", "VRA_line" }, }),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    public sealed class OpEPBTO : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpEPBTO _instance = new OpEPBTO();

        private OpEPBTO()
        {
            Category = "FailureState";
            Description = "This operation state is to set EPB TO, and check the related signals";

        }
        public override string Name
        {
            get { return "EPBTimeOut"; }
        }
        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(EPBTimeOut.GetInstance()); }
        }

        public static OpEPBTO GetInstance()
        {
            return _instance;
        }
        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon("ABS_SILA", 1, TriggerConditionSignal.Equal), 8000, 1000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }

        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Assign fault evaluation
            testScript.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "ComScl_EPB_1_Timeout" };

            testScript.CUMandatoryFaults.ValueList = new ObservableCollection<string>()
            {
                "ComScl_EPB_1_Timeout"
            };
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new Wait(3000),
                new MM6Start("9","Normal"),
                new TraceStart(),
                new Wait(5000),
                new DisableMessage("EPB_1"),
                new Wait(3000),
                new ResetCanManipulation(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    public sealed class OpIFAFailure : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpIFAFailure _instance = new OpIFAFailure();

        private OpIFAFailure()
        {
            Category = "FailureState";
            Description = "This operation state is to generate IFA, and check the related signals->Test Squence: speed up to 50kph, interrupt VRA, apply APB button";

        }
        public override string Name
        {
            get { return "IFAFailure"; }
        }
        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { IFA.GetInstance(); }
        }

        public static OpIFAFailure GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {

            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon("EBV_SILA", 1, TriggerConditionSignal.Equal), 5000, 1000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }

        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Assign fault evaluation
            testScript.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "VrOnFailsContinuous" };
            testScript.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "VrOnFailsContinuous" };

            // Set 4 wheel speed to 50kph
            Dictionary<string, string> modelValueNormal = new Dictionary<string, string>
            {
                { "v_Wheel_LF", "13.9" },
                { "v_Wheel_RF", "13.9" },
                { "v_Wheel_LR", "13.9" },
                { "v_Wheel_RR", "13.9" }
            };


            Dictionary<string, string> modelValueApbButton = new Dictionary<string, string>
            {
                { "APB_apply", "1" }

            };

            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new Wait(3000),
                new MM6Start("9","Normal"),
                new TraceStart(),
                new Wait(3000),
                new SetModelValues(modelValueNormal),
                new Wait(3000),
                new InterruptLines(new Dictionary<string, string>() { {"line1", "VRA_line" }, }),
                new Wait(20000),
                new SetModelValues(modelValueApbButton),
                new Wait(5000),
                new UndoInterruptLines(new Dictionary<string, string>() { {"line1", "VRA_line" }, }),
                new Wait(3000),
                new UndoSetModelValues(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    public sealed class OpApbLineOpen : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly OpApbLineOpen _instance = new OpApbLineOpen();

        private OpApbLineOpen()
        {
            Category = "FailureState";
            Description = "This operation state is to interrupt Apb button Line, and check the related signals";

        }
        public override string Name
        {
            get { return "APbButtonLineOpen"; }
        }
        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { SigApbButtonLineOpen.GetInstance(); }
        }

        public static OpApbLineOpen GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {

            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon("Full_system", 1, TriggerConditionSignal.Equal), 5000, 1000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }

        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            // Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Assign fault evaluation
            testScript.RBMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBApbButtonLine" };
            testScript.CUMandatoryFaults.ValueList = new ObservableCollection<string>() { "RBApbButtonLine" };

            // Set 4 wheel speed to 50kph
            Dictionary<string, string> modelValueNormal = new Dictionary<string, string>
            {
                { "v_Wheel_LF", "3.3" },
                { "v_Wheel_RF", "3.3" },
                { "v_Wheel_LR", "3.3" },
                { "v_Wheel_RR", "3.3" }
            };


            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new Wait(3000),
                new TraceStart(),
                new MM6Start("9","Normal"),
                new Wait(3000),
                new SetModelValues(modelValueNormal),
                new Wait(3000),
                new InterruptLines(new Dictionary<string, string>() { {"line1", "K177" }, }),
                new Wait(5000),
                new UndoInterruptLines(new Dictionary<string, string>() { {"line1", "K177" }, }),
                new Wait(3000),
                new UndoSetModelValues(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    //Jack Add Diag Mode to CAN TX 2018/03/15
    public sealed class OpDiagMode : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();

        private static readonly OpDiagMode _instance = new OpDiagMode();


        private OpDiagMode()
        {
            Category = "FailureState";
            Description = "This operation state is send Diag command";
        }
        public override string Name
        {
            get { return "DiagMode"; }
        }

        public static OpDiagMode GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            MeasurementPoint tempMEP = new ReadCanSignalOverTime(8000);

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation
            ObservableCollection<string> diagRequest = new ObservableCollection<string>();
            diagRequest.Add("10 03");
            ObservableCollection<string> expResponse = new ObservableCollection<string>();
            expResponse.Add("50 03 00 32 01 F4");

            //Parameterize Bug:336 ->add wait 3000 after ECU on
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new Wait(3000),
                new TraceStart(),
                new MM6Start("9","Normal"),
                new Wait(2000),
                new StartDiagComCU(),
                new SendRequest(diagRequest,expResponse),
                new Wait(10000),
                new StopDiagComCU(),
                new MM6Stop(),
                new TraceStop(),
                new Wait(3000),
                new EcuOff(),
            });
        }
    }

    #endregion FailureState

    #region SpeedManipulation
    public sealed class OpVehicleSpeedMinimumBelow : OpSpeedSetModelValues
    {
        private static readonly OpVehicleSpeedMinimumBelow _instance = new OpVehicleSpeedMinimumBelow() { };
        public static OpVehicleSpeedMinimumBelow GetInstance()
        {
            return _instance;
        }


        private OpVehicleSpeedMinimumBelow()
        {
            Category = "SpeedManipulation";
            Description = "This operation state is to set below minimum speed ,default 0.9kph";
            Parameters.First(x => x.Name == "TargetSpeed_mps").Value = (0.9 / 3.6).ToString("F2");
            Parameters.First(x => x.Name == "Wheel2Set").Value = "All";
        }
        public override string Name
        {
            get { return "VehicleSpeedBelowMinimum"; }
        }
        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {
            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = "0";
            }
        }

    }

    public sealed class OpVehicleSpeedMinimumAbove : OpSpeedSetModelValues
    {
        private static readonly OpVehicleSpeedMinimumAbove _instance = new OpVehicleSpeedMinimumAbove() { };
        public static OpVehicleSpeedMinimumAbove GetInstance()
        {
            return _instance;
        }


        private OpVehicleSpeedMinimumAbove()
        {
            Category = "SpeedManipulation";
            Description = "This operation state is to set above minimum speed, default 1.1 kph ";
            Parameters.First(x => x.Name == "TargetSpeed_mps").Value = (1.1 / 3.6).ToString("F2");
            Parameters.First(x => x.Name == "Wheel2Set").Value = "All";
        }
        public override string Name
        {
            get { return "VehicleSpeedAboveMinimum"; }
        }


    }

    public sealed class OpVehicleSpeedTypical : OpSpeedSetModelValues
    {
        private static readonly OpVehicleSpeedTypical _instance = new OpVehicleSpeedTypical() { };
        public static OpVehicleSpeedTypical GetInstance()
        {
            return _instance;
        }


        private OpVehicleSpeedTypical()
        {
            Category = "SpeedManipulation";
            Description = "This operation state is to set above minimum speed, default 50 kph ";
            Parameters.First(x => x.Name == "TargetSpeed_mps").Value = (50 / 3.6).ToString("F2");
            Parameters.First(x => x.Name == "Wheel2Set").Value = "All";
        }
        public override string Name
        {
            get { return "VehicleSpeedTypical"; }
        }


    }

    public sealed class OpVehicleSpeedTypicalReverseGear : OpSpeedSetModelValues
    {
        private static readonly OpVehicleSpeedTypicalReverseGear _instance = new OpVehicleSpeedTypicalReverseGear() { };
        public static OpVehicleSpeedTypicalReverseGear GetInstance()
        {
            return _instance;
        }


        private OpVehicleSpeedTypicalReverseGear()
        {
            Category = "SpeedManipulation";
            Description = "This operation state is to set above minimum speed, default 50 kph ";
            Parameters.First(x => x.Name == "TargetSpeed_mps").Value = (50 / 3.6).ToString("F2");
            Parameters.First(x => x.Name == "Wheel2Set").Value = "All";
        }
        public override string Name
        {
            get { return "VehicleSpeedTypicalReverseGear"; }
        }


    }

    public sealed class OpVehicleSpeedAllDifferent : OpSpeedSetModelValues
    {
        private static readonly OpVehicleSpeedAllDifferent _instance = new OpVehicleSpeedAllDifferent() { };
        public static OpVehicleSpeedAllDifferent GetInstance()
        {
            return _instance;
        }


        private OpVehicleSpeedAllDifferent()
        {
            Category = "SpeedManipulation";
            Description = "This operation state is to set  4 wheel speed different to verify driven wheel calculation for vehicle speed  ";
            Parameters.First(x => x.Name == "TargetSpeed_mps").Value = (12 / 3.6).ToString("F2") + ";" + (14 / 3.6).ToString("F2") + ";" + (16 / 3.6).ToString("F2") + ";" + (18 / 3.6).ToString("F2");
            Parameters.First(x => x.Name == "Wheel2Set").Value = "v_Wheel_LF;v_Wheel_RF;v_Wheel_LR;v_Wheel_RR";
        }

        public override string Name
        {
            get { return "VehicleSpeedAllDifferent"; }
        }


    }

    public sealed class OpVehicleSpeedMaximumBelow : OpSpeedSetModelValues
    {
        private static readonly OpVehicleSpeedMaximumBelow _instance = new OpVehicleSpeedMaximumBelow() { };
        public static OpVehicleSpeedMaximumBelow GetInstance()
        {
            return _instance;
        }


        private OpVehicleSpeedMaximumBelow()
        {
            Category = "SpeedManipulation";
            Description = "This operation state is to set below maximum speed, default 290kph ";
            Parameters.First(x => x.Name == "TargetSpeed_mps").Value = (290 / 3.6).ToString("F2");
            Parameters.First(x => x.Name == "Wheel2Set").Value = "All";
        }
        public override string Name
        {
            get { return "VehicleSpeedBelowMaximum"; }
        }
        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {
            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = "0";
            }
        }
    }

    public sealed class OpVehicleSpeedMaximumAbove : OpSpeedSetModelValues
    {
        private static readonly OpVehicleSpeedMaximumAbove _instance = new OpVehicleSpeedMaximumAbove() { };
        public static OpVehicleSpeedMaximumAbove GetInstance()
        {
            return _instance;
        }


        private OpVehicleSpeedMaximumAbove()
        {
            Category = "SpeedManipulation";
            Description = "This operation state is to set above maximum speed, default 290kph ";
            Parameters.First(x => x.Name == "TargetSpeed_mps").Value = (368 / 3.6).ToString("F2");
            Parameters.First(x => x.Name == "Wheel2Set").Value = "All";
        }
        public override string Name
        {
            get { return "VehicleSpeedAboveMaximum"; }
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {
            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = "Input invalid values";
            }
        }
    }
    #endregion SpeedManipulation

    #region SensorValueManipulation
    #region Ax value check
    public sealed class OpAxMaximumAbove : OpAxSetModelValues
    {
        private static readonly OpAxMaximumAbove _instance = new OpAxMaximumAbove() { };
        public static OpAxMaximumAbove GetInstance()
        {
            return _instance;
        }


        private OpAxMaximumAbove()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set above maximum Ax, default 100ms2 ";
            Parameters.First(x => x.Name == "TargetAx_ms2").Value = (100).ToString("F2");
        }
        public override string Name
        {
            get { return "AxAboveMaximum"; }
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {

            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name).Value;
            }

        }
    }

    public sealed class OpAxMaximumBelow : OpAxSetModelValues
    {
        private static readonly OpAxMaximumBelow _instance = new OpAxMaximumBelow() { };
        public static OpAxMaximumBelow GetInstance()
        {
            return _instance;
        }


        private OpAxMaximumBelow()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set below maximum Ax, default 24ms2 ";
            Parameters.First(x => x.Name == "TargetAx_ms2").Value = (24).ToString("F2");
        }
        public override string Name
        {
            get { return "AxBelowMaximum"; }
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {

            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name).Value;
            }

        }
    }

    public sealed class OpAxMaximum : OpAxSetModelValues
    {
        private static readonly OpAxMaximum _instance = new OpAxMaximum() { };
        public static OpAxMaximum GetInstance()
        {
            return _instance;
        }


        private OpAxMaximum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set below maximum Ax, default 25ms2 ";
            Parameters.First(x => x.Name == "TargetAx_ms2").Value = (25).ToString("F2");
        }
        public override string Name
        {
            get { return "AxMaximum"; }
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {


            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name).Value;
            }

        }
    }

    public sealed class OpAxTypicalValue : OpAxSetModelValues
    {
        private static readonly OpAxTypicalValue _instance = new OpAxTypicalValue() { };
        public static OpAxTypicalValue GetInstance()
        {
            return _instance;
        }


        private OpAxTypicalValue()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set typical Ax, default 5ms2 ";
            Parameters.First(x => x.Name == "TargetAx_ms2").Value = (5).ToString("F2");
        }
        public override string Name
        {
            get { return "AxTypicalValue"; }
        }
    }

    public sealed class OpAxMinimumAbove : OpAxSetModelValues
    {
        private static readonly OpAxMinimumAbove _instance = new OpAxMinimumAbove() { };
        public static OpAxMinimumAbove GetInstance()
        {
            return _instance;
        }


        private OpAxMinimumAbove()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set below maximum Ax, default -24ms2 ";
            Parameters.First(x => x.Name == "TargetAx_ms2").Value = (-25).ToString("F2");
        }

        public override string Name
        {
            get { return "AxMinimumAbove"; }
        }
        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {

            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name).Value;
            }

        }

    }

    public sealed class OpAxMinimum : OpAxSetModelValues
    {
        private static readonly OpAxMinimum _instance = new OpAxMinimum() { };
        public static OpAxMinimum GetInstance()
        {
            return _instance;
        }


        private OpAxMinimum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set below maximum Ax, default -25ms2 ";
            Parameters.First(x => x.Name == "TargetAx_ms2").Value = (-25).ToString("F2");
        }

        public override string Name
        {
            get { return "AxMinimum"; }
        }
        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {

            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name).Value;
            }

        }

    }

    public sealed class OpAxMinimumBelow : OpAxSetModelValues
    {
        private static readonly OpAxMinimumBelow _instance = new OpAxMinimumBelow() { };
        public static OpAxMinimumBelow GetInstance()
        {
            return _instance;
        }


        private OpAxMinimumBelow()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set below maximum Ax, default -26ms2 ";
            Parameters.First(x => x.Name == "TargetAx_ms2").Value = (-26).ToString("F2");
        }

        public override string Name
        {
            get { return "AxMinimumBelow"; }
        }
        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {

            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name).Value;
            }

        }

    }
    #endregion Ax value check

    #region Ax offset value check 
    //Task 460 ->20181102
    public sealed class OpAxOffsetTypicalValue : OpAxOffsetSetModelValues
    {
        private static readonly OpAxOffsetTypicalValue _instance = new OpAxOffsetTypicalValue() { };
        public static OpAxOffsetTypicalValue GetInstance()
        {
            return _instance;
        }


        private OpAxOffsetTypicalValue()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set typical Ax offset, default 1ms2 ";
            Parameters.First(x => x.Name == "TargetAx_ms2").Value = (0).ToString("F2");
        }
        public override string Name
        {
            get { return "AxOffsetTypicalValue"; }
        }
    }

    public sealed class OpAxOffsetMaximum : OpAxOffsetSetModelValues
    {
        private static readonly OpAxOffsetMaximum _instance = new OpAxOffsetMaximum() { };
        public static OpAxOffsetMaximum GetInstance()
        {
            return _instance;
        }


        private OpAxOffsetMaximum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set  maximum Ax offset, default 2ms2 ";
            Parameters.First(x => x.Name == "TargetAx_ms2").Value = (2).ToString("F2");
        }
        public override string Name
        {
            get { return "AxOffsetMaximum"; }
        }

        //public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        //{


        //    if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
        //    {
        //        // OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name).Value;

        //        OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = "4095";
        //    }

        //}
    }

    public sealed class OpAxOffsetMaximumAbove : OpAxOffsetSetModelValues
    {
        private static readonly OpAxOffsetMaximumAbove _instance = new OpAxOffsetMaximumAbove() { };
        public static OpAxOffsetMaximumAbove GetInstance()
        {
            return _instance;
        }


        private OpAxOffsetMaximumAbove()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set  above maximum Ax offset offset, default 3ms2 ";
            Parameters.First(x => x.Name == "TargetAx_ms2").Value = (3).ToString("F2");
        }
        public override string Name
        {
            get { return "AxOffsetAboveMaximum"; }
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {


            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {

                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = "869%10";
            }

        }
    }

    public sealed class OpAxOffsetMaximumBelow : OpAxOffsetSetModelValues
    {
        private static readonly OpAxOffsetMaximumBelow _instance = new OpAxOffsetMaximumBelow() { };
        public static OpAxOffsetMaximumBelow GetInstance()
        {
            return _instance;
        }


        private OpAxOffsetMaximumBelow()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set  below maximum Ax offset offset, default 1ms2 ";
            Parameters.First(x => x.Name == "TargetAx_ms2").Value = (1).ToString("F2");
        }
        public override string Name
        {
            get { return "AxOffsetBelowMaximum"; }
        }

    }

    public sealed class OpAxOffsetMinimum : OpAxOffsetSetModelValues
    {
        private static readonly OpAxOffsetMinimum _instance = new OpAxOffsetMinimum() { };
        public static OpAxOffsetMinimum GetInstance()
        {
            return _instance;
        }


        private OpAxOffsetMinimum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set  minimum Ax offset, default -2ms2 ";
            Parameters.First(x => x.Name == "TargetAx_ms2").Value = (-2).ToString("F2");
        }
        public override string Name
        {
            get { return "AxOffsetMinimum"; }
        }

        //public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        //{


        //    if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
        //    {
        //        OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = "4095";
        //    }

        //}
    }

    public sealed class OpAxOffsetBelowMinimum : OpAxOffsetSetModelValues
    {
        private static readonly OpAxOffsetBelowMinimum _instance = new OpAxOffsetBelowMinimum() { };
        public static OpAxOffsetBelowMinimum GetInstance()
        {
            return _instance;
        }


        private OpAxOffsetBelowMinimum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set below minimum Ax offset, default -3ms2 ";
            Parameters.First(x => x.Name == "TargetAx_ms2").Value = (-3).ToString("F2");
        }
        public override string Name
        {
            get { return "AxOffsetBelowMinimum"; }
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {


            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = "723%10";
            }

        }
    }

    public sealed class OpAxOffsetAboveMinimum : OpAxOffsetSetModelValues
    {
        private static readonly OpAxOffsetAboveMinimum _instance = new OpAxOffsetAboveMinimum() { };
        public static OpAxOffsetAboveMinimum GetInstance()
        {
            return _instance;
        }


        private OpAxOffsetAboveMinimum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set  above maximum Ax offset offset, default -1ms2 ";
            Parameters.First(x => x.Name == "TargetAx_ms2").Value = (-1).ToString("F2");
        }
        public override string Name
        {
            get { return "AxOffsetAboveMinimum"; }
        }

    }


    #endregion Ax offset value check

    #region Ay value check

    public sealed class OpAyMaximumAbove : OpAySetModelValues
    {
        private static readonly OpAyMaximumAbove _instance = new OpAyMaximumAbove() { };
        public static OpAyMaximumAbove GetInstance()
        {
            return _instance;
        }


        private OpAyMaximumAbove()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set above maximum Ay, default 100ms2 ";
            Parameters.First(x => x.Name == "TargetAy_ms2").Value = (100).ToString("F2");
        }
        public override string Name
        {
            get { return "AyAboveMaximum"; }
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {

            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name).Value;
            }

        }
    }

    public sealed class OpAyMaximum : OpAySetModelValues
    {
        private static readonly OpAyMaximum _instance = new OpAyMaximum() { };
        public static OpAyMaximum GetInstance()
        {
            return _instance;
        }


        private OpAyMaximum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set above maximum Ay, default 100ms2 ";
            Parameters.First(x => x.Name == "TargetAy_ms2").Value = (25).ToString("F2");
        }
        public override string Name
        {
            get { return "AyMaximum"; }
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {

            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name).Value;
            }

        }
    }

    public sealed class OpAyMaximumBelow : OpAySetModelValues
    {
        private static readonly OpAyMaximumBelow _instance = new OpAyMaximumBelow() { };
        public static OpAyMaximumBelow GetInstance()
        {
            return _instance;
        }


        private OpAyMaximumBelow()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set above maximum Ay, default 100ms2 ";
            Parameters.First(x => x.Name == "TargetAy_ms2").Value = (24).ToString("F2");
        }
        public override string Name
        {
            get { return "AyMaximumBelow"; }
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {

            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name).Value;
            }

        }
    }

    public sealed class OpAyTypicalValue : OpAySetModelValues
    {
        private static readonly OpAyTypicalValue _instance = new OpAyTypicalValue() { };
        public static OpAyTypicalValue GetInstance()
        {
            return _instance;
        }


        private OpAyTypicalValue()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set typical Ay, default 5ms2 ";
            Parameters.First(x => x.Name == "TargetAy_ms2").Value = (5).ToString("F2");
        }
        public override string Name
        {
            get { return "AyTypicalValue"; }
        }
    }

    public sealed class OpAyMinimumBelow : OpAySetModelValues
    {
        private static readonly OpAyMinimumBelow _instance = new OpAyMinimumBelow() { };
        public static OpAyMinimumBelow GetInstance()
        {
            return _instance;
        }


        private OpAyMinimumBelow()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set below maximum Ay, default 0ms2 ";
            Parameters.First(x => x.Name == "TargetAy_ms2").Value = (-26).ToString("F2");
        }

        public override string Name
        {
            get { return "AyMinimumBelow"; }
        }
        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {

            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name).Value;
            }

        }
    }

    public sealed class OpAyMinimum : OpAySetModelValues
    {
        private static readonly OpAyMinimum _instance = new OpAyMinimum() { };
        public static OpAyMinimum GetInstance()
        {
            return _instance;
        }


        private OpAyMinimum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set below maximum Ay, default 0ms2 ";
            Parameters.First(x => x.Name == "TargetAy_ms2").Value = (-25).ToString("F2");
        }

        public override string Name
        {
            get { return "AyMinimum"; }
        }
        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {

            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name).Value;
            }

        }
    }

    public sealed class OpAyMinimumAbove : OpAySetModelValues
    {
        private static readonly OpAyMinimumAbove _instance = new OpAyMinimumAbove() { };
        public static OpAyMinimumAbove GetInstance()
        {
            return _instance;
        }


        private OpAyMinimumAbove()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set below maximum Ay, default 0ms2 ";
            Parameters.First(x => x.Name == "TargetAy_ms2").Value = (-24).ToString("F2");
        }

        public override string Name
        {
            get { return "AyMinimumAbove"; }
        }
        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {

            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name).Value;
            }

        }
    }

    #endregion Ay value check

    #region Ay offset value check
    public sealed class OpAyOffsetTypicalValue : OpAyOffsetSetModelValues
    {
        private static readonly OpAyOffsetTypicalValue _instance = new OpAyOffsetTypicalValue() { };
        public static OpAyOffsetTypicalValue GetInstance()
        {
            return _instance;
        }


        private OpAyOffsetTypicalValue()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set typical Ay offset, default 3ms2 ";
            Parameters.First(x => x.Name == "TargetAx_ms2").Value = (0).ToString("F2");
        }
        public override string Name
        {
            get { return "AyOffsetTypicalValue"; }
        }
    }

    public sealed class OpAyOffsetMaximum : OpAyOffsetSetModelValues
    {
        private static readonly OpAyOffsetMaximum _instance = new OpAyOffsetMaximum() { };
        public static OpAyOffsetMaximum GetInstance()
        {
            return _instance;
        }


        private OpAyOffsetMaximum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set maximum Ay offset, default 2ms2 ";
            Parameters.First(x => x.Name == "TargetAx_ms2").Value = (2).ToString("F2");
        }
        public override string Name
        {
            get { return "AyOffsetMaximum"; }
        }

        //public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        //{


        //    if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
        //    {

        //        OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = "4095";
        //    }

        //}
    }

    public sealed class OpAyOffsetAboveMaximum : OpAyOffsetSetModelValues
    {
        private static readonly OpAyOffsetAboveMaximum _instance = new OpAyOffsetAboveMaximum() { };
        public static OpAyOffsetAboveMaximum GetInstance()
        {
            return _instance;
        }


        private OpAyOffsetAboveMaximum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set above maximum Ay offset, default 3ms2 ";
            Parameters.First(x => x.Name == "TargetAx_ms2").Value = (3).ToString("F2");
        }
        public override string Name
        {
            get { return "AyOffsetAboveMaximum"; }
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {


            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {

                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = "869%10";
            }

        }
    }

    public sealed class OpAyOffsetBelowMaximum : OpAyOffsetSetModelValues
    {
        private static readonly OpAyOffsetBelowMaximum _instance = new OpAyOffsetBelowMaximum() { };
        public static OpAyOffsetBelowMaximum GetInstance()
        {
            return _instance;
        }


        private OpAyOffsetBelowMaximum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set below maximum Ay offset, default 1ms2 ";
            Parameters.First(x => x.Name == "TargetAx_ms2").Value = (1).ToString("F2");
        }
        public override string Name
        {
            get { return "AyOffsetBelowMaximum"; }
        }

    }

    public sealed class OpAyOffsetMinimum : OpAyOffsetSetModelValues
    {
        private static readonly OpAyOffsetMinimum _instance = new OpAyOffsetMinimum() { };
        public static OpAyOffsetMinimum GetInstance()
        {
            return _instance;
        }


        private OpAyOffsetMinimum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set minimum Ay offset, default -2ms2 ";
            Parameters.First(x => x.Name == "TargetAx_ms2").Value = (-2).ToString("F2");
        }
        public override string Name
        {
            get { return "AyOffsetMinimum"; }
        }

        //public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        //{


        //    if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
        //    {

        //        OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = "4095";
        //    }

        //}
    }

    public sealed class OpAyOffsetBelowMinimum : OpAyOffsetSetModelValues
    {
        private static readonly OpAyOffsetBelowMinimum _instance = new OpAyOffsetBelowMinimum() { };
        public static OpAyOffsetBelowMinimum GetInstance()
        {
            return _instance;
        }


        private OpAyOffsetBelowMinimum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set below minimum Ay offset, default -3ms2 ";
            Parameters.First(x => x.Name == "TargetAx_ms2").Value = (-3).ToString("F2");
        }
        public override string Name
        {
            get { return "AyOffsetBelowMinimum"; }
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {


            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {

                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = "722%10";
            }

        }
    }

    public sealed class OpAyOffsetAboveMinimum : OpAyOffsetSetModelValues
    {
        private static readonly OpAyOffsetAboveMinimum _instance = new OpAyOffsetAboveMinimum() { };
        public static OpAyOffsetAboveMinimum GetInstance()
        {
            return _instance;
        }


        private OpAyOffsetAboveMinimum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set above minimum Ay offset, default -1ms2 ";
            Parameters.First(x => x.Name == "TargetAx_ms2").Value = (-1).ToString("F2");
        }
        public override string Name
        {
            get { return "AyOffsetAboveMinimum"; }
        }

    }

    #endregion Ay offset value check

    #region yawrate value check
    public sealed class OpYawrateMaximumAbove : OpYawrateSetModelValues
    {
        private static readonly OpYawrateMaximumAbove _instance = new OpYawrateMaximumAbove() { };
        public static OpYawrateMaximumAbove GetInstance()
        {
            return _instance;
        }


        private OpYawrateMaximumAbove()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set above maximum yawrate, default 100 degree per second ";
            Parameters.First(x => x.Name == "TargetYawrate_dps").Value = (26).ToString("F2");
        }
        public override string Name
        {
            get { return "YawrateAboveMaximum"; }
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {

            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name).Value;
            }

        }
    }

    public sealed class OpYawrateMaximum : OpYawrateSetModelValues
    {
        private static readonly OpYawrateMaximum _instance = new OpYawrateMaximum() { };
        public static OpYawrateMaximum GetInstance()
        {
            return _instance;
        }


        private OpYawrateMaximum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set above maximum yawrate, default 25 degree per second ";
            Parameters.First(x => x.Name == "TargetYawrate_dps").Value = (25).ToString("F2");
        }
        public override string Name
        {
            get { return "YawrateMaximum"; }
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {

            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name).Value;
            }

        }
    }

    public sealed class OpYawrateMaximumBelow : OpYawrateSetModelValues
    {
        private static readonly OpYawrateMaximumBelow _instance = new OpYawrateMaximumBelow() { };
        public static OpYawrateMaximumBelow GetInstance()
        {
            return _instance;
        }


        private OpYawrateMaximumBelow()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set above maximum yawrate, default 24 degree per second ";
            Parameters.First(x => x.Name == "TargetYawrate_dps").Value = (25).ToString("F2");
        }
        public override string Name
        {
            get { return "YawrateMaximumBelow"; }
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {

            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name).Value;
            }

        }
    }

    public sealed class OpYawrateTypicalValue : OpYawrateSetModelValues
    {
        private static readonly OpYawrateTypicalValue _instance = new OpYawrateTypicalValue() { };
        public static OpYawrateTypicalValue GetInstance()
        {
            return _instance;
        }


        private OpYawrateTypicalValue()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set typical yawrate, default 5 degree per second ";
            Parameters.First(x => x.Name == "TargetYawrate_dps").Value = (5).ToString("F2");
        }
        public override string Name
        {
            get { return "YawrateTypicalValue"; }
        }
    }

    public sealed class OpYawrateMinimumBelow : OpYawrateSetModelValues
    {
        private static readonly OpYawrateMinimumBelow _instance = new OpYawrateMinimumBelow() { };
        public static OpYawrateMinimumBelow GetInstance()
        {
            return _instance;
        }


        private OpYawrateMinimumBelow()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set below minimum yawrate, default -26 degree per second ";
            Parameters.First(x => x.Name == "TargetYawrate_dps").Value = (-26).ToString("F2");
        }

        public override string Name
        {
            get { return "YawrateMinimumBelow"; }
        }
    }

    public sealed class OpYawrateMinimum : OpYawrateSetModelValues
    {
        private static readonly OpYawrateMinimum _instance = new OpYawrateMinimum() { };
        public static OpYawrateMinimum GetInstance()
        {
            return _instance;
        }


        private OpYawrateMinimum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set below minimum yawrate, default -25 degree per second ";
            Parameters.First(x => x.Name == "TargetYawrate_dps").Value = (-25).ToString("F2");
        }

        public override string Name
        {
            get { return "YawrateMinimum"; }
        }
    }

    public sealed class OpYawrateMinimumAbove : OpYawrateSetModelValues
    {
        private static readonly OpYawrateMinimumAbove _instance = new OpYawrateMinimumAbove() { };
        public static OpYawrateMinimumAbove GetInstance()
        {
            return _instance;
        }


        private OpYawrateMinimumAbove()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set below minimum yawrate, default -24 degree per second ";
            Parameters.First(x => x.Name == "TargetYawrate_dps").Value = (-24).ToString("F2");
        }

        public override string Name
        {
            get { return "YawrateMinimumAbove"; }
        }
    }

    #endregion yawrate value check

    #region yawrate offset value check
    public sealed class OpOffsetYawrateTypicalValue : OpOffsetYawrateSetModelValues
    {
        private static readonly OpOffsetYawrateTypicalValue _instance = new OpOffsetYawrateTypicalValue() { };
        public static OpOffsetYawrateTypicalValue GetInstance()
        {
            return _instance;
        }


        private OpOffsetYawrateTypicalValue()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set typical yawrate offset, default 5 degree per second ";
            Parameters.First(x => x.Name == "TargetYawrate_dps").Value = (0).ToString("F2");
        }
        public override string Name
        {
            get { return "YawrateOffsetTypicalValue"; }
        }
    }

    public sealed class OpOffsetYawrateMaximum : OpOffsetYawrateSetModelValues
    {
        private static readonly OpOffsetYawrateMaximum _instance = new OpOffsetYawrateMaximum() { };
        public static OpOffsetYawrateMaximum GetInstance()
        {
            return _instance;
        }


        private OpOffsetYawrateMaximum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set maximum yawrate offset, default 0.09 degree per second ";
            Parameters.First(x => x.Name == "TargetYawrate_dps").Value = (0.09).ToString("F2");
        }
        public override string Name
        {
            get { return "YawrateOffsetMaximumValue"; }
        }
        //public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        //{


        //    if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
        //    {
        //        OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = "4095";
        //    }

        //}
    }

    public sealed class OpOffsetYawrateAboveMaximum : OpOffsetYawrateSetModelValues
    {
        private static readonly OpOffsetYawrateAboveMaximum _instance = new OpOffsetYawrateAboveMaximum() { };
        public static OpOffsetYawrateAboveMaximum GetInstance()
        {
            return _instance;
        }


        private OpOffsetYawrateAboveMaximum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set above maximum yawrate offset, default 20 degree per second ";
            Parameters.First(x => x.Name == "TargetYawrate_dps").Value = (0.1).ToString("F2");
        }
        public override string Name
        {
            get { return "YawrateOffsetAboveMaximumValue"; }
        }
        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {


            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = "4095";
            }

        }
    }

    public sealed class OpOffsetYawrateBelowMaximum : OpOffsetYawrateSetModelValues
    {
        private static readonly OpOffsetYawrateBelowMaximum _instance = new OpOffsetYawrateBelowMaximum() { };
        public static OpOffsetYawrateBelowMaximum GetInstance()
        {
            return _instance;
        }


        private OpOffsetYawrateBelowMaximum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set below maximum yawrate offset, default 0.05 degree per second ";
            Parameters.First(x => x.Name == "TargetYawrate_dps").Value = (0.05).ToString("F2");
        }
        public override string Name
        {
            get { return "YawrateOffsetBelowMaximumValue"; }
        }

    }

    public sealed class OpOffsetYawrateMinimum : OpOffsetYawrateSetModelValues
    {
        private static readonly OpOffsetYawrateMinimum _instance = new OpOffsetYawrateMinimum() { };
        public static OpOffsetYawrateMinimum GetInstance()
        {
            return _instance;
        }


        private OpOffsetYawrateMinimum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set minimum yawrate offset, default -0.09 degree per second ";
            Parameters.First(x => x.Name == "TargetYawrate_dps").Value = (-0.09).ToString("F2");
        }
        public override string Name
        {
            get { return "YawrateOffsetMinimumValue"; }
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {


            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = "4095";
            }

        }

    }

    public sealed class OpOffsetYawrateBelowMinimum : OpOffsetYawrateSetModelValues
    {
        private static readonly OpOffsetYawrateBelowMinimum _instance = new OpOffsetYawrateBelowMinimum() { };
        public static OpOffsetYawrateBelowMinimum GetInstance()
        {
            return _instance;
        }


        private OpOffsetYawrateBelowMinimum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set below minimum yawrate offset, default -0.1 degree per second ";
            Parameters.First(x => x.Name == "TargetYawrate_dps").Value = (-0.1).ToString("F2");
        }
        public override string Name
        {
            get { return "YawrateOffsetBelowMinimumValue"; }
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {


            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = "4095";
            }

        }

    }

    public sealed class OpOffsetYawrateAboveMinimum : OpOffsetYawrateSetModelValues
    {
        private static readonly OpOffsetYawrateAboveMinimum _instance = new OpOffsetYawrateAboveMinimum() { };
        public static OpOffsetYawrateAboveMinimum GetInstance()
        {
            return _instance;
        }


        private OpOffsetYawrateAboveMinimum()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set above minimum yawrate offset, default -0.05 degree per second ";
            Parameters.First(x => x.Name == "TargetYawrate_dps").Value = (-0.05).ToString("F2");
        }
        public override string Name
        {
            get { return "YawrateOffsetAboveMinimumValue"; }
        }


    }

    #endregion yawrate offset value check

    #region Pressure value check
    public sealed class OpPressureMaximumAbove : OpPressureSetModelValues
    {
        private static readonly OpPressureMaximumAbove _instance = new OpPressureMaximumAbove() { };

        public static OpPressureMaximumAbove GetInstance()
        {
            return _instance;
        }


        private OpPressureMaximumAbove()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set above maximum pressure, default 500 bar ";
            Parameters.First(x => x.Name == "TargetPressure_bar").Value = (500).ToString("F2");
        }
        public override string Name
        {
            get { return "PressureAboveMaximum"; }
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {

            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = "Input invalid values";
            }

        }
    }

    public sealed class OpPressureTypicalValue : OpPressureSetModelValues
    {
        private static readonly OpPressureTypicalValue _instance = new OpPressureTypicalValue() { };
        public static OpPressureTypicalValue GetInstance()
        {
            return _instance;
        }


        private OpPressureTypicalValue()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set typical pressure, default 100 bar ";
            Parameters.First(x => x.Name == "TargetPressure_bar").Value = (100).ToString("F2");
        }
        public override string Name
        {
            get { return "PressureTypicalValue"; }
        }
    }

    public sealed class OpPressureMinimumBelow : OpPressureSetModelValues
    {
        private static readonly OpPressureMinimumBelow _instance = new OpPressureMinimumBelow() { };
        public static OpPressureMinimumBelow GetInstance()
        {
            return _instance;
        }


        private OpPressureMinimumBelow()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set below minimum pressure, default 0 bar ";
            Parameters.First(x => x.Name == "TargetPressure_bar").Value = (-50).ToString("F2");
        }

        public override string Name
        {
            get { return "PressureMinimumBelow"; }
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {

            if (paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name) != null)
            {
                //OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = "0";
                OperationStateSignals.FirstOrDefault(w => w.Name == paraSet.FirstOrDefault(x => x.Name == "SourceCANTxSignal").Value).Value = paraSet.FirstOrDefault(x => x.Name == "OpSigValue_" + Name).Value;
            }

        }
    }

    #endregion Pressure value check

    public sealed class OpFullBrake : OpBrakeSetModelValues
    {
        private static readonly OpFullBrake _instance = new OpFullBrake() { };
        public static OpFullBrake GetInstance()
        {
            return _instance;
        }


        private OpFullBrake()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set full brake, default 1 ";
            Parameters.First(x => x.Name == "TargetBrake_perone").Value = (1).ToString("F2");
        }

        public override string Name
        {
            get { return "FullBrake"; }
        }


    }

    public sealed class OpBrakeRelease : OpBrakeSetModelValues
    {
        private static readonly OpBrakeRelease _instance = new OpBrakeRelease() { };
        public static OpBrakeRelease GetInstance()
        {
            return _instance;
        }


        private OpBrakeRelease()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set full brake, default 0 ";
            Parameters.First(x => x.Name == "TargetBrake_perone").Value = (0).ToString("F2");
        }

        public override string Name
        {
            get { return "BrakeRelease"; }
        }
    }

    public sealed class OpBrakePartial : OpBrakeSetModelValues
    {
        private static readonly OpBrakePartial _instance = new OpBrakePartial() { };
        public static OpBrakePartial GetInstance()
        {
            return _instance;
        }


        private OpBrakePartial()
        {
            Category = "SensorValueManipulation";
            Description = "This operation state is to set partial brake, default 0.5 ";
            Parameters.First(x => x.Name == "TargetBrake_perone").Value = (0.5).ToString("F2");
        }

        public override string Name
        {
            get { return "PartialBrake"; }
        }


    }
    #endregion SensorValueManipulation

    #region VAFS
    #region ESP VAFS
    /// <summary>
    /// HBA active
    /// </summary>
    public sealed class HBActive : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly HBActive _instance = new HBActive();


        private HBActive()
        {
            Category = "VAFS";
            Description = "This operation state is to activate HBA";
        }
        public override string Name
        {
            get { return "HBActive"; }
        }

        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigHBActive.GetInstance()); }
        }

        public static HBActive GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            //HBA active 
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon(Parameters[1].Value, 1, TriggerConditionSignal.Equal));

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string>() {"HBA_SysT_HBA_001.lcs" }),
                new Wait(30000),
                new MM6Stop(),
                new StimuliStop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    /// <summary>
    /// HBB Active
    /// </summary>
    public sealed class HBBActive : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly HBBActive _instance = new HBBActive();


        private HBBActive()
        {
            Category = "VAFS";
            Description = "This operation state is to activate HBB";
        }
        public override string Name
        {
            get { return "HBBActive"; }
        }

        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigHBBActive.GetInstance()); }

        }

        public static HBBActive GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            //HBB active 
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon(Parameters[1].Value, 1, TriggerConditionSignal.Equal));

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string>() {"HBB.lcs" }),
                new Wait(30000),
                new MM6Stop(),
                new StimuliStop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    /// <summary>
    /// HBC Active
    /// </summary>
    public sealed class HBCActive : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly HBCActive _instance = new HBCActive();


        private HBCActive()
        {
            Category = "VAFS";
            Description = "This operation state is to activate HBC";
            Parameters.Add(new CANTxParameter("vacuumValue", "Set Vacuum Value in ECM node based on projects Specs, such as -0.08", "-0.08", typeof(float)));
        }
        public override string Name
        {
            get { return "HBCActive"; }
        }

        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigHBCActive.GetInstance()); }
        }

        public static HBCActive GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            //HBA active 
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon(Parameters[1].Value, 1, TriggerConditionSignal.Equal));

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {
            string vacuumValue = paraSet.SingleOrDefault(x => x.Name == "vacuumValue").Value;

        }

        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation

            AssignParameterValues(paraSet);
            //setModel Values
            string vacuumValue = paraSet.SingleOrDefault(x => x.Name == "vacuumValue").Value;
            SetModelValues setModelValue;
            Dictionary<string, string> dic2Set = new Dictionary<string, string>();
            dic2Set.Add("Hyd_Vacuum", vacuumValue);
            setModelValue = new SetModelValues(dic2Set);
            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                setModelValue,
                new Wait(5000),
                new StimuliStart(new ObservableCollection<string>() { "HBA_SysT_HBA_001.lcs" }),
                new Wait(30000),
                new StimuliStop(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
                new UndoSetModelValues(),
            });
        }
    }

    /// <summary>
    /// HHC Active
    /// </summary>
    public sealed class HHCActive : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly HHCActive _instance = new HHCActive();


        private HHCActive()
        {
            Category = "VAFS";
            Description = "This operation state is to activate HHC";
        }
        public override string Name
        {
            get { return "HHCActive"; }
        }

        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigHHCActive.GetInstance()); }
        }

        public static HHCActive GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            //HHC active 
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon(Parameters[1].Value, 1, TriggerConditionSignal.Equal));

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string>() {"HHC.lcs" }),
                new Wait(30000),
                new StimuliStop(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    /// <summary>
    /// HAZ Active
    /// </summary>
    public sealed class HAZActive : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly HAZActive _instance = new HAZActive();


        private HAZActive()
        {
            Category = "VAFS";
            Description = "This operation state is to activate HAZ";
        }
        public override string Name
        {
            get { return "HAZActive"; }
        }

        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigHAZActive.GetInstance()); }
        }

        public static HAZActive GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            //HAZ active 
            //MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon("LCI_I_MV_LF_UP_A", 0.5m, TriggerConditionSignal.Greater), 10000, 1000);

            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon(Parameters[1].Value, 1, TriggerConditionSignal.Equal));
            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string>() {"HAZ_0006__Active_ABS.lcs" }),
                new Wait(30000),
                new StimuliStop(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    /// <summary>
    /// HDC Active
    /// </summary>
    public sealed class HDCActive : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly HDCActive _instance = new HDCActive();


        private HDCActive()
        {
            Category = "VAFS";
            Description = "This operation state is to activate HDC";
        }
        public override string Name
        {
            get { return "HDCActive"; }
        }

        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigHDCActive.GetInstance()); }
        }

        public static HDCActive GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            //HDC active 
            //MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon("LCI_I_MV_LF_UP_A", 0.5m, TriggerConditionSignal.Greater), 10000, 1000);
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon(Parameters[1].Value, 1, TriggerConditionSignal.Equal));

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string>() {"HDC.lcs" }),
                new Wait(30000),
                new StimuliStop(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    /// <summary>
    /// CDP Active
    /// </summary>
    public sealed class CDPActive : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly CDPActive _instance = new CDPActive();


        private CDPActive()
        {
            Category = "VAFS";
            Description = "This operation state is to activate CDP";
        }
        public override string Name
        {
            get { return "CDPActive"; }
        }

        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigCDPActive.GetInstance()); }
        }

        public static CDPActive GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            //HDC active 
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon(Parameters[1].Value, 1, TriggerConditionSignal.Greater));

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }



        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation

            // Change APB button value to 1(Apply) -> need to add product type -> todo
            Dictionary<string, string> modelValueApbButton = new Dictionary<string, string>
            {
                { "APB_apply", "1" }
            };
            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string>() {"10kph_in_5s.lcs" }),
                new Wait(10000),
                new SetModelValues(modelValueApbButton),
                new Wait(3000),
                new StimuliStop(),
                new UndoSetModelValues(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    /// <summary>
    /// AWB Active
    /// </summary>
    public sealed class AWBActive : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly AWBActive _instance = new AWBActive();

        #region set Model value variable
        public string _AWBLevel = "AWBlevel";
        public string _AWBRequest = "AWBRequest";

        string AWBLevel
        {
            get
            {
                return _AWBLevel;
            }
            set
            {
                _AWBLevel = value;
            }
        }

        string AWBRequest
        {
            get
            {
                return _AWBRequest;
            }
            set
            {
                _AWBRequest = value;
            }
        }

        #endregion

        private AWBActive()
        {
            Category = "VAFS";
            Description = "This operation state is to activate AWB";
            Parameters.Add(new CANTxParameter(AWBLevel, "Set AWBLevel to level4 based on projects Specs, such as 4", "4", typeof(int)));
            Parameters.Add(new CANTxParameter(AWBRequest, "Set AWBRequest based on projects Specs, such as 1", "1", typeof(int)));
        }
        public override string Name
        {
            get { return "AWBActive"; }
        }

        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigAWBActive.GetInstance()); }
        }

        public static AWBActive GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            //AWB active 
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon(Parameters[1].Value, 1, TriggerConditionSignal.Equal));

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {


            AWBLevel = paraSet.SingleOrDefault(x => x.Name == paraSet[3].Name).Value;

            AWBRequest = paraSet.SingleOrDefault(x => x.Name == paraSet[4].Name).Value;

        }

        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);

            // add optional RB and CU faults->08/03/2018
            testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "Wss_MonVDiff_RR", "Wss_SignalLost_FR", "Wss_SignalLost_FL", "Wss_SignalLost_RL" };
            testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "Wss_MonVDiff_RR", "Wss_SignalLost_FR", "Wss_SignalLost_FL", "Wss_SignalLost_RL" };

            //Parameter check
            AssignParameterValues(paraSet);
            string AWBLevel = paraSet.SingleOrDefault(x => x.Name == paraSet[3].Name).Value;
            string AWBRequest = paraSet.SingleOrDefault(x => x.Name == paraSet[4].Name).Value;
            SetModelValues setModelValue1;
            SetModelValues setModelValue2;
            //Assign Faults evaluation
            Dictionary<string, string> dic2Set1 = new Dictionary<string, string>
            {
                { paraSet[3].Name, AWBLevel }
            };
            Dictionary<string, string> dic2Set2 = new Dictionary<string, string>
            {
                { paraSet[4].Name, AWBRequest }
            };

            setModelValue1 = new SetModelValues(dic2Set1);

            setModelValue2 = new SetModelValues(dic2Set2);
            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                 new StimuliStart(new ObservableCollection<string>() {"50kph_in_10s.lcs" }),
                new Wait(10000),
                setModelValue1,
                new Wait(500),
                setModelValue2,
                new Wait(10000),
                new StimuliStop(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
                new UndoSetModelValues(),
            });
        }
    }

    /// <summary>
    /// AVH active
    /// </summary>
    public sealed class AVHActive : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly AVHActive _instance = new AVHActive();


        private AVHActive()
        {
            Category = "VAFS";
            Description = "This operation state is to activate AVH";
        }
        public override string Name
        {
            get { return "AVHActive"; }
        }

        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigAVHActive.GetInstance()); }
        }

        public static AVHActive GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            //HBA active 
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon(Parameters[1].Value, 1, TriggerConditionSignal.Equal));

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }


        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string>() {"VLC_Vehicle_Longitudinal_Control.lcs" }),
                new Wait(30000),
                new StimuliStop(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    /// <summary>
    /// ABP active
    /// </summary>
    public sealed class ABPActive : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly ABPActive _instance = new ABPActive();


        private ABPActive()
        {
            Category = "VAFS";
            Description = "This operation state is to activate ABP";
        }
        public override string Name
        {
            get { return "ABPActive"; }
        }

        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigABPActive.GetInstance()); }
        }

        public static ABPActive GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            //ABP active 
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon(Parameters[1].Value, 1, TriggerConditionSignal.Equal));

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }


        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check

            //Assign Faults evaluation

            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new StimuliStart(new ObservableCollection<string>() {"VLC_Vehicle_Longitudinal_Control.lcs" }),
                new Wait(30000),
                new StimuliStop(),
                new TraceStop(),
                new EcuOff(),
            });
        }
    }

    /// <summary>
    /// ABA active
    /// </summary>
    public sealed class ABActive : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly ABActive _instance = new ABActive();
        #region set Model value variable
        public string _ABAlevel = "ABAlevel";
        public string _ABARequest = "ABARequest";

        string ABAlevel
        {
            get
            {
                return _ABAlevel;
            }
            set
            {
                _ABAlevel = value;
            }
        }

        string ABARequest
        {
            get
            {
                return _ABARequest;
            }
            set
            {
                _ABARequest = value;
            }
        }

        #endregion

        private ABActive()
        {
            Category = "VAFS";
            Description = "This operation state is to activate ABA";
            Parameters.Add(new CANTxParameter(ABAlevel, "Set ABALevel to level4 based on projects Specs, such as 4", "3", typeof(int)));
            Parameters.Add(new CANTxParameter(ABARequest, "Set ABARequest based on projects Specs, such as 1", "1", typeof(int)));
        }
        public override string Name
        {
            get { return "ABActive"; }
        }

        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigABActive.GetInstance()); }
        }

        public static ABActive GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            //ABA active 
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon(Parameters[1].Value, 1, TriggerConditionSignal.Equal));

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {


            ABAlevel = paraSet.SingleOrDefault(x => x.Name == paraSet[3].Name).Value;

            ABARequest = paraSet.SingleOrDefault(x => x.Name == paraSet[4].Name).Value;

        }
        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check
            AssignParameterValues(paraSet);
            //Assign Faults evaluation
            string ABAlevel = paraSet.SingleOrDefault(x => x.Name == paraSet[3].Name).Value;
            string ABARequest = paraSet.SingleOrDefault(x => x.Name == paraSet[4].Name).Value;
            SetModelValues setModelValue1;
            SetModelValues setModelValue2;
            //Assign Faults evaluation
            Dictionary<string, string> dic2Set1 = new Dictionary<string, string>
            {
                { paraSet[3].Name, ABAlevel }
            };
            Dictionary<string, string> dic2Set2 = new Dictionary<string, string>
            {
                { paraSet[4].Name, ABARequest }
            };

            setModelValue1 = new SetModelValues(dic2Set1);

            setModelValue2 = new SetModelValues(dic2Set2);
            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new StimuliStart(new ObservableCollection<string>() {"50kph_in_10s.lcs" }),
                new Wait(10000),
                setModelValue1,
                new Wait(500),
                setModelValue2,
                new Wait(30000),
                new StimuliStop(),
                new TraceStop(),
                new EcuOff(),
                 new UndoSetModelValues(),
            });
        }
    }

    #endregion ESP VAFS

    #region DA VAFS
    /// <summary>
    /// VLC active
    /// </summary>
    public sealed class VLCActive : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly VLCActive _instance = new VLCActive();

        public string _axvCvAim = "MRR_2_axvCvAim";

        public string _accMode = "MRR_2_ACCMode";

        public string _dtUpperLimitAxvCv = "MRR_2_aDtUpperLimitAxvCv";

        public string _dtLowerLimitAxvCv = "MRR_2_aDtLowerLimitAxvCv";

        string axvCvAim
        {
            get
            {
                return _axvCvAim;
            }

            set
            {
                _axvCvAim = value;
            }

        }

        string accMode
        {
            get
            {
                return _accMode;
            }

            set
            {
                _accMode = value;
            }

        }

        string DtUpperLimitAxvCv
        {
            get
            {
                return _dtUpperLimitAxvCv;
            }
            set
            {
                _dtUpperLimitAxvCv = value;
            }
        }

        string DtLowerLimitAxvCv
        {

            get
            {
                return _dtLowerLimitAxvCv;
            }
            set
            {
                _dtLowerLimitAxvCv = value;
            }
        }

        private VLCActive()
        {
            Category = "VAFS";
            Description = "This operation state is to activate VLC";
            Parameters.Add(new CANTxParameter(axvCvAim, "Set ACC deceleration request speed based on projects Specs, such as -3", "-3", typeof(int)));
            Parameters.Add(new CANTxParameter(accMode, "Set ACC Mode based on projects Specs, such as 3", "3", typeof(int)));
            Parameters.Add(new CANTxParameter(DtUpperLimitAxvCv, "set Upper limit value", "10", typeof(int)));
            Parameters.Add(new CANTxParameter(DtLowerLimitAxvCv, "set Upper limit value", "-10", typeof(int)));
        }
        public override string Name
        {
            get { return "VLCActive"; }
        }

        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigVLCActive.GetInstance()); }
        }

        public static VLCActive GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            //HBA active 
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon(Parameters[1].Value, 1, TriggerConditionSignal.Equal));

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {


            axvCvAim = paraSet.SingleOrDefault(x => x.Name == paraSet[3].Name).Value;

            accMode = paraSet.SingleOrDefault(x => x.Name == paraSet[4].Name).Value;

            DtUpperLimitAxvCv = paraSet.SingleOrDefault(x => x.Name == paraSet[5].Name).Value;

            DtLowerLimitAxvCv = paraSet.SingleOrDefault(x => x.Name == paraSet[6].Name).Value;

        }

        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check
            // add optional RB and CU faults->08/01/2018
            testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "Wss_MoreThanOneSuspected" };
            testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "Wss_MoreThanOneSuspected" };

            //Assign Faults evaluation
            AssignParameterValues(paraSet);
            string axvCvAim = paraSet.SingleOrDefault(x => x.Name == paraSet[3].Name).Value;
            string accMode = paraSet.SingleOrDefault(x => x.Name == paraSet[4].Name).Value;
            string dtUpperLimitAxvCv = paraSet.SingleOrDefault(x => x.Name == paraSet[5].Name).Value;
            string dtLowerLimitAxvCv = paraSet.SingleOrDefault(x => x.Name == paraSet[6].Name).Value;
            SetModelValues setModelValue1;
            SetModelValues setModelValue2;
            SetModelValues setModelValue3;
            SetModelValues setModelValue4;
            Dictionary<string, string> dic2Set1 = new Dictionary<string, string>
            {
                { paraSet[3].Name, axvCvAim }
            };
            Dictionary<string, string> dic2Set2 = new Dictionary<string, string>
            {
                { paraSet[4].Name, accMode }
            };

            Dictionary<string, string> dic3Set3 = new Dictionary<string, string>
            {
                { paraSet[5].Name, dtUpperLimitAxvCv }
            };

            Dictionary<string, string> dic4Set4 = new Dictionary<string, string>
            {
                { paraSet[6].Name, dtLowerLimitAxvCv }
            };


            setModelValue1 = new SetModelValues(dic2Set1);

            setModelValue2 = new SetModelValues(dic2Set2);

            setModelValue3 = new SetModelValues(dic3Set3);

            setModelValue4 = new SetModelValues(dic4Set4);


            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string>() {"50kph_in_10s.lcs" }),
                new Wait(10000),
                setModelValue1,
                new Wait(500),
                setModelValue2,
                new Wait(500),
                setModelValue3,
                 new Wait(500),
                setModelValue4,
                new Wait(10000),           
                new StimuliStop(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
                new UndoSetModelValues(),
            });
        }
    }

    /// <summary>
    /// CDD active
    /// </summary>
    public sealed class CDDActive : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly CDDActive _instance = new CDDActive();

        public string _MRR_2_axvCvAim = "MRR_2_axvCvAim";

        public string _MRR_2_ACCMode = "MRR_2_ACCMode";

        public string _MRR_2_aDtUpperLimitAxvCv = "MRR_2_aDtUpperLimitAxvCv";

        public string _MRR_2_aDtLowerLimitAxvCv = "MRR_2_aDtLowerLimitAxvCv";

        string MRR_2_axvCvAim
        {
            get
            {
                return _MRR_2_axvCvAim;
            }

            set
            {
                _MRR_2_axvCvAim = value;
            }

        }

        string MRR_2_ACCMode
        {
            get
            {
                return _MRR_2_ACCMode;
            }

            set
            {
                _MRR_2_ACCMode = value;
            }

        }

        string MRR_2_aDtUpperLimitAxvCv
        {

            get
            {
                return _MRR_2_aDtUpperLimitAxvCv;
            }
            set
            {
                _MRR_2_aDtUpperLimitAxvCv = value;
            }
        }

        string MRR_2_aDtLowerLimitAxvCv
        {
            get
            {
                return _MRR_2_aDtLowerLimitAxvCv;
            }
            set
            {
                _MRR_2_aDtLowerLimitAxvCv = value;
            }
        }

        private CDDActive()
        {
            Category = "VAFS";
            Description = "This operation state is to activate CDD";
            Parameters.Add(new CANTxParameter(MRR_2_axvCvAim, "Set ACC deceleration request speed based on projects Specs, such as -3", "-3", typeof(int)));
            Parameters.Add(new CANTxParameter(MRR_2_ACCMode, "Set ACC mode based on projects Specs, such as 3", "3", typeof(int)));
            Parameters.Add(new CANTxParameter(MRR_2_aDtUpperLimitAxvCv, "Set MRR_2_aDtUpperLimitAxvCv such as 10", "10", typeof(int)));
            Parameters.Add(new CANTxParameter(MRR_2_aDtLowerLimitAxvCv, "Set MRR_2_aDtLowerLimitAxvCv such as 1-0", "-10", typeof(int)));

        }
        public override string Name
        {
            get { return "CDDActive"; }
        }

        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigCDDActive.GetInstance()); }
        }

        public static CDDActive GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            //HBA active 
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon(Parameters[1].Value, 1, TriggerConditionSignal.Equal));

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {


            MRR_2_axvCvAim = paraSet.SingleOrDefault(x => x.Name == paraSet[3].Name).Value;

            MRR_2_ACCMode = paraSet.SingleOrDefault(x => x.Name == paraSet[4].Name).Value;

            MRR_2_aDtUpperLimitAxvCv = paraSet.SingleOrDefault(x => x.Name == paraSet[5].Name).Value;

            MRR_2_aDtLowerLimitAxvCv = paraSet.SingleOrDefault(x => x.Name == paraSet[6].Name).Value;

        }

        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check
            // add optional RB and CU faults->08/03/2018
            testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "Wss_MoreThanOneSuspected" };
            testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "Wss_MoreThanOneSuspected" };
            //Assign Faults evaluation
            AssignParameterValues(paraSet);
            string MRR_2_axvCvAim = paraSet.SingleOrDefault(x => x.Name == paraSet[3].Name).Value;
            string MRR_2_ACCMode = paraSet.SingleOrDefault(x => x.Name == paraSet[4].Name).Value;

            string MRR_2_aDtUpperLimitAxvCv = paraSet.SingleOrDefault(x => x.Name == paraSet[5].Name).Value;

            string MRR_2_aDtLowerLimitAxvCv = paraSet.SingleOrDefault(x => x.Name == paraSet[6].Name).Value;

            SetModelValues setModelValue1;
            SetModelValues setModelValue2;
            SetModelValues setModelValue3;
            SetModelValues setModelValue4;
            Dictionary<string, string> dic2Set1 = new Dictionary<string, string>
            {
                { paraSet[3].Name, MRR_2_axvCvAim }
            };
            Dictionary<string, string> dic2Set2 = new Dictionary<string, string>
            {
                { paraSet[4].Name, MRR_2_ACCMode }
            };

            Dictionary<string, string> dic3Set3 = new Dictionary<string, string>
            {
                { paraSet[5].Name, MRR_2_aDtUpperLimitAxvCv }
            };

            Dictionary<string, string> dic4Set4 = new Dictionary<string, string>
            {
                { paraSet[6].Name, MRR_2_aDtLowerLimitAxvCv }
            };
            setModelValue1 = new SetModelValues(dic2Set1);

            setModelValue2 = new SetModelValues(dic2Set2);

            setModelValue3 = new SetModelValues(dic3Set3);

            setModelValue4 = new SetModelValues(dic4Set4);
            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string>() {"50kph_in_10s.lcs" }),
                new Wait(10000),
                setModelValue1,
                new Wait(500),
                setModelValue2,
                new Wait(500),
                setModelValue3,
                new Wait(500),
                setModelValue4,
                new Wait(10000),
                new StimuliStop(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
                new UndoSetModelValues(),
            });
        }
    }

    /// <summary>
    /// AEB active
    /// </summary>
    public sealed class AEBActive : OpState
    {
        private ItemsChangeObservableCollection<CanTxSignalType> afftectsignals = new ItemsChangeObservableCollection<CanTxSignalType>();
        private static readonly AEBActive _instance = new AEBActive();

        public string _AEBDecCtrlRequest = "AEBDecCtrlRequest";

        public string _ACCAEBBAAct = "ACCAEBBAAct";

        public string _AEBaxTar = "AEBaxTar";

        public string AEBDecCtrlRequest
        {
            get
            {
                return _AEBDecCtrlRequest;
            }

            set
            {
                _AEBDecCtrlRequest = value;
            }

        }

        public string ACCAEBBAAct
        {
            get
            {
                return _ACCAEBBAAct;
            }

            set
            {
                _ACCAEBBAAct = value;
            }

        }
        public string AEBaxTar
        {
            get
            {
                return _AEBaxTar;
            }
            set
            {
                _AEBaxTar = value;
            }
        }


        private AEBActive()
        {
            Category = "VAFS";
            Description = "This operation state is to activate AEB";
            Parameters.Add(new CANTxParameter(AEBDecCtrlRequest, "Set AEB  deceleration request  based on projects Specs, such as -10", "1", typeof(int)));
            Parameters.Add(new CANTxParameter(ACCAEBBAAct, "Set ACCAEBBAAct based on projects Specs, such as 3", "0", typeof(int)));
            Parameters.Add(new CANTxParameter(AEBaxTar, "Set ACCAEBBAAct based on projects Specs, such as 3", "-7", typeof(int)));

        }
        public override string Name
        {
            get { return "AEBActive"; }
        }

        public override ItemsChangeObservableCollection<CanTxSignalType> AffectedSignals
        {
            get { return afftectsignals; }
            set { afftectsignals.Add(SigAEBActive.GetInstance()); }
        }

        public static AEBActive GetInstance()
        {
            return _instance;
        }

        public override MeasurementPoint GetMeasurementPoint(ObservableCollection<CANTxParameter> Parameters, ref TestScript testScript)
        {
            //AEB active 
            MeasurementPoint tempMEP = new ReadCanSignalOverCondition(new SigConditon(Parameters[1].Value, 1, TriggerConditionSignal.Equal));

            tempMEP.SignalList.ValueList = new ObservableCollection<string>(OperationStateSignals.Select(x => x.Name));
            tempMEP.CanOpState.ValueList = new ObservableCollection<string>() { Name };
            return tempMEP;
        }

        public override void AssignParameterValues(ObservableCollection<CANTxParameter> paraSet)
        {


            AEBDecCtrlRequest = paraSet.SingleOrDefault(x => x.Name == paraSet[3].Name).Value;

            ACCAEBBAAct = paraSet.SingleOrDefault(x => x.Name == paraSet[4].Name).Value;

            AEBaxTar = paraSet.SingleOrDefault(x => x.Name == paraSet[5].Name).Value;

        }

        public override TestSequence ParameterizeTestSequence(ObservableCollection<CANTxParameter> paraSet, ref TestScript testScript)
        {
            //Add default operation state signals
            AddDefaultSignal2OpState(paraSet, Name);
            //Parameter check
            // add optional RB and CU faults->08/03/2018
            testScript.CUOptionalFaults.ValueList = new ObservableCollection<string>() { "Wss_MoreThanOneSuspected" };
            testScript.RBOptionalFaults.ValueList = new ObservableCollection<string>() { "Wss_MoreThanOneSuspected" };
            //Assign Faults evaluation
            AssignParameterValues(paraSet);
            string AEBDecCtrlRequest = paraSet.SingleOrDefault(x => x.Name == paraSet[3].Name).Value;
            string ACCAEBBAAct = paraSet.SingleOrDefault(x => x.Name == paraSet[4].Name).Value;
            string AEBaxTar = paraSet.SingleOrDefault(x => x.Name == paraSet[5].Name).Value;
            SetModelValues setModelValue1;
            SetModelValues setModelValue2;
            SetModelValues setModelValue3;
            Dictionary<string, string> dic2Set1 = new Dictionary<string, string>
            {
                { paraSet[3].Name, AEBDecCtrlRequest }
            };
            Dictionary<string, string> dic2Set2 = new Dictionary<string, string>
            {
                { paraSet[4].Name, ACCAEBBAAct }
            };
            Dictionary<string, string> dic3Set3 = new Dictionary<string, string>
            {
                { paraSet[5].Name, ACCAEBBAAct }
            };

            setModelValue1 = new SetModelValues(dic2Set1);

            setModelValue2 = new SetModelValues(dic2Set2);

            setModelValue3 = new SetModelValues(dic3Set3);
            //Parameterize
            return new TestSequence(new ObservableCollection<Keyword>()
            {
                new EcuOn(),
                new TraceStart(),
                new Wait(5000),
                new MM6Start("9","Normal"),
                new StimuliStart(new ObservableCollection<string>() {"50kph_in_10s.lcs" }),
                new Wait(10000),
                setModelValue1,
                new Wait(500),
                setModelValue2,
                new Wait(500),
                setModelValue3,
                new Wait(10000),
                new StimuliStop(),
                new MM6Stop(),
                new TraceStop(),
                new EcuOff(),
                new UndoSetModelValues(),
            });
        }
    }

    #endregion DA VAFS

    #endregion VAFS
}