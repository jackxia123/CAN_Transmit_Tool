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
    public class StartDiagComRB : PairedKeyword
    {


        public StartDiagComRB()
        {
            base.Name = "start_diag_communication_RB";
            base.Description = @"Start Bosch diagnosis communication. 

>>The wakeup pattern & start comm. command is sent by Samtec.
>>Use along with ‘stop_diag_communication_RB’

*2s delay just after running in customer diagnosis communication.

";
            Delay = 0;
            UsageLimit = 255;
            PairType = KeywordPairType.One2One;
            AddPairAfter("stop_diag_communication_RB");

        }

        
       

    }
}
