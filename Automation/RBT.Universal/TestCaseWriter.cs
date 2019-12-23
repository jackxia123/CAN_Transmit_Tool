namespace RBT.Universal
{
    /// <summary>
    /// Write QC compatible test case from type test script
    /// </summary>
    public class TestCaseWriter
    {
        /// <summary>
        /// Ctor
        /// </summary>
       public TestCaseWriter() { }

        public string ID { get; set; }
        public string TestName { get; set; }
        public string Subject { get; set; }
        public string TestType { get; set; }
        public string FNID{ get; set; }
        public string RB_DoorsLink { get; set; }             
        public string Description { get; set; }
        public string ExpectedResult { get; set; }
        public string ActualResult { get; set; }
        public string Verdict { get; set; }
        public string Comment { get; set; }
        public string Designer { get; set; }
        public string TestEnviroment { get; set; }
        public string TestObject { get; set; }
        public string CAT { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string RB_AutomationScript { get; set; }
        public string RB_ScriptStatus { get; set; }
        public string RB_Product { get; set; }
    }
}
