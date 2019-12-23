using RBT.Universal.Keywords;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace RBT.Universal
{
    [DataContract]
    public class TestSequence:Model,ICloneable
    {
        private ObservableCollection<Keyword> _keywordList = new ObservableCollection<Keyword>();
        /// <summary>
        /// parameterless ctor to enable serialization
        /// </summary>
        public TestSequence() { }
        /// <summary>
        /// ctor with parameters
        /// </summary>
        /// <param name="keywordList"></param>
        public TestSequence(ObservableCollection<Keyword> keywordList)
        {

            _keywordList = keywordList;
           

        }

        /// <summary>
        /// Collection to hold all the keywords
        /// </summary>
        /// 
        [DataMember]
        public ObservableCollection<Keyword> Keywords
        {
            get
            {
                return _keywordList;

            }
            set
            {

                SetProperty(ref _keywordList,value);

            }

        }
        /// <summary>
        /// indexer to use the keywordlist
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Keyword this[int i]
        {
            get
            {
                return _keywordList[i];


            }
            set
            {

                _keywordList[i] = value;

            }

        }
        /// <summary>
        /// To check the plausibility of test sequence
        /// 1. Parameter validation
        /// 2. Usage limit check
        /// 3. Pairing test
        /// </summary>
        /// <returns></returns>
        public void PlausCheck()
        {
            //int all keyword paired status to false
            foreach (Keyword keywd in _keywordList)
            {
                if (keywd is IPairedKeyword)
                {
                    ((IPairedKeyword)keywd).Paired = false;

                }
            }

            //start check here
            foreach (Keyword keywd in _keywordList)
            {
                //Validate parameters
                if (keywd is IDependentKeyword)
                {
                    if (!((IDependentKeyword)keywd).ParametersValidated())
                    {

                        throw new FormatException(string.Format("Keyword {0} is not correctly parameterized, Hint:\n{1}", keywd.Name, ((IDependentKeyword)keywd).DependentParameters[0].Description));

                    }

                }                

                if (keywd is IPairedKeyword)
                {
                    //Check pairing   
                    
                    //get all paired index             
                    List<int> PairedBeforeIndex = new List<int>();
                    List<int> PairedAfterIndex = new List<int>();

                    for (int i = 0; i < _keywordList.IndexOf(keywd); i++)
                    {
                        if (keywd.Name == "reset_CAN_manipulation")   // treatment for reset_CAN_Manipulation
                        {
                            if (((IPairedKeyword)keywd).PairBefore.Contains(_keywordList[i].Name))
                            {
                                PairedBeforeIndex.Add(i);

                            }

                        }
                        else
                        {
                            if (((IPairedKeyword)keywd).PairBefore.Contains(_keywordList[i].ScriptName))
                            {
                                PairedBeforeIndex.Add(i);

                            }

                        }

                    }

                    for (int i = _keywordList.IndexOf(keywd); i < _keywordList.Count; i++)
                    {
                        if (((IPairedKeyword)keywd).PairAfter.Contains(_keywordList[i].ScriptName))
                        {
                            PairedAfterIndex.Add(i);
                        }
                    }


                    //mark in the paired flag of keyword

                    if (((IPairedKeyword)keywd).PairBefore.Count > 0 && PairedBeforeIndex.Count == 0)
                    {
                        throw new FormatException(string.Format("Index {2} Keyword {0} is not paired correctly, no required paired keywords provided before\nRequired pair keywords: {1}", keywd.Name, String.Join(" ", ((IPairedKeyword)keywd).PairBefore.ToArray()), _keywordList.IndexOf(keywd)));

                    }
                    else if (((IPairedKeyword)keywd).PairBefore.Count > 0)
                    {

                        if (((IPairedKeyword)keywd).PairType == KeywordPairType.One2One)
                        {
                            if (((IPairedKeyword)_keywordList[PairedBeforeIndex[PairedBeforeIndex.Count - 1]]).Paired == false)
                            {
                                ((IPairedKeyword)_keywordList[PairedBeforeIndex[PairedBeforeIndex.Count - 1]]).Paired = true;
                            }
                            else
                            {
                                throw new FormatException(string.Format("Index {2} Keyword {0} is not paired correctly, required paired keywords already paired before\nRequired pair keywords: {1}", keywd.Name, String.Join(" ", ((IPairedKeyword)keywd).PairBefore.ToArray()), _keywordList.IndexOf(keywd)));

                            }

                        }
                        else if (((IPairedKeyword)keywd).PairType == KeywordPairType.One2n)
                        {
                            if (PairedBeforeIndex.TrueForAll(x => ((IPairedKeyword)_keywordList[x]).Paired == true))
                            {
                                throw new FormatException(string.Format("Index {2} Keyword {0} is not paired correctly, all required keywords already paired before\nRequired pair keywords: {1}", keywd.Name, String.Join(" ", ((IPairedKeyword)keywd).PairBefore.ToArray()), _keywordList.IndexOf(keywd)));

                            }
                            else
                            {
                                foreach( int i in PairedBeforeIndex.FindAll(x => ((IPairedKeyword)_keywordList[x]).Paired == false))
                                {
                                    ((IPairedKeyword)_keywordList[i]).Paired = true;
                                }
                                

                            }

                        }

                    }



                    if (((IPairedKeyword)keywd).PairAfter.Count > 0 && PairedAfterIndex.Count == 0)
                    {
                        throw new FormatException(string.Format("Index {2} Keyword {0} is not paired correctly, no required paired keywords provided after\nRequired pair keywords: {1}", keywd.Name, String.Join(" ", ((IPairedKeyword)keywd).PairAfter.ToArray()), _keywordList.IndexOf(keywd)));

                    }
                    else if (((IPairedKeyword)keywd).PairAfter.Count > 0)
                    {
                        if (((IPairedKeyword)_keywordList[PairedAfterIndex[0]]).Paired == false)
                        {
                            ((IPairedKeyword)_keywordList[PairedAfterIndex[0]]).Paired = true;
                        }
                        else
                        {
                            if (((IPairedKeyword)_keywordList[PairedAfterIndex[0]]).PairType == KeywordPairType.One2One)
                            {

                                throw new FormatException(string.Format("Index {2} Keyword {0} is not paired correctly, required paired keywords already paired after\nRequired pair keywords: {1}", keywd.Name, String.Join(" ", ((IPairedKeyword)keywd).PairAfter.ToArray()), _keywordList.IndexOf(keywd)));

                            }
                            else if(((IPairedKeyword)_keywordList[PairedAfterIndex[0]]).PairType == KeywordPairType.One2n)
                            {
                                //if all required keyword paired
                                if (PairedAfterIndex.TrueForAll(x => ((IPairedKeyword)_keywordList[x]).Paired == true))
                                {
                                    //if all keyword paired and type is one2one, fault situation
                                    if (PairedAfterIndex.TrueForAll(x => ((IPairedKeyword)_keywordList[x]).PairType == KeywordPairType.One2One))
                                    {
                                        throw new FormatException(string.Format("Index {2} Keyword {0} is not paired correctly, all one2one required paired keywords already paired after\nRequired pair keywords: {1}", keywd.Name, String.Join(" ", ((IPairedKeyword)keywd).PairAfter.ToArray()), _keywordList.IndexOf(keywd)));
                                    }
                                    else
                                    {
                                        //all  required keyword paired but they have one2n paire,nothing to do

                                    }

                                }
                                else
                                {
                                    //set the next unpaired keyword to paired
                                    ((IPairedKeyword)_keywordList[PairedAfterIndex.First(x=>( (IPairedKeyword)_keywordList[x]).Paired == false)]).Paired = true;

                                }

                            }

                        }
                        
                    }      

                }//end of if Ikeyword

                //check usagelimit
                if (_keywordList.ToList().FindAll(x=>x.Name == keywd.Name).Count > keywd.UsageLimit)
                {
                    throw new FormatException(string.Format("Index {3} Keyword {0} is exceed occurance limit, found {1} times, limit {2} times", keywd.Name, _keywordList.ToList().FindAll(x => x.Name == keywd.Name).Count,keywd.UsageLimit, _keywordList.IndexOf(keywd)));

                }


            }//end of foreach keyword

        }
        /// <summary>
        /// Clone object by serialization
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            TestSequence testSequence;

            MemoryStream memoryStream = new MemoryStream();
            DataContractSerializer formatter = new DataContractSerializer(typeof(TestSequence));
            formatter.WriteObject(memoryStream, this);
            memoryStream.Position = 0;
            testSequence = (TestSequence)formatter.ReadObject(memoryStream);

            return testSequence;
        }

        /// <summary>
        /// Convert test sequence to script, will be later included into test case script
        /// </summary>
        /// <returns></returns>
        /// 
        [DataMember]
        public string TestSequenceScript
        {

            get
            {
                List<string> templist = new List<string>();
                List<DependentParameter> paraSet = new List<DependentParameter>();

                string testSeqParameters = "";
                string testSequence;
#pragma warning disable CS0219 // The variable 'i' is assigned but its value is never used
                int i = 1;
#pragma warning restore CS0219 // The variable 'i' is assigned but its value is never used

                //get test sequence and all para list
                foreach (Keyword kw in _keywordList)
                {
                    templist.Add("'" + kw.ScriptName + "'");
                    if (kw is IDependentKeyword && ((IDependentKeyword)kw).ParametrizationType != DependentKeywordParameterizationType.InlineParameterized)
                    {
                        foreach (DependentParameter dp in ((IDependentKeyword)kw).DependentParameters)
                        {
                            paraSet.Add(dp);

                        }

                    }

                }

                //process paralist
                List<string> ProcessedDP = new List<string>();

                foreach (DependentParameter dp in paraSet)
                {
                    if (ProcessedDP.Contains(dp.Name)) { continue; } // next if the parameter is handled already
                    List<DependentParameter> listDP = paraSet.FindAll(X => X.Name == dp.Name);

                    if (dp is ListDependentParameter)
                    {
                        if (((ListDependentParameter)dp).ValueList == null) { continue; }   // handle for do_line_manipulation, becasue the interrupt or short may be null
                        ObservableCollection<string> MasterValueList = new ObservableCollection<string>();
                        foreach (DependentParameter lsDP in listDP)
                        {
                            foreach (string val in ((ListDependentParameter)lsDP).ValueList)
                            {
                                MasterValueList.Add(val);
                            }

                        }
                        ListDependentParameter tempDP = new ListDependentParameter(dp.Name, dp.DefaultValue, MasterValueList, dp.DefaultValue);
                        testSeqParameters = testSeqParameters + tempDP.Name + TestScript.SpacesTidy(tempDP.Name) + " = " + tempDP.Value + "\n";


                    }
                    else if (dp is HashDependentParameter)
                    {
                        Dictionary<string, string> MasterKeyValuePairs = new Dictionary<string, string>();

                        if (dp.Name == "edit_CAN_signal")
                        {
                            Dictionary<string, ListDependentParameter> refSigValue = new Dictionary<string, ListDependentParameter>();

                            foreach (DependentParameter lsDP in listDP)
                            {
                                foreach (var val in ((HashDependentParameter)lsDP).KeyValuePairList)
                                {
                                    if (!MasterKeyValuePairs.ContainsKey(val.Key))
                                    {
                                        MasterKeyValuePairs.Add(val.Key, val.Value);
                                    }
                                    else
                                    {
                                        if (!refSigValue.ContainsKey(val.Key))
                                        {
                                            refSigValue.Add(val.Key, new ListDependentParameter(val.Key, "", new ObservableCollection<string> { MasterKeyValuePairs[val.Key], }, ""));
                                            MasterKeyValuePairs[val.Key] = "value_list_" + val.Key;
                                            refSigValue[val.Key].AddValue(val.Value);
                                        }
                                        else
                                        {
                                            refSigValue[val.Key].AddValue(val.Value);
                                        }

                                    }

                                }
                            }
                            HashDependentParameter tempDP = new HashDependentParameter(dp.Name, dp.DefaultValue, MasterKeyValuePairs, dp.DefaultValue);
                            testSeqParameters = testSeqParameters + tempDP.Name + TestScript.SpacesTidy(tempDP.Name) + " = " + tempDP.Value + "\n";
                            foreach (var sig in refSigValue)
                            {
                                testSeqParameters = testSeqParameters + sig.Value.Name + TestScript.SpacesTidy(sig.Value.Name) + " = " + sig.Value.Value + "\n";
                            }
                        }
                        else if (dp.Name == "set_model_value_to_constant")
                        {
                            int j = 1;
                            foreach (DependentParameter lsDP in listDP)
                            {
                                if (j == 1)
                                {
                                    testSeqParameters = testSeqParameters + lsDP.Name + TestScript.SpacesTidy(lsDP.Name) + " = " + lsDP.Value + "\n";
                                }
                                else
                                {

                                    testSeqParameters = testSeqParameters + lsDP.Name + "_" + j + TestScript.SpacesTidy(lsDP.Name + "_" + j) + " = " + lsDP.Value + "\n";
                                }
                                j++;
                            }
                        }
                        else
                        {
                            foreach (DependentParameter lsDP in listDP)
                            {
                                foreach (var val in ((HashDependentParameter)lsDP).KeyValuePairList)
                                {
                                    if (!MasterKeyValuePairs.ContainsKey(val.Key))
                                    {
                                        MasterKeyValuePairs.Add(val.Key, val.Value);
                                    }
                                    else
                                    {
                                        if (MasterKeyValuePairs[val.Key] != val.Value)
                                        {

                                            throw new FormatException(string.Format("Hash Parameter {0} is double defined conflict value : {1} vs {2}", dp.Name, MasterKeyValuePairs[val.Key], val.Value));

                                        }
                                        else
                                        {
                                            //do nothing

                                        }

                                    }

                                }

                            }
                            HashDependentParameter tempDP = new HashDependentParameter(dp.Name, dp.DefaultValue, MasterKeyValuePairs, dp.DefaultValue);
                            testSeqParameters = testSeqParameters + tempDP.Name + TestScript.SpacesTidy(tempDP.Name) + " = " + tempDP.Value + "\n";
                        }

                    }
                    ProcessedDP.Add(dp.Name);

                }//end of foreach keyord


                testSequence = "test_sequence" + TestScript.SpacesTidy("test_sequence") + " = @(" + String.Join(",", templist) + @")";

                return testSequence + "\n" + testSeqParameters;

            }

            private set { }
        }

    }
}
