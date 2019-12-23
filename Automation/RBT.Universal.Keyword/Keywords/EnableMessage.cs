using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace RBT.Universal.Keywords
{
    /// <summary>
    /// KeyWord EnableMessage
    /// </summary>
    /// 
    [Serializable]
    public class EnableMessage : PairedDependentKeyword
    {
        private ObservableCollection<string> _pairBefore = new ObservableCollection<string>();
        public EnableMessage()
        {
            InitKeyword();
        }
        public EnableMessage(string msgName)
        {
            InitKeyword();
            ((ScalarDependentParameter)DependentParameters.First(x => x.Name == "message")).ScalarValue = msgName;
        }

        private void InitKeyword()
        {
            Name = "enableMessage_";            
            Description = @"Enable the CAN message which is disabled by ‘disableMessage_xxx’.

>>‘xxx’ = CAN message name defined in CAN mapping file.
>>Use along with ‘disableMessage_xxx’.

e.g. test_sequence = @(…,’enableMessage_Msg_A’,…)


";
            Delay = 0;
            UsageLimit = 255;
            PairType = KeywordPairType.One2n;


            AddDependentParameter(new ScalarDependentParameter("message", "Message name to be Enabled", "", ""));
            ParameterCompositionOption = DependentKeywordParameterCompositionOption.Unique;
            ParametrizationType = DependentKeywordParameterizationType.InlineParameterized;


        }

        public override bool ParametersValidated()
        {
            
            if (!String.IsNullOrEmpty(((ScalarDependentParameter)DependentParameters.First(x => x.Name == "message")).Value))
            {
                return true;

            }

            return false;

        }

        public override ObservableCollection<string> PairBefore
        {
            get
            {

                _pairBefore = new ObservableCollection<string>() { ScriptName.Replace(Name, "disableMessage_") };
              
                RaisePropertyChanged();
                return _pairBefore;
            }

            protected set
            {
                _pairBefore = value;
                SetProperty(ref _pairBefore, value);
            }
        }


    }
}
