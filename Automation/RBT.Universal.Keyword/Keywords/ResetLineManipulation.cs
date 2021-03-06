﻿using System;
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
    public class ResetLineManipulation : PairedKeyword
    {


        public ResetLineManipulation()
        {
            Name = "reset_Line_manipulation";            
            Description = @"Reset the line manipulations which are done by ‘do_Line_manipulation’.

>>Use along with ‘do_Line_manipulation’.


";
            Delay = 0;
            UsageLimit = 1;
            AddPairBefore("do_Line_manipulation");
            PairType = KeywordPairType.One2One;


        }

       

    }
}
