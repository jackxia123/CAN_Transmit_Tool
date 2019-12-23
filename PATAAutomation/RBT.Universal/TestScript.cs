using RBT.Universal.CanEvalParameters;
using RBT.Universal.Keywords;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace RBT.Universal
{
    [DataContract]
    public class TestScript : Model
    {
        private ScalarDependentParameter _purpose = new ScalarDependentParameter("purpose", "objects to be verified in this test case", "", "");
        private ListDependentParameter _initDiagSeq = new ListDependentParameter("init_diag_sequences ", @"Run a seq-file in Bosch Diag Mode in Initialization phase. 

e.g.init_diag_sequences = @(‘Auto_init.seq’)    
", new ObservableCollection<string>(), "");

        private ListDependentParameter _finalDiagSeq = new ListDependentParameter("final_diag_sequences ", @"Run a seq-file in Bosch Diag Mode in finalization phase. 

e.g. final_diag_sequences = @(‘EEPROM_Clear.seq’)    
", new ObservableCollection<string>(), "");

        private HashDependentParameter _initModelValues = new HashDependentParameter("initialize_model_values ", @"Set LabCar model variables to given values. The values will be reset in Finalization phase.
 
e.g. initialize_model_values = %(‘p_MBC’ => ‘0’, ‘p_C1’ => ‘0’, ‘BLS’ => ‘0’, ‘BrakePedal’=> ‘0’)

", new Dictionary<string, string>(), "");

        private ListDependentParameter _rbMandatoryFaults = new ListDependentParameter("RB_mandatory_faults ", @"Give expected fail code. If ‘read_FPS’ is not used, fail code will be read at the very end of Stimulation & Measurement phase.
 
e.g. RB_mandatory_faults = @(‘1234XXXX’) 
   or RB_mandatory_faults = @(‘fail word1’)  

NOTE:
Fail word has to be defined in Fault mapping file.

", new ObservableCollection<string>(), "");

        private ListDependentParameter _rbOptionalFaults = new ListDependentParameter("RB_optional_faults ", @"Give expected fail code. If ‘read_FPS’ is not used, fail code will be read at the very end of Stimulation & Measurement phase.
 
e.g. RB_mandatory_faults = @(‘1234XXXX’) 
   or RB_mandatory_faults = @(‘fail word1’)  

NOTE:
Fail word has to be defined in Fault mapping file.

", new ObservableCollection<string>(), "");

        private ListDependentParameter _rbDisjunctionFaults = new ListDependentParameter("RB_disjuction_faults ", @"Give expected fail code. If ‘read_FPS’ is not used, fail code will be read at the very end of Stimulation & Measurement phase.
 
e.g. RB_mandatory_faults = @(‘1234XXXX’) 
   or RB_mandatory_faults = @(‘fail word1’)  

NOTE:
Fail word has to be defined in Fault mapping file.

", new ObservableCollection<string>(), "");

        private ListDependentParameter _cuMandatoryFaults = new ListDependentParameter("CU_mandatory_faults ", @"Give expected fail code. If ‘read_FPC’ is not used, fail code will be read at the very end of Stimulation & Measurement phase.
Commands for read/delete DTC must be defined in Project Default file

e.g. CU_mandatory_faults = @(‘1234XXXX’) 
   or CU_mandatory_faults = @(‘fail word1’)  

", new ObservableCollection<string>(), "");

        private ListDependentParameter _cuOptionalFaults = new ListDependentParameter("CU_optional_faults ", @"Give expected fail code. If ‘read_FPC’ is not used, fail code will be read at the very end of Stimulation & Measurement phase.
Commands for read/delete DTC must be defined in Project Default file

e.g. CU_mandatory_faults = @(‘1234XXXX’) 
   or CU_mandatory_faults = @(‘fail word1’)  

", new ObservableCollection<string>(), "");

        private ListDependentParameter _cuDisjunctionFaults = new ListDependentParameter("CU_disjunction_faults ", @"Give expected fail code. If ‘read_FPC’ is not used, fail code will be read at the very end of Stimulation & Measurement phase.
Commands for read/delete DTC must be defined in Project Default file

e.g. CU_mandatory_faults = @(‘1234XXXX’) 
   or CU_mandatory_faults = @(‘fail word1’)  

", new ObservableCollection<string>(), "");


        private ListDependentParameter _notDelFPC = new ListDependentParameter("not_delete_FPC ", @"Not delete FPC in finalization phase.

e.g. not_delete_FPC = @(‘final’)

", new ObservableCollection<string>(), "");

        private TestSequence _testSequence;
        private CanTraceAnalyser _canTraceAnalyser;
        private Variant _variant;
        private string _testCaseScript;
        private string _testDescription;
        private string _expectedResult;
        private string _doorsLink;
        private ObservableCollection<string> _testSteps = new ObservableCollection<string>();
        private ObservableCollection<string> _expSteps = new ObservableCollection<string>();

        private string _qcFolderPath = "";

        /// <summary>
        /// ctor to create a new test case script
        /// </summary>
        public TestScript()
        {
            Trigger.ResetID();
            AndTrigger.ResetID();
            DeltaTime.ResetID();


        }


        //Xia Jack add  CAT filter condition 2018/07/23
        [DataMember]
        public string CAT { get; set; }

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public ScalarDependentParameter Purpose
        {
            get
            {
                return _purpose;
            }
            set
            {
                SetProperty(ref _purpose, value);
            }
        }
        [DataMember]
        public ListDependentParameter InitDiagSequence
        {
            get
            {
                return _initDiagSeq;
            }
            set
            {
                SetProperty(ref _initDiagSeq, value);
            }
        }
        [DataMember]
        public ListDependentParameter FinalDiagSequence
        {
            get
            {
                return _finalDiagSeq;
            }
            set
            {
                SetProperty(ref _finalDiagSeq, value);
            }
        }
        [DataMember]
        public HashDependentParameter InitModelValues
        {
            get
            {
                return _initModelValues;
            }
            set
            {
                SetProperty(ref _initModelValues, value);
            }
        }
        [DataMember]
        public ListDependentParameter RBMandatoryFaults
        {
            get
            {
                return _rbMandatoryFaults;
            }
            set
            {
                SetProperty(ref _rbMandatoryFaults, value);
            }
        }
        [DataMember]
        public ListDependentParameter RBOptionalFaults
        {
            get
            {
                return _rbOptionalFaults;
            }
            set
            {
                SetProperty(ref _rbOptionalFaults, value);
            }
        }
        [DataMember]
        public ListDependentParameter RBDisjunctionFaults
        {
            get
            {
                return _rbDisjunctionFaults;
            }
            set
            {
                SetProperty(ref _rbDisjunctionFaults, value);
            }
        }
        [DataMember]
        public ListDependentParameter CUMandatoryFaults
        {
            get
            {
                return _cuMandatoryFaults;
            }
            set
            {
                SetProperty(ref _cuMandatoryFaults, value);
            }
        }
        [DataMember]
        public ListDependentParameter CUOptionalFaults
        {
            get
            {
                return _cuOptionalFaults;
            }
            set
            {
                SetProperty(ref _cuOptionalFaults, value);
            }
        }
        [DataMember]
        public ListDependentParameter CUDisjunctionFaults
        {
            get
            {
                return _cuDisjunctionFaults;
            }
            set
            {
                SetProperty(ref _cuDisjunctionFaults, value);
            }
        }
        [DataMember]
        public ListDependentParameter NotDeleteFPC
        {
            get
            {
                return _notDelFPC;
            }
            set
            {
                SetProperty(ref _notDelFPC, value);
            }
        }
        [DataMember]
        public TestSequence TestSequence
        {
            get
            {
                return _testSequence;
            }
            set
            {
                SetProperty(ref _testSequence, value);
            }
        }
        [DataMember]
        public CanTraceAnalyser CanTraceAnalyser
        {
            get
            {
                return _canTraceAnalyser;
            }
            set
            {

                SetProperty(ref _canTraceAnalyser, value);
            }
        }
        [DataMember]
        public Variant Variant
        {
            get
            {
                return _variant;
            }
            set
            {
                SetProperty(ref _variant, value);
            }
        }

        /// <summary>
        /// Tidy the par file by add space during writing
        /// </summary>
        /// <param name="strAhead"></param>
        /// <param name="legnth"></param>
        /// <returns></returns>
        public static string SpacesTidy(string strAhead, int legnth = 45)
        {
            string str = "";
            for (int i = 0; i < (legnth - strAhead.Length); i++)
            {
                str = str + " ";
            }
            return str;

        }
        /// <summary>
        /// To convert script directly to Test case
        /// </summary>
        [DataMember]
        public string TestCaseScript
        {
            get
            {
                string tempTC = "[" + Name + "]" + "\n";
                tempTC = tempTC + Purpose.Name + SpacesTidy(Purpose.Name) + " = " + Purpose.Value + "\n";
                if (TestSequence != null)
                {
                    //comment the exception handling to catch in upper level and give user hint for correction
                    //try
                    //{
                    TestSequence.PlausCheck();
                    //}
                    //catch (Exception e)
                    //{
                    //    tempTC = "Error: TestSequence"+e.Message + "\n";
                    //    return tempTC;

                    // }
                    tempTC = tempTC + TestSequence.TestSequenceScript + "\n";

                }

                if (InitDiagSequence.ValueList.Count > 0)
                {
                    tempTC = tempTC + InitDiagSequence.Name + SpacesTidy(InitDiagSequence.Name) + " = " + InitDiagSequence.Value + "\n";
                }

                if (FinalDiagSequence.ValueList.Count > 0)
                {
                    tempTC = tempTC + FinalDiagSequence.Name + SpacesTidy(FinalDiagSequence.Name) + " = " + FinalDiagSequence.Value + "\n";
                }

                if (InitModelValues.KeyValuePairList.Count > 0)
                {
                    tempTC = tempTC + InitModelValues.Name + SpacesTidy(InitModelValues.Name) + " = " + InitModelValues.Value + "\n";
                }
                if (RBMandatoryFaults.ValueList.Count > 0)
                {
                    tempTC = tempTC + RBMandatoryFaults.Name + SpacesTidy(RBMandatoryFaults.Name) + " = " + RBMandatoryFaults.Value + "\n";
                }
                if (RBOptionalFaults.ValueList.Count > 0)
                {
                    tempTC = tempTC + RBOptionalFaults.Name + SpacesTidy(RBOptionalFaults.Name) + " = " + RBOptionalFaults.Value + "\n";
                }
                if (RBDisjunctionFaults.ValueList.Count > 0)
                {
                    tempTC = tempTC + RBDisjunctionFaults.Name + SpacesTidy(RBDisjunctionFaults.Name) + " = " + RBDisjunctionFaults.Value + "\n";
                }
                if (CUMandatoryFaults.ValueList.Count > 0)
                {
                    tempTC = tempTC + CUMandatoryFaults.Name + SpacesTidy(CUMandatoryFaults.Name) + " = " + CUMandatoryFaults.Value + "\n";
                }
                if (CUOptionalFaults.ValueList.Count > 0)
                {
                    tempTC = tempTC + CUOptionalFaults.Name + SpacesTidy(CUOptionalFaults.Name) + " = " + CUOptionalFaults.Value + "\n";
                }
                if (CUDisjunctionFaults.ValueList.Count > 0)
                {
                    tempTC = tempTC + CUDisjunctionFaults.Name + SpacesTidy(CUDisjunctionFaults.Name) + " = " + CUDisjunctionFaults.Value + "\n";
                }

                if (CanTraceAnalyser != null)
                {
                    //comment the exception handling to catch in upper level and give user hint for correction
                    //try
                    //{
                    CanTraceAnalyser.ParametersValidated();
                    // }
                    // catch (Exception e)
                    // {
                    //     tempTC = "Error: CanTraceAnalyser: " + e.Message + "\n";
                    //     return tempTC;

                    // }
                    tempTC = tempTC + CanTraceAnalyser.EvalScript + "\n";

                }

                return tempTC;
            }


            set
            {
                SetProperty(ref _testCaseScript, value);
            }
        }
        [DataMember]
        public string TestDescription
        {
            get
            {
                ObservableCollection<string> temp = new ObservableCollection<string>();
                int i = 1;
                foreach (var st in TestSteps)
                {
                    temp.Add(i + ". " + st);
                    i++;
                }

                string testDesc = String.Join("\n", temp);
                return "Purpose:\n" + Purpose.ScalarValue + "\n" + testDesc;
            }
            set { SetProperty(ref _testDescription, value); }
        }

        [DataMember]
        public string ExpectedResult
        {
            get
            {
                return String.Join("\n", ExpectedSteps.ToList());
            }
            set { SetProperty(ref _expectedResult, value); }
        }

        [DataMember]
        public ObservableCollection<string> TestSteps
        {
            get
            {

                string _tmpStep = "";
                _testSteps.Clear();
                _expSteps.Clear();
                foreach (var step in TestSequence.Keywords)
                {

                    if (step is IDependentKeyword && ((IDependentKeyword)step).ParametrizationType != DependentKeywordParameterizationType.InlineParameterized) //show in spec the parameters of dependent keyword
                    {
                        _tmpStep = step.ScriptName;
                        //Process keywords with evaluation function
                        if (step is ReadCanLamps || step is ReadCanSignals || step is ReadEcuSignals || step is ReadLamps || step is CheckCanSignal || step is ReadFPC || step is ReadFPS || step is ReadVr || step is ReadVra)
                        {
                            string tempExp = "";
                            foreach (var dp in ((IDependentKeyword)step).DependentParameters)
                            {
                                tempExp = tempExp + dp.Name + " = " + dp.Value + "\n";
                            }
                            _expSteps.Add(TestSequence.Keywords.IndexOf(step) + 1 + "." + tempExp);

                        }
                        else
                        {
                            _tmpStep = _tmpStep + "   parameters: " + "\n";
                            foreach (var dp in ((IDependentKeyword)step).DependentParameters)
                            {

                                if (dp is ScalarDependentParameter)
                                {

                                    _tmpStep = _tmpStep + "        " + dp.Name + " = " + dp.ScalarValue + "\n";

                                }
                                else if (dp is ListDependentParameter)
                                {
                                    if (((ListDependentParameter)dp).ValueList != null) _tmpStep = _tmpStep + "        " + dp.Name + " = " + string.Join(" ", ((ListDependentParameter)dp).ValueList) + "\n";  //workaround for do_line_manipulation
                                }
                                else if (dp is HashDependentParameter)
                                {
                                    _tmpStep = _tmpStep + "        " + dp.Name + " : " + "\n";
                                    foreach (var pair in ((HashDependentParameter)dp).KeyValuePairList)
                                    {
                                        _tmpStep = _tmpStep + "        " + pair.Key + " = " + pair.Value + "\n";

                                    }

                                }

                            }
                        }
                    }
                    else
                    {
                        _tmpStep = step.ScriptName;

                    }
                    _testSteps.Add(_tmpStep);
                }//end of foreach


                //add common evaluation for RB and CU faults
                if (RBMandatoryFaults.ValueList.Count > 0 || RBOptionalFaults.ValueList.Count > 0 || RBDisjunctionFaults.ValueList.Count > 0)
                {
                    _testSteps.Add("Check RB faults");
                }
                if (CUMandatoryFaults.ValueList.Count > 0 || CUOptionalFaults.ValueList.Count > 0 || CUDisjunctionFaults.ValueList.Count > 0)
                {
                    _testSteps.Add("Check CU faults");
                }

                //add to evaluation
                if (RBMandatoryFaults.ValueList.Count > 0)
                {
                    _expSteps.Add("RBMandatoryFaults: " + String.Join(" ", RBMandatoryFaults.ValueList));
                }
                if (RBOptionalFaults.ValueList.Count > 0)
                {
                    _expSteps.Add("RBOptionalFaults: " + String.Join(" ", RBOptionalFaults.ValueList));
                }
                if (RBDisjunctionFaults.ValueList.Count > 0)
                {
                    _expSteps.Add("RBDisjunctionFaults: " + String.Join(" ", RBDisjunctionFaults.ValueList));
                }

                if (CUMandatoryFaults.ValueList.Count > 0)
                {
                    _expSteps.Add("CUMandatoryFaults: " + String.Join(" ", CUMandatoryFaults.ValueList));
                }
                if (CUOptionalFaults.ValueList.Count > 0)
                {
                    _expSteps.Add("CUOptionalFaults: " + String.Join(" ", CUOptionalFaults.ValueList));
                }

                if (CUDisjunctionFaults.ValueList.Count > 0)
                {
                    _expSteps.Add("CUDisjunctionFaults: " + String.Join(" ", CUDisjunctionFaults.ValueList));
                }

                return _testSteps;
            }
            set
            {
                SetProperty(ref _testSteps, value);
            }
        }

        [DataMember]
        public ObservableCollection<string> ExpectedSteps
        {
            get
            {


                if (CanTraceAnalyser != null)
                {
                    _expSteps.Add("\nAnalyse CAN Trace as following:");
                    foreach (var canEval in CanTraceAnalyser.AllParameters)
                    {
                        if (canEval != null)
                        {
                            string tmpExp = "";

                            if (canEval.EvalParameter is ScalarDependentParameter)
                            {

                                tmpExp = tmpExp + "        " + canEval.EvalParameter.Name + " = " + canEval.EvalParameter.ScalarValue + "\n";

                            }
                            else if (canEval.EvalParameter is ListDependentParameter)
                            {
                                if (((ListDependentParameter)canEval.EvalParameter).ValueList != null) tmpExp = tmpExp + "        " + canEval.EvalParameter.Name + " = " + string.Join(" ", ((ListDependentParameter)canEval.EvalParameter).ValueList) + "\n";
                            }
                            else if (canEval.EvalParameter is HashDependentParameter)
                            {
                                tmpExp = tmpExp + "        " + canEval.EvalParameter.Name + " : " + "\n";
                                foreach (var pair in ((HashDependentParameter)canEval.EvalParameter).KeyValuePairList)
                                {
                                    tmpExp = tmpExp + "        " + pair.Key + " = " + pair.Value + "\n";

                                }

                            }


                            _expSteps.Add(tmpExp);

                        }

                    }
                }

                return _expSteps;
            }


            set
            {
                SetProperty(ref _expSteps, value);
            }
        }

        [DataMember]
        public string DoorsLink
        {
            get
            {

                return _doorsLink;
            }


            set
            {
                SetProperty(ref _doorsLink, value);
            }
        }

        [DataMember]
        public string QcFolderPath
        {
            get
            {

                return _qcFolderPath;
            }


            set
            {
                SetProperty(ref _qcFolderPath, value);
            }
        }

        [DataMember]
        public string TestCaseName
        {
            get
            {

                return Name.Substring(17);
            }


            set
            {

            }
        }
    }
}
