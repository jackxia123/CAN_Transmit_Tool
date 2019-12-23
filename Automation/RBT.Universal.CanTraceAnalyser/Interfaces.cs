using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RBT.Universal.Keywords;
using System.Xml.Serialization;

namespace RBT.Universal.CanEvalParameters
{
    public enum CheckConditionMessage { TimeOut, DLCFail , Init,}
    public enum TriggerConditionSignal { Equal, Greater, Less }

    public interface ICanEvalParameter
    {
        DependentParameter EvalParameter { get; set; }
        bool ArgumentsValidated();    
    }


    public interface TriggerBase
    {
         int TriggerID { get; set; }
         string TriggerName { get; }

    }

    public interface ICondition
    {
        

    }

    public interface IStartStopTime
    {


    }



}
