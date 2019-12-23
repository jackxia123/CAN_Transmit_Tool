using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RBT.Universal
{
    /// <summary>
    /// Parser for JC universal script
    /// 1. Gen Test Spec out of script
    /// 2. Data retrive from serilization 
    /// </summary>
    public class ScriptParser
    {
        /// <summary>
        /// parameterless ctor
        /// </summary>
        public ScriptParser() { }
        /// <summary>
        /// from generated text strings
        /// </summary>
        /// <param name="Script"></param>
        public ScriptParser(string Script)
        {


        }

        public string TestCaseName { get; set; }
        public string TestPurpose { get; set; }
        public ObservableCollection<Dictionary<string, string>> TestSteps { get; set; }
        public ObservableCollection<string> SupportedKeyWords { get;  }


    }
}
