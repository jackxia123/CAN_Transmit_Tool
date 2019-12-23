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
    /// KeyWord ECU Off
    /// </summary>
    /// 
    [Serializable]
    public class EcuOff : PairedKeyword
    {

        public EcuOff()
        {
            base.Name = "ecu_off";            
            base.Description = @"Turn ECU OFF.

>>Use along with ‘ecu_on’.
";
            base.Delay = 0;
            base.UsageLimit = 255;//rough value for unlimited
            base.AddPairBefore( "ecu_on");
            PairType = KeywordPairType.One2One;

        }



    }
}
