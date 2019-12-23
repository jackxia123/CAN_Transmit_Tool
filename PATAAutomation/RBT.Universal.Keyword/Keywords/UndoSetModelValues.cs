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
    /// KeyWord Stimuli Stop
    /// </summary>
    /// 
    [Serializable]
    public class UndoSetModelValues : PairedKeyword
    {
        private ObservableCollection<string> _pairBefore = new ObservableCollection<string>();

        public UndoSetModelValues()
        {
            Name = "undo_set_model_values";            
            Description = @"Set back to the values provided by LabCar model. (switch back to closed loop)

>>Use along with ‘set_model_values’.

";
            Delay = 0;
            UsageLimit = 1;
            AddPairBefore("set_model_values");
            PairType = KeywordPairType.One2n;


        }

        

    }
}
