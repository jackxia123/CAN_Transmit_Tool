using RBT.Universal;
using RBT.Universal.CanEvalParameters;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace OpStates
{
    enum SpeedUnit
    {
        kph,
        mps,

    }

    enum PressureUnit
    {
        bar,
        pascal,

    }

    #region BaseSignalTypes

    public class MessageTargetType : CanTxSignalType
    {

        private ObservableCollection<OpState> _testableOpStates = new ObservableCollection<OpState>();

        protected MessageTargetType()
        {

            Category = "BaseSignalTypes";
            Parameters.Add(new CANTxParameter("SourceCANTxSignal", "CAN message name for current project", "", typeof(string)));
            Parameters.Add(new CANTxParameter("CycleTime", "Pattern for current operation state, remember to ommit init value ", "10", typeof(int)));
            Parameters.Add(new CANTxParameter("TolerancePercentage", "Tolerance in percentage", "10", typeof(int)));

        }

        public override string Name
        {
            get { return "BooleanSignalType"; }
        }

        public override CanTraceAnalyser ParameterizeCanTraceAnalyser(ObservableCollection<CANTxParameter> paraSet)
        {

            if (paraSet.SingleOrDefault(x => x.Name == "SourceCANTxSignal") == null || paraSet.SingleOrDefault(x => x.Name == "CycleTime") == null || paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage") == null)
            {

                throw new FormatException(string.Format("Error when parameterizng signal type {0}: Either message name or message cycle not provided", Name));
            }
            else
            {

                return new CanTraceAnalyser()
                {


                    CycleTimeCheck = new CycleTimeCheck(paraSet.SingleOrDefault(x => x.Name == "SourceCANTxSignal").Value, (Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "CycleTime").Value) * (1 - Convert.ToDouble((paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value)) / 100))
                    + "-" + (Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "CycleTime").Value) * (1 + Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value) / 100))),
                };

            }
        }
    }

    public class BoolTargetSignalType : CanTxSignalType
    {

        private ObservableCollection<OpState> _testableOpStates = new ObservableCollection<OpState>();

        protected BoolTargetSignalType()
        {

            Category = "BaseSignalTypes";
            Parameters.Add(new CANTxParameter("SourceCANTxSignal", "CAN signal name for current project", "", typeof(string)));

            Parameters.Add(new CANTxParameter("SignalPattern", "Pattern for current operation state, remember to ommit init value ", "0->1->0", typeof(string)));
        }

        public override string Name
        {
            get { return "BooleanSignalType"; }
        }

        public override CanTraceAnalyser ParameterizeCanTraceAnalyser(ObservableCollection<CANTxParameter> paraSet)
        {

            if (paraSet.SingleOrDefault(x => x.Name == "SourceCANTxSignal") == null || paraSet.SingleOrDefault(x => x.Name == "SignalPattern") == null)
            {

                throw new FormatException(string.Format("Error when parmeterizng signal type {0}: Either signal name or signal pattern not provided", Name));
            }
            else
            {

                return new CanTraceAnalyser()
                {
                    SigPatternAnalyser = new SigPatternAnalyser(paraSet.First(x => x.Name == "SourceCANTxSignal").Value, paraSet.SingleOrDefault(x => x.Name == "SignalPattern").Value),
                };

            }
        }
    }

    /// <summary>
    /// All value check regarding sensors, speed, ax, ay,yawrate,pressure
    /// </summary>
    public class ValueTargetSignalType : CanTxSignalType
    {


        private ObservableCollection<OpState> _testableOpStates = new ObservableCollection<OpState>();

        protected ValueTargetSignalType()
        {

            Category = "BaseSignalTypes";
            Parameters.Add(new CANTxParameter("SourceCANTxSignal", "CAN signal name for current project", "", typeof(string)));
            Parameters.Add(new CANTxParameter("TolerancePercentage", "Tolerance in percentage", "10", typeof(int)));
            Parameters.Add(new CANTxParameter("SignalUnit", "The unit of the signal supported, for speed, kph or mps, for AxAy, ms2 or g, for yawrate, rps or dps, for pressure, bar or pa,for brakepostion, perone or percentage", "", typeof(SpeedUnit)));
            Parameters.Add(new CANTxParameter("Offset", "Copy dbc values, update if needed", "", typeof(double)));
            Parameters.Add(new CANTxParameter("Factor", "Copy dbc values, update if needed", "0->1->0", typeof(double)));
        }

        public override string Name
        {
            get { return "ValueSignalType"; }
        }

        public override CanTraceAnalyser ParameterizeCanTraceAnalyser(ObservableCollection<CANTxParameter> paraSet)
        {

            if (paraSet.SingleOrDefault(x => x.Name == "SourceCANTxSignal") == null)
            {

                throw new FormatException(string.Format("Error when parmeterizng signal type {0}: Either signal name or signal pattern not provided", Name));
            }
            else
            {

                return new CanTraceAnalyser();

            }
        }
    }

    #endregion BaseSignalTypes

    #region ControllerState
    public sealed class SigEBDActive : BoolTargetSignalType
    {
        private static readonly SigEBDActive _instance = new SigEBDActive();
        public static SigEBDActive GetInstance()
        {
            return _instance;
        }

        private SigEBDActive()
        {

            Category = "ControllerState";
            Description = "This signal is a boolean value, which indicate the ABS function is on or not";

            //Parameters for this signal type ABSActive

        }

        public override string Name
        {
            get { return "EBDActive"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { OpEBDActive.GetInstance(), }; }
        }

    }

    public sealed class SigEBDFailSts : BoolTargetSignalType
    {
        private static readonly SigEBDFailSts _instance = new SigEBDFailSts();
        public static SigEBDFailSts GetInstance()
        {
            return _instance;
        }

        private SigEBDFailSts()
        {
            Category = "ControllerState";
            Description = "This signal is a boolean value, which indicate the EBD function is shutdown";

        }

        public override string Name
        {
            get { return "EBDFailSts"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { OpOneValveInterrupt.GetInstance(), }; }
        }
    }

    public sealed class SigABSActive : BoolTargetSignalType
    {

        private static readonly SigABSActive _instance = new SigABSActive();
        public static SigABSActive GetInstance()
        {

            return _instance;
        }


        private SigABSActive()
        {
            Category = "ControllerState";
            Description = "This signal is a boolean value, which indicate the ABS function is on or not";

        }

        public override string Name
        {
            get { return "ABSActive"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { OpABSActive.GetInstance(), }; }
        }

    }

    public sealed class SigABSFailSts : BoolTargetSignalType
    {
        private static readonly SigABSFailSts _instance = new SigABSFailSts();
        public static SigABSFailSts GetInstance()
        {
            return _instance;
        }

        private SigABSFailSts()
        {
            Category = "ControllerState";
            Description = "This signal is a boolean value, which indicate the ABS function is shutdown";

        }

        public override string Name
        {
            get { return "ABSFailSts"; }
        }
    }

    public sealed class SigTCSActive : BoolTargetSignalType
    {

        private static readonly SigTCSActive _instance = new SigTCSActive();
        public static SigTCSActive GetInstance()
        {

            return _instance;
        }



        private SigTCSActive()
        {

            Category = "ControllerState";
            Description = "This signal is a boolean value, which indicate the TCS(AMR or BMR) function is on or not";

        }

        public override string Name
        {
            get { return "TCSActive"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { OpTCSActive.GetInstance(), }; }
        }

    }

    public sealed class SigTCSFailSts : BoolTargetSignalType
    {
        private static readonly SigTCSFailSts _instance = new SigTCSFailSts();
        public static SigTCSFailSts GetInstance()
        {
            return _instance;
        }

        private SigTCSFailSts()
        {
            Category = "ControllerState";
            Description = "This signal is a boolean value, which indicate the TCS function is shutdown";

        }

        public override string Name
        {
            get { return "TCSFailSts"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { OpAxMaximumBelow.GetInstance(), }; }
        }
    }

    public sealed class SigVDCActive : BoolTargetSignalType
    {
        private static readonly SigVDCActive _instance = new SigVDCActive();
        public static SigVDCActive GetInstance()
        {

            return _instance;
        }

        private SigVDCActive()
        {

            Category = "ControllerState";
            Description = "This signal is a boolean value, which indicate the VDC function is on or not";

        }

        public override string Name
        {
            get { return "VDCActive"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { OpVDCActive.GetInstance(), }; }
        }
    }

    public sealed class SigVDCFailSts : BoolTargetSignalType
    {
        private static readonly SigVDCFailSts _instance = new SigVDCFailSts();
        public static SigVDCFailSts GetInstance()
        {
            return _instance;
        }

        private SigVDCFailSts()
        {
            Category = "ControllerState";
            Description = "This signal is a boolean value, which indicate the VDC function is shutdown";

        }

        public override string Name
        {
            get { return "VDCFailSts"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { OpYawrateMaximumAbove.GetInstance(), }; }
        }
    }

    public sealed class SigEspFuncOff : BoolTargetSignalType
    {


        private static readonly SigEspFuncOff _instance = new SigEspFuncOff();
        public static SigEspFuncOff GetInstance()
        {
            return _instance;
        }

        private SigEspFuncOff()
        {
            Category = "ControllerState";
            Description = "This signal is a boolean value, which indicate the VDC function is disabled(PATA on)";

        }

        public override string Name
        {
            get { return "EspFuncOff"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { OpESPFuncOff.GetInstance(), OpPataStuck.GetInstance() }; }
        }
    }

    #endregion ControllerState

    /// <summary>
    /// Todo: Seperate each wheels?
    /// </summary>
    #region SpeedSignals
    public sealed class SigWheelSpdValid : BoolTargetSignalType
    {
        private static readonly SigWheelSpdValid _instance = new SigWheelSpdValid();
        public static SigWheelSpdValid GetInstance()
        {

            return _instance;
        }


        private SigWheelSpdValid()
        {

            Category = "SpeedSignals";
            Description = "This signal is a boolean value, which indicate the WheelSpd signal is valid or not,this can be used to indicate Wss edgeValid";
            // Parameters.Add(new CANTxParameter("SignalPattern", "Pattern for current operation state, remember to ommit init value ", "0->1", typeof(string)));
            //Parameters.RemoveAt(2);
            //Parameters.Add(new CANTxParameter("SignalPattern", "Pattern for current operation state, remember to ommit init value ", "0->1", typeof(string)));

        }

        public override string Name
        {
            get { return "WheelSpeedValid"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { }; }
        }
    }

    public sealed class SigVehicleSpdValid : BoolTargetSignalType
    {
        private static readonly SigVehicleSpdValid _instance = new SigVehicleSpdValid();
        public static SigVehicleSpdValid GetInstance()
        {

            return _instance;
        }

        private SigVehicleSpdValid()
        {

            Category = "SpeedSignals";
            Description = "This signal is a boolean value, which indicate the VehicleSpeed signal is valid or not,this can be used to indicate Odo signal valid";

        }

        public override string Name
        {
            get { return "VehicleSpeedValid"; }
        }

        /// <summary>
        /// Todo: More interupt scenario
        /// </summary>
        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { }; }
        }
    }

    /// <summary>
    /// Todo: Seperate each wheel?
    /// </summary>
    public sealed class SigWheelSpeed : ValueTargetSignalType
    {
        private static readonly SigWheelSpeed _instance = new SigWheelSpeed();
        public static SigWheelSpeed GetInstance()
        {

            return _instance;
        }


        private SigWheelSpeed()
        {

            Category = "SpeedSignals";
            Description = "This signal is the wheel speed signal, which indicate the value for each wheel depend on the unit";
            Parameters.Add(new CANTxParameter("Wheel", "Wheel LF or RF or RL or RR", "LF", typeof(string)));
            Parameters.First(x => x.Name == "SignalUnit").Value = "kph";

        }

        public override string Name
        {
            get { return "WheelSpeed"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get
            {
                return new ObservableCollection<OpState>() {  OpVehicleSpeedMinimumBelow.GetInstance(), OpVehicleSpeedMinimumAbove.GetInstance(),
                                                                OpVehicleSpeedMaximumBelow.GetInstance(), OpVehicleSpeedMaximumAbove.GetInstance(),
                                                                OpVehicleSpeedTypical.GetInstance(),};
            }
        }

    }

    public sealed class SigVehicleSpeed : ValueTargetSignalType
    {
        private static readonly SigVehicleSpeed _instance = new SigVehicleSpeed();
        public static SigVehicleSpeed GetInstance()
        {

            return _instance;
        }


        private SigVehicleSpeed()
        {

            Category = "SpeedSignals";
            Description = "This signal is the wheel speed signal, which indicate the value for each wheel depend on the unit";
            Parameters.Add(new CANTxParameter("DrivenType", "FWD or RWD", "FWD", typeof(string)));
            Parameters.First(x => x.Name == "SignalUnit").Value = "kph";

        }

        public override string Name
        {
            get { return "VehicleSpeed"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get
            {
                return new ObservableCollection<OpState>() {  OpVehicleSpeedMinimumBelow.GetInstance(), OpVehicleSpeedMinimumAbove.GetInstance(),
                                                                OpVehicleSpeedMaximumBelow.GetInstance(), OpVehicleSpeedMaximumAbove.GetInstance(),
                                                                OpVehicleSpeedTypical.GetInstance(),};
            }
        }
    }

    /// <summary>
    /// odo value check
    /// </summary>
    public sealed class OdoValueTargetSignalType : CanTxSignalType
    {


        private ObservableCollection<OpState> _testableOpStates = new ObservableCollection<OpState>();
        private static readonly OdoValueTargetSignalType _instance = new OdoValueTargetSignalType();

        public static OdoValueTargetSignalType GetInstance()
        {

            return _instance;
        }

        private OdoValueTargetSignalType()
        {

            Category = "SpeedSignals";
            Description = "This type is used to check the odo in meters, use calculate count up to check the signal accuracy";
            Parameters.Add(new CANTxParameter("SourceCANTxSignal", "CAN signal name for current project", "", typeof(string)));
            Parameters.Add(new CANTxParameter("TolerancePercentage", "Tolerance in percentage", "10", typeof(int)));
            Parameters.Add(new CANTxParameter("DrivenType", "FWD or RWD", "FWD", typeof(string)));

        }

        public override string Name
        {
            get { return "VehicleOdoInMeter"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get
            {
                return new ObservableCollection<OpState>() { OpNormalDrivingInit0S.GetInstance(), OpNormalDriving50kph.GetInstance(), };
            }
        }


        public override CanTraceAnalyser ParameterizeCanTraceAnalyser(ObservableCollection<CANTxParameter> paraSet)
        {
            string sigName = paraSet.SingleOrDefault(x => x.Name == "SourceCANTxSignal").Value;
            double tor = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value);



            //parameter check for opstate releated parameters

            if (paraSet.SingleOrDefault(x => x.Name == "StartTime") == null || paraSet.SingleOrDefault(x => x.Name == "TargetSpeed_mps") == null)
            {
                return new CanTraceAnalyser()
                {
                };

            }
            else
            {
                double speed = 0;

                if (Regex.IsMatch(paraSet.SingleOrDefault(x => x.Name == "TargetSpeed_mps").Value, ";"))
                {
                    string drivenType = paraSet.SingleOrDefault(x => x.Name == "DrivenType").Value;
                    string[] speeds = paraSet.SingleOrDefault(x => x.Name == "TargetSpeed_mps").Value.Split(Char.Parse(";"));


                    if (drivenType == "FWD")
                    {
                        speed = (Convert.ToDouble(speeds[0]) + Convert.ToDouble(speeds[1])) / 2;

                    }
                    else if (drivenType == "RWD")
                    {
                        speed = (Convert.ToDouble(speeds[2]) + Convert.ToDouble(speeds[3])) / 2;
                    }


                }
                else
                {
                    speed = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "TargetSpeed_mps").Value);

                }


                string startTime = paraSet.SingleOrDefault(x => x.Name == "StartTime").Value;
                int start = Convert.ToInt32(paraSet.SingleOrDefault(x => x.Name == "StartTime").Value);

                return new CanTraceAnalyser()
                {

                    SignalCountupCalculate = new SignalCountupCalculate(sigName, (int)(speed * (1 - tor / 100)) + "-" + (int)(speed * (1 + tor / 100)), start, start + 1000),

                };

            }
        }
    }

    /// <summary>
    /// Edgesum value check
    /// </summary>
    public sealed class EdgesumValueTargetSignalType : CanTxSignalType
    {


        private ObservableCollection<OpState> _testableOpStates = new ObservableCollection<OpState>();
        private static readonly EdgesumValueTargetSignalType _instance = new EdgesumValueTargetSignalType();

        public static EdgesumValueTargetSignalType GetInstance()
        {

            return _instance;
        }

        private EdgesumValueTargetSignalType()
        {

            Category = "SpeedSignals";
            Description = "This type is used to check the edgesums, use calculate count up to check the signal accuracy";
            Parameters.Add(new CANTxParameter("SourceCANTxSignal", "CAN signal name for current project", "", typeof(string)));
            Parameters.Add(new CANTxParameter("TolerancePercentage", "Tolerance in percentage", "10", typeof(int)));
            Parameters.Add(new CANTxParameter("Teethcount", "Teeth Count", "48", typeof(int)));
            Parameters.Add(new CANTxParameter("Radius", "Wheel Radius", "0.36", typeof(double)));
        }

        public override string Name
        {
            get { return "Edgesums"; }
        }

        public override CanTraceAnalyser ParameterizeCanTraceAnalyser(ObservableCollection<CANTxParameter> paraSet)
        {
            string sigName = paraSet.SingleOrDefault(x => x.Name == "SourceCANTxSignal").Value;
            double tor = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "TolerancePercentage").Value);
            double rad = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "Radius").Value);
            int teet = Convert.ToInt32(paraSet.SingleOrDefault(x => x.Name == "Teethcount").Value);


            //parameter check for opstate releated parameters

            if (paraSet.SingleOrDefault(x => x.Name == "StartTime") == null || paraSet.SingleOrDefault(x => x.Name == "TargetSpeed_mps") == null)
            {
                return new CanTraceAnalyser()
                {
                };

            }
            else
            {
                double speed = Convert.ToDouble(paraSet.SingleOrDefault(x => x.Name == "TargetSpeed_mps").Value);
                string startTime = paraSet.SingleOrDefault(x => x.Name == "StartTime").Value;
                int start = Convert.ToInt32(paraSet.SingleOrDefault(x => x.Name == "StartTime").Value);

                return new CanTraceAnalyser()
                {

                    SignalCountupCalculate = new SignalCountupCalculate(sigName, (int)(speed * teet * 2 / (6.28 * rad) * (1 - tor / 100)) + "-" + (int)(speed * teet * 2 / (6.28 * rad) * (1 + tor / 100)), start, start + 1000),

                };

            }
        }
    }
    #endregion SpeedSignals

    #region SensorSignals
    public sealed class SigAxValid : BoolTargetSignalType
    {
        private static readonly SigAxValid _instance = new SigAxValid();
        public static SigAxValid GetInstance()
        {

            return _instance;
        }


        private SigAxValid()
        {

            Category = "SensorSignals";
            Description = "This signal is a boolean value, which indicate the Ax signal is valid or not";

        }

        public override string Name
        {
            get { return "AxValid"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { OpAxMaximumBelow.GetInstance(), OpAxMinimumBelow.GetInstance(), OpAxTypicalValue.GetInstance(), }; }
        }
    }

    public sealed class SigAyValid : BoolTargetSignalType
    {
        private static readonly SigAyValid _instance = new SigAyValid();
        public static SigAyValid GetInstance()
        {

            return _instance;
        }


        private SigAyValid()
        {

            Category = "SensorSignals";
            Description = "This signal is a boolean value, which indicate the Ay signal is valid or not";

        }

        public override string Name
        {
            get { return "AyValid"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { OpAyMaximumAbove.GetInstance(), OpAyMinimumBelow.GetInstance(), OpAyTypicalValue.GetInstance(), }; }
        }
    }

    public sealed class SigYawRateValid : BoolTargetSignalType
    {
        private static readonly SigYawRateValid _instance = new SigYawRateValid();
        public static SigYawRateValid GetInstance()
        {

            return _instance;
        }


        private SigYawRateValid()
        {

            Category = "SensorSignals";
            Description = "This signal is a boolean value, which indicate the Yawrate signal is valid or not";

        }

        public override string Name
        {
            get { return "YawrateValid"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { OpYawrateMaximumAbove.GetInstance(), OpYawrateMinimumBelow.GetInstance(), OpYawrateTypicalValue.GetInstance(), }; }
        }
    }

    public sealed class SigSteeringAngleValid : BoolTargetSignalType
    {
        private static readonly SigSteeringAngleValid _instance = new SigSteeringAngleValid();
        public static SigSteeringAngleValid GetInstance()
        {

            return _instance;
        }


        private SigSteeringAngleValid()
        {

            Category = "SensorSignals";
            Description = "This signal is a boolean value, which indicate the SteeringAngle signal is valid or not";

        }

        public override string Name
        {
            get { return "SteeringAngleValid"; }
        }
        /// <summary>
        /// SteeringAngleInvalid opstate
        /// </summary>
        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { }; }
        }
    }

    public sealed class SigPressureValid : BoolTargetSignalType
    {
        private static readonly SigPressureValid _instance = new SigPressureValid();
        public static SigPressureValid GetInstance()
        {

            return _instance;
        }


        private SigPressureValid()
        {

            Category = "SensorSignals";
            Description = "This signal is a boolean value, which indicate the Pressure signal is valid or not";

        }

        public override string Name
        {
            get { return "PressureValid"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { OpPressureMaximumAbove.GetInstance(), OpPressureMinimumBelow.GetInstance(), OpPressureTypicalValue.GetInstance(), }; }
        }

    }

    public sealed class SigBLS : BoolTargetSignalType
    {
        private static readonly SigBLS _instance = new SigBLS();
        public static SigBLS GetInstance()
        {

            return _instance;
        }


        private SigBLS()
        {

            Category = "SensorSignals";
            Description = "This signal is a boolean value, which indicate the brake is pressed or not";

        }

        public override string Name
        {
            get { return "BLS"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { OpBrakePartial.GetInstance(), OpBrakeRelease.GetInstance(), OpFullBrake.GetInstance() }; }
        }
    }

    public sealed class SigOffsetAx : ValueTargetSignalType
    {
        private static readonly SigOffsetAx _instance = new SigOffsetAx();
        public static SigOffsetAx GetInstance()
        {

            return _instance;
        }


        private SigOffsetAx()
        {

            Category = "SensorSignals";
            Description = "This signal is the Ax offset signal depend on the unit";
            Parameters.First(x => x.Name == "SignalUnit").Value = "ms2";

        }

        public override string Name
        {
            get { return "AxOffset"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get
            {
                return new ObservableCollection<OpState>() {  OpAxMaximumBelow.GetInstance(), OpAxMinimumBelow.GetInstance(),
                                                                OpAxTypicalValue.GetInstance(),};
            }
        }
    }

    public sealed class SigOffsetAy : ValueTargetSignalType
    {
        private static readonly SigOffsetAy _instance = new SigOffsetAy();
        public static SigOffsetAy GetInstance()
        {

            return _instance;
        }


        private SigOffsetAy()
        {

            Category = "SensorSignals";
            Description = "This signal is the Ay offset signal depend on the unit";
            Parameters.First(x => x.Name == "SignalUnit").Value = "ms2";

        }

        public override string Name
        {
            get { return "AyOffset"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get
            {
                return new ObservableCollection<OpState>() {  OpAxMaximumBelow.GetInstance(), OpAxMinimumBelow.GetInstance(),
                                                                OpAxTypicalValue.GetInstance(),};
            }
        }
    }

    public sealed class SigOffsetYawrate : ValueTargetSignalType
    {
        private static readonly SigOffsetYawrate _instance = new SigOffsetYawrate();
        public static SigOffsetYawrate GetInstance()
        {

            return _instance;
        }


        private SigOffsetYawrate()
        {

            Category = "SensorSignals";
            Description = "This signal is the Yawrate offset signal depend on the unit";
            Parameters.First(x => x.Name == "SignalUnit").Value = "dps";

        }

        public override string Name
        {
            get { return "YawrateOffset"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get
            {
                return new ObservableCollection<OpState>() {  OpYawrateMaximumAbove.GetInstance(), OpYawrateMinimumBelow.GetInstance(),
                                                                OpYawrateTypicalValue.GetInstance(),};
            }
        }

    }

    public sealed class SigAx : ValueTargetSignalType
    {
        private static readonly SigAx _instance = new SigAx();
        public static SigAx GetInstance()
        {

            return _instance;
        }


        private SigAx()
        {

            Category = "SensorSignals";
            Description = "This signal is the Ax signal depend on the unit";
            Parameters.First(x => x.Name == "SignalUnit").Value = "ms2";

        }

        public override string Name
        {
            get { return "Ax"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get
            {
                return new ObservableCollection<OpState>() {  OpAxMaximumBelow.GetInstance(), OpAxMinimumBelow.GetInstance(),
                                                                OpAxTypicalValue.GetInstance(),};
            }
        }
    }

    public sealed class SigAy : ValueTargetSignalType
    {
        private static readonly SigAy _instance = new SigAy();
        public static SigAy GetInstance()
        {

            return _instance;
        }


        private SigAy()
        {

            Category = "SensorSignals";
            Description = "This signal is the Ay signal depend on the unit";
            Parameters.First(x => x.Name == "SignalUnit").Value = "ms2";

        }

        public override string Name
        {
            get { return "Ay"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get
            {
                return new ObservableCollection<OpState>() {  OpAyMaximumAbove.GetInstance(), OpAyMinimumBelow.GetInstance(),
                                                                OpAyTypicalValue.GetInstance(),};
            }
        }
    }

    public sealed class SigYawrate : ValueTargetSignalType
    {
        private static readonly SigYawrate _instance = new SigYawrate();
        public static SigYawrate GetInstance()
        {

            return _instance;
        }


        private SigYawrate()
        {

            Category = "SensorSignals";
            Description = "This signal is the Yawrate signal depend on the unit";
            Parameters.First(x => x.Name == "SignalUnit").Value = "dps";

        }

        public override string Name
        {
            get { return "Yawrate"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get
            {
                return new ObservableCollection<OpState>() {  OpYawrateMaximumAbove.GetInstance(), OpYawrateMinimumBelow.GetInstance(),
                                                                OpYawrateTypicalValue.GetInstance(),};
            }
        }

    }

    public sealed class SigPressure : ValueTargetSignalType
    {
        private static readonly SigPressure _instance = new SigPressure();
        public static SigPressure GetInstance()
        {

            return _instance;
        }


        private SigPressure()
        {

            Category = "SensorSignals";
            Description = "This signal is the MC Pressure signal depend on the unit";
            Parameters.First(x => x.Name == "SignalUnit").Value = "bar";

        }

        public override string Name
        {
            get { return "PressureMC"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get
            {
                return new ObservableCollection<OpState>() {  OpPressureMaximumAbove.GetInstance(), OpPressureMinimumBelow.GetInstance(),
                                                                OpPressureTypicalValue.GetInstance(),};
            }
        }

    }

    public sealed class SigBrakePostn : ValueTargetSignalType
    {
        private static readonly SigBrakePostn _instance = new SigBrakePostn();
        public static SigBrakePostn GetInstance()
        {

            return _instance;
        }


        private SigBrakePostn()
        {

            Category = "SensorSignals";
            Description = "This signal is the brake position depend on the unit";
            Parameters.First(x => x.Name == "SignalUnit").Value = "perone";

        }

        public override string Name
        {
            get { return "BrakePosition"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get
            {
                return new ObservableCollection<OpState>() {  OpFullBrake.GetInstance(), OpBrakePartial.GetInstance(),
                                                                OpBrakeRelease.GetInstance(),};
            }
        }

    }
    #endregion SensorSignals

    #region Others
    public sealed class SigNotUsed : BoolTargetSignalType
    {
        private static readonly SigNotUsed _instance = new SigNotUsed();
        public static SigNotUsed GetInstance()
        {

            return _instance;
        }


        private SigNotUsed()
        {

            Category = "Others";
            Description = "This signal is a constant value, which indicate the not used signal, typical value is 0";
            Parameters.First(x => x.Name == "SignalPattern").Value = "0";

        }

        public override string Name
        {
            get { return "NotUsedSignal"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { OpPostrun50kph.GetInstance(), OpNormalDrivingInit1S.GetInstance(), OpNormalDriving100kph.GetInstance(), }; }
        }
    }

    public sealed class AlivecounterTargetSignalType : CanTxSignalType
    {


        private ObservableCollection<OpState> _testableOpStates = new ObservableCollection<OpState>();
        private static readonly AlivecounterTargetSignalType _instance = new AlivecounterTargetSignalType();

        public static AlivecounterTargetSignalType GetInstance()
        {

            return _instance;
        }

        private AlivecounterTargetSignalType()
        {

            Category = "Others";
            Description = "This type is used to check the alivecounter";
            Parameters.Add(new CANTxParameter("SourceCANTxSignal", "CAN signal name for current project", "", typeof(string)));
            Parameters.Add(new CANTxParameter("Min", "min", "0", typeof(int)));
            Parameters.Add(new CANTxParameter("Max", "max", "15", typeof(int)));
            Parameters.Add(new CANTxParameter("Step", "step", "1", typeof(int)));
            Parameters.Add(new CANTxParameter("StartTime", "StartTime", "0", typeof(int)));
            Parameters.Add(new CANTxParameter("StopTime", "StopTime", "3000", typeof(int)));
        }

        public override string Name
        {
            get { return "Alivecounter"; }
        }

        public override CanTraceAnalyser ParameterizeCanTraceAnalyser(ObservableCollection<CANTxParameter> paraSet)
        {
            string sigName = paraSet.SingleOrDefault(x => x.Name == "SourceCANTxSignal").Value;
            int min = Convert.ToInt32(paraSet.SingleOrDefault(x => x.Name == "Min").Value);
            int max = Convert.ToInt32(paraSet.SingleOrDefault(x => x.Name == "Max").Value);
            int step = Convert.ToInt32(paraSet.SingleOrDefault(x => x.Name == "Step").Value);
            int start = Convert.ToInt32(paraSet.SingleOrDefault(x => x.Name == "StartTime").Value);
            int stop = Convert.ToInt32(paraSet.SingleOrDefault(x => x.Name == "StopTime").Value);


            //parameter check for opstate releated parameters

            return new CanTraceAnalyser()
            {

                MessageCounterCheck = new MessageCounterCheck(sigName, min, max, start, stop, step),
            };

        }
    }

    public sealed class CountUpTargetSignalType : CanTxSignalType
    {

        private ObservableCollection<OpState> _testableOpStates = new ObservableCollection<OpState>();
        private static readonly CountUpTargetSignalType _instance = new CountUpTargetSignalType();

        public static CountUpTargetSignalType GetInstance()
        {

            return _instance;
        }

        private CountUpTargetSignalType()
        {

            Category = "Others";
            Description = "This type is used to check the signal countup";
            Parameters.Add(new CANTxParameter("SourceCANTxSignal", "CAN signal name for current project", "", typeof(string)));
            Parameters.Add(new CANTxParameter("StartTime", "StartTime", "10000", typeof(int)));
            Parameters.Add(new CANTxParameter("StopTime", "StopTime", "15000", typeof(int)));
            Parameters.Add(new CANTxParameter("Expected", "Expected", "0-1023", typeof(string)));
        }

        public override string Name
        {
            get { return "SignalCountUp"; }
        }

        public override CanTraceAnalyser ParameterizeCanTraceAnalyser(ObservableCollection<CANTxParameter> paraSet)
        {
            string sigName = paraSet.SingleOrDefault(x => x.Name == "SourceCANTxSignal").Value;
            int start = Convert.ToInt32(paraSet.SingleOrDefault(x => x.Name == "StartTime").Value);
            int stop = Convert.ToInt32(paraSet.SingleOrDefault(x => x.Name == "StopTime").Value);
            string expectedValue = paraSet.SingleOrDefault(x => x.Name == "Expected").Value;


            //parameter check for opstate releated parameters

            return new CanTraceAnalyser()
            {

                //MessageCounterCheck = new MessageCounterCheck(sigName, min, max, start, stop, step),
                SignalCountupCalculate = new SignalCountupCalculate(sigName, expectedValue, start, stop),


            };

        }
    }

    public sealed class SigChecksum : CanTxSignalType
    {

        private ObservableCollection<OpState> _testableOpStates = new ObservableCollection<OpState>();
        private static readonly SigChecksum _instance = new SigChecksum();

        private SigChecksum()
        {
            Category = "Others";
            Description = "This type is used to check the checksum";
            Parameters.Add(new CANTxParameter("SourceCANTxSignal", "CAN signal name for current project", "", typeof(string)));
            Parameters.Add(new CANTxParameter("algorithm", @"checksum algorithm, please give function name specified in NetTxCrcAlgrithm.pm in Misc folder", null, typeof(string)));
            Parameters.Add(new CANTxParameter("StartTime", "StartTime", "0", typeof(int)));
            Parameters.Add(new CANTxParameter("StopTime", "StopTime", "3000", typeof(int)));

        }

        public override string Name
        {
            get { return "Checksum"; }
        }
        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return _testableOpStates; }
        }

        public static SigChecksum GetInstance()
        {
            return _instance;

        }

        public override CanTraceAnalyser ParameterizeCanTraceAnalyser(ObservableCollection<CANTxParameter> paraSet)
        {
            string sigName = paraSet.SingleOrDefault(x => x.Name == "SourceCANTxSignal").Value;
            string algo = paraSet.SingleOrDefault(x => x.Name == "algorithm").Value;
            int start = Convert.ToInt32(paraSet.SingleOrDefault(x => x.Name == "StartTime").Value);
            int stop = Convert.ToInt32(paraSet.SingleOrDefault(x => x.Name == "StopTime").Value);


            //parameter check for opstate releated parameters

            return new CanTraceAnalyser()
            {

                MessageChecksumCheck = new MessageChecksumCheck(sigName, algo, start, stop),
            };


        }

    }

    public sealed class EPBTimeOut : BoolTargetSignalType
    {
        private static readonly EPBTimeOut _instance = new EPBTimeOut();
        public static EPBTimeOut GetInstance()
        {

            return _instance;
        }


        private EPBTimeOut()
        {

            Category = "Others";
            Description = "This signal is a boolean value, which indicate the EPB signal is TimeOut";

        }

        public override string Name
        {
            get { return "EPBTimeOut"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { OpEPBTO.GetInstance() }; }
        }
    }

    public sealed class IFA : BoolTargetSignalType
    {
        private static readonly IFA _instance = new IFA();
        public static IFA GetInstance()
        {

            return _instance;
        }


        private IFA()
        {

            Category = "Others";
            Description = "This signal is a boolean value, which indicate the EPB signal is TimeOut";

        }

        public override string Name
        {
            get { return "IFAFailure"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { OpIFAFailure.GetInstance() }; }
        }
    }

    public sealed class SigApbButtonLineOpen : BoolTargetSignalType
    {
        private static readonly SigApbButtonLineOpen _instance = new SigApbButtonLineOpen();
        public static SigApbButtonLineOpen GetInstance()
        {

            return _instance;
        }


        private SigApbButtonLineOpen()
        {

            Category = "Others";
            Description = "This signal is a boolean value, which indicate the APB button line whether is open or not";

        }

        public override string Name
        {
            get { return "ApbButtonLineOpen"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { OpApbLineOpen.GetInstance() }; }
        }
    }
    #endregion Others

    #region Message
    public sealed class MsgCycleTimeCheck : MessageTargetType
    {
        private static readonly MsgCycleTimeCheck _instance = new MsgCycleTimeCheck();
        public static MsgCycleTimeCheck GetInstance()
        {

            return _instance;
        }


        private MsgCycleTimeCheck()
        {

            Category = "Message";
            Description = "This type is used to check the message cycle time";
            Parameters.First(x => x.Name == "TolerancePercentage").Value = "10";

        }

        public override string Name
        {
            get { return "MessageCycleTimeCheck"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get
            {
                return new ObservableCollection<OpState>() { OpNormalDrivingInit0S.GetInstance(), OpNormalDriving50kph.GetInstance(), };
            }
        }

    }
    #endregion Message

    #region VAFS

    /// <summary>
    /// HBA signal active
    /// </summary>
    public sealed class SigHBActive : BoolTargetSignalType
    {

        private static readonly SigHBActive _instance = new SigHBActive();
        public static SigHBActive GetInstance()
        {

            return _instance;
        }


        private SigHBActive()
        {
            Category = "VAFS";
            Description = "This signal is a boolean value, which activates HBA";

        }

        public override string Name
        {
            get { return "HBActive"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { HBActive.GetInstance(), }; }
        }

    }

    /// <summary>
    /// HBB signal active
    /// </summary>
    public sealed class SigHBBActive : BoolTargetSignalType
    {

        private static readonly SigHBBActive _instance = new SigHBBActive();
        public static SigHBBActive GetInstance()
        {

            return _instance;
        }


        private SigHBBActive()
        {
            Category = "VAFS";
            Description = "This signal is a boolean value, which activates HBB";

        }

        public override string Name
        {
            get { return "HBBActive"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { HBBActive.GetInstance(), }; }
        }

    }

    /// <summary>
    /// HBC signal active
    /// </summary>
    public sealed class SigHBCActive : BoolTargetSignalType
    {

        private static readonly SigHBCActive _instance = new SigHBCActive();
        public static SigHBCActive GetInstance()
        {

            return _instance;
        }


        private SigHBCActive()
        {
            Category = "VAFS";
            Description = "This signal is a boolean value, which activates HBC";

        }

        public override string Name
        {
            get { return "HBCActive"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { HBCActive.GetInstance(), }; }
        }

    }

    /// <summary>
    /// HHC signal active
    /// </summary>
    public sealed class SigHHCActive : BoolTargetSignalType
    {

        private static readonly SigHHCActive _instance = new SigHHCActive();
        public static SigHHCActive GetInstance()
        {

            return _instance;
        }


        private SigHHCActive()
        {
            Category = "VAFS";
            Description = "This signal is a boolean value, which activates HBC";

        }

        public override string Name
        {
            get { return "HHCActive"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { HHCActive.GetInstance(), }; }
        }

    }

    /// <summary>
    /// HAZ signal active 
    /// </summary>
    public sealed class SigHAZActive : BoolTargetSignalType
    {

        private static readonly SigHAZActive _instance = new SigHAZActive();
        public static SigHAZActive GetInstance()
        {

            return _instance;
        }


        private SigHAZActive()
        {
            Category = "VAFS";
            Description = "This signal is a boolean value, which activates HAZ";

        }

        public override string Name
        {
            get { return "HAZActive"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { HAZActive.GetInstance(), }; }
        }

    }

    /// <summary>
    /// HDC signal active
    /// </summary>
    public sealed class SigHDCActive : BoolTargetSignalType
    {

        private static readonly SigHDCActive _instance = new SigHDCActive();
        public static SigHDCActive GetInstance()
        {

            return _instance;
        }


        private SigHDCActive()
        {
            Category = "VAFS";
            Description = "This signal is a boolean value, which activates HDC";

        }

        public override string Name
        {
            get { return "HDCActive"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { HDCActive.GetInstance(), }; }
        }

    }

    /// <summary>
    /// CDP sginal active
    /// </summary>
    public sealed class SigCDPActive : BoolTargetSignalType
    {

        private static readonly SigCDPActive _instance = new SigCDPActive();
        public static SigCDPActive GetInstance()
        {

            return _instance;
        }


        private SigCDPActive()
        {
            Category = "VAFS";
            Description = "This signal is a boolean value, which activates CDP";

        }

        public override string Name
        {
            get { return "CDPActive"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { CDPActive.GetInstance(), }; }
        }

    }

    /// <summary>
    /// AWB signal active
    /// </summary>
    public sealed class SigAWBActive : BoolTargetSignalType
    {

        private static readonly SigAWBActive _instance = new SigAWBActive();
        public static SigAWBActive GetInstance()
        {

            return _instance;
        }


        private SigAWBActive()
        {
            Category = "VAFS";
            Description = "This signal is a boolean value, which activates AWB";

        }

        public override string Name
        {
            get { return "AWBActive"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { AWBActive.GetInstance(), }; }
        }

    }

    /// <summary>
    /// VLC signal active
    /// </summary>
    public sealed class SigVLCActive : BoolTargetSignalType
    {

        private static readonly SigVLCActive _instance = new SigVLCActive();
        public static SigVLCActive GetInstance()
        {

            return _instance;
        }


        private SigVLCActive()
        {
            Category = "VAFS";
            Description = "This signal is a boolean value, which activates VLC";

        }

        public override string Name
        {
            get { return "VLCActive"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { VLCActive.GetInstance(), }; }
        }

    }

    /// <summary>
    /// CDD signal active 
    /// </summary>
    public sealed class SigCDDActive : BoolTargetSignalType
    {

        private static readonly SigCDDActive _instance = new SigCDDActive();
        public static SigCDDActive GetInstance()
        {

            return _instance;
        }


        private SigCDDActive()
        {
            Category = "VAFS";
            Description = "This signal is a boolean value, which activates CDD";

        }

        public override string Name
        {
            get { return "CDDActive"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { CDDActive.GetInstance(), }; }
        }

    }


    /// <summary>
    /// AEB signal active
    /// </summary>
    public sealed class SigAEBActive : BoolTargetSignalType
    {

        private static readonly SigAEBActive _instance = new SigAEBActive();
        public static SigAEBActive GetInstance()
        {

            return _instance;
        }


        private SigAEBActive()
        {
            Category = "VAFS";
            Description = "This signal is a boolean value, which activates AEB";

        }

        public override string Name
        {
            get { return "AEBActive"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { AEBActive.GetInstance(), }; }
        }

    }

    /// <summary>
    /// AVH signal active
    /// </summary>
    public sealed class SigAVHActive : BoolTargetSignalType
    {

        private static readonly SigAVHActive _instance = new SigAVHActive();
        public static SigAVHActive GetInstance()
        {

            return _instance;
        }


        private SigAVHActive()
        {
            Category = "VAFS";
            Description = "This signal is a boolean value, which activates AVH";

        }

        public override string Name
        {
            get { return "AVHActive"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { AVHActive.GetInstance(), }; }
        }

    }

    /// <summary>
    /// ABP signal active
    /// </summary>
    public sealed class SigABPActive : BoolTargetSignalType
    {

        private static readonly SigABPActive _instance = new SigABPActive();
        public static SigABPActive GetInstance()
        {

            return _instance;
        }


        private SigABPActive()
        {
            Category = "VAFS";
            Description = "This signal is a boolean value, which activates ABP";

        }

        public override string Name
        {
            get { return "ABPActive"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { ABPActive.GetInstance(), }; }
        }

    }

    /// <summary>
    /// ABP signal active
    /// </summary>
    public sealed class SigABActive : BoolTargetSignalType
    {

        private static readonly SigABActive _instance = new SigABActive();
        public static SigABActive GetInstance()
        {

            return _instance;
        }


        private SigABActive()
        {
            Category = "VAFS";
            Description = "This signal is a boolean value, which activates ABP";

        }

        public override string Name
        {
            get { return "ABActive"; }
        }

        public override ObservableCollection<OpState> ProposedOpStates
        {
            get { return new ObservableCollection<OpState>() { ABActive.GetInstance(), }; }
        }

    }
    #endregion VAFS

}