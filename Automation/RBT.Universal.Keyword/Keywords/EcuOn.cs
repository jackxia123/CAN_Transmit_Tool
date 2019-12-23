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
    /// KeyWord ECU On
    /// </summary>
    /// 
    [Serializable]
    public class EcuOn : PairedKeyword
    {


        public EcuOn()
        {
            base.Name = "ecu_on";
            base.Description = @"Turn ECU ON.

>> Use along with ‘ecu_off’.

e.g.test_sequence = @(‘ecu_on’,…,’ecu_off’)
";
            base.Delay = 0;
            base.UsageLimit = 255;
            base.AddPairAfter("ecu_off");
            PairType = KeywordPairType.One2One;
        }

        
       

    }
}
