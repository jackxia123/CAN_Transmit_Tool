using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DBCHandling
{
    public class DBCReader
    {

        Dictionary<int, int> dicInvIndex = new Dictionary<int, int>
        {
            {7,0 }, {6, 1},{5, 2},{4, 3},{3,4 },{2,5 },{1,6 },{0, 7},
            {15,8 }, {14,9 },{13,10 },{12,11 },{11, 12},{10, 13},{9,14 },{8,15 },
            {23,16 }, {22, 17},{21, 18},{20, 19},{19,20 },{18,21 },{17,22 },{16,23 },
            {31,24 }, {30,25 },{29, 26},{28, 27},{27, 28},{26,29 },{25, 30},{24,31 },
            {39,32 }, {38,33 },{37, 34},{36, 35},{35,36 },{34,37 },{33,38 },{32,39 },
            {47,40 }, {46,41 },{45, 42},{44,43 },{43, 44},{42,45 },{41,46 },{40,47 },
            {55,48 }, {54,49 },{53, 50},{52, 51},{51, 52},{50, 53},{49,54 },{48, 55},
            {63,56 }, {62,57 },{61,58 },{60,59 },{59,60 },{58,61 },{57, 62},{56, 63},
        };

        ObservableCollection<DbcMessage> _allMessage = new ObservableCollection<DbcMessage>();
        ObservableCollection<DbcMessage> _extAllMessages = new ObservableCollection<DbcMessage>();

        string _dbVersion;
        string _dbCustomer;
        string _dbBaudrate;
        string _dbName;

        public string DbVersion
        {
            get { return _dbVersion; }
            set { _dbVersion = value; }

        }
        public string DbCustomer
        {
            get { return _dbCustomer; }
            set { _dbCustomer = value; }

        }
        public string DbBaudrate
        {
            get { return _dbBaudrate; }
            set { _dbBaudrate = value; }

        }
        public string DbName
        {
            get { return _dbName; }
            set { _dbName = value; }

        }

        /// <summary>
        /// Ctor for major paser job
        /// </summary>
        /// <param name="filePath"></param>
        public DBCReader(string filePath)
        {
            _allMessage.Clear();
            FileStream fsDBC = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            StreamReader SR = new StreamReader(fsDBC);

            string line;
            Regex patternMsgDBC = new Regex(
                 @"BO_    # BO_ keyword
          \s+    # some white char
          (\d+)  # match BOTSCHAFTS ID
          \s+    # some white char
          (\w+)  # match BOTSCHAFTS NAME
          \s*    # may be some white char
          :      # the :
          \s +    # some white char
          (\d +)  # the DLC (byte count)
          \s +    # some white char
          (\w +)  # match TRANSMITTER NODE NAME
          \s*    # may be some white char", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

            Regex patternSigDBC = new Regex(@"SG_   
           \s+    
           (\w+)  # match the NAME
           \s*    # maybe some white char
           m?     # maybe multiplexed signal
           \d?    # maybe multiplexed signal
           \s*    # may be some white char
           :      # the :
           \s *    # some white char
           (\d +)  # the start POSITION (bit)
           \|     # the |
           (\d +)  # the LENGTH of signal in bit
           @      # some any char
           (\d)   # FORMAT : 1 (Intel) or 0 (Motorola)
           (\+| -) # the + or the -
           \s +    # some white char
           \(     # match the (
           (
               [+-] ?\d *\.?\d *[Ee][+-]\d+|
                  [+-] ?\d +\.\d+|
                    [+-] ?\d +\.|
                      [+-] ?\.\d +|
                       [+-] ?\d +
           ) # match the FACTOR,Resolution
           ,    # match the ,
           (
               [+-] ?\d *\.?\d *[Ee][+-]\d +|
                  [+-] ?\d +\.\d +|
                    [+-] ?\d +\.|
                      [+-] ?\.\d +|
                       [+-] ?\d +
           ) # match the OFFSET
           \)    # match the )
           \s *    # some white char
           \[    # match the [
           (
               [+-]?\d*\.?\d*[Ee][+-]\d+|
               [+-]?\d+\.\d+|
               [+-]?\d+\.|
               [+-]?\.\d+|
               [+-]?\d+
           ) # match the MINIMUM
           \|    # match the |
           (
               [+-]?\d*\.?\d*[Ee][+-]\d+|
               [+-]?\d+\.\d+|
               [+-]?\d+\.|
               [+-]?\.\d+|
               [+-]?\d+
           ) # match the MAXIMUM
           \]    # match the [
           \s*    # some white char
           \""    # match the [
           (.*)     # match the UNIT
           \""    # match the [
           \s*    # some white char
           (.*)    # network elements", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

            Regex patternSigMultiplexer = new Regex(@"                    
              SG_    # match the KEY
              \s+    # some white char
              \w+    # match the NAME
              \s+    # maybe some white char
              M      # multiplexed signal
              \s+", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

            Regex patternSigMultiplexed_value = new Regex(@"                    
              SG_    # match the KEY
              \s+    # some white char
              \w+    # match the NAME
              \s+    # maybe some white char
              M      # multiplexed signal
              (\d+)  # multiplexed signal code
              \s+", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

#pragma warning disable CS0219 // The variable 'MsgReceiver' is assigned but its value is never used
            string MsgName, MsgSender, MsgReceiver;
#pragma warning restore CS0219 // The variable 'MsgReceiver' is assigned but its value is never used
            decimal MsgID;
            int MsgDlc;
            int MsgCycle;


            string SigNAME;
            int SigPOSITION;
            int SigLENGTH;
            string SigFORMAT;
            string SigTYPE;
            double SigFACTOR;
            double SigOFFSET;
            string SigUNIT;
            List<string> SigMODULS = new List<string>();
            double SigMINIMUM;
            double SigMAXIMUM;
            string SigMULTIPLEX_VALUE;
            bool SigMULTIPLEXER;
#pragma warning disable CS0219 // The variable 'SigShortNAME' is assigned but its value is never used
            string SigShortNAME;
#pragma warning restore CS0219 // The variable 'SigShortNAME' is assigned but its value is never used
            string SigCODING;
            MsgSendType MsgSendType;

            int cnt = 0;

            DbcMessage newMessage = null;
            while ((line = SR.ReadLine()) != null)
            {
                cnt++;

                if (Regex.IsMatch(line, "___NOT_USED___")) { continue; }
                // Version,Customer,Baudrate,Project 
                if (Regex.IsMatch(line, @"VERSION \""(\S+)\"""))
                {
                    _dbVersion = null;
                    _dbVersion = Regex.Match(line, @"VERSION \""(\S+)\""").Groups[1].Value;

                }
                if (Regex.IsMatch(line, @"BA_\s*\""Manufacturer\""\s*\""(\S+)\""\s*;"))
                {

                    _dbCustomer = null;
                    _dbCustomer = Regex.Match(line, @"BA_\s*\""Manufacturer\""\s*\""(\S+)\""\s*;").Groups[1].Value;

                }
                if (Regex.IsMatch(line, @"BA_\s*\""Baudrate\""\s*(\S+)\s*;"))
                {
                    _dbBaudrate = null;
                    _dbBaudrate = Regex.Match(line, @"BA_\s*\""Baudrate\""\s*(\S+)\s*;").Groups[1].Value;

                }
                if (Regex.IsMatch(line, @"BA_\s*\""DBName\""\s*\""(\S+)\""\s*;"))
                {
                    _dbName = null;
                    _dbName = Regex.Match(line, @"BA_\s*\""DBName\""\s*\""(\S+)\""\s*;").Groups[1].Value;

                }

                //Message parsing
                if (patternMsgDBC.IsMatch(line))
                {
                    MsgName = null;
                    MsgID = 0;
                    MsgDlc = 0;
                    MsgSender = null;
                    MsgReceiver = null;

                    //new message
                    MsgID = Convert.ToInt64(patternMsgDBC.Match(line).Groups[1].Value);
                    MsgName = patternMsgDBC.Match(line).Groups[2].Value;
                    MsgDlc = Convert.ToInt16(patternMsgDBC.Match(line).Groups[3].Value);
                    MsgSender = patternMsgDBC.Match(line).Groups[4].Value;
                    //MsgReceiver = patternMsgDBC.Match(line).Groups[4].Value;

                    newMessage = new DbcMessage { ID = MsgID, DLC = MsgDlc, Name = MsgName, Transmitters = { MsgSender, }, };
                    _allMessage.Add(newMessage);

                }
                if (patternSigDBC.IsMatch(line))
                {
                    SigNAME = null;
                    SigPOSITION = 0;
                    SigLENGTH = 0;
                    SigFORMAT = null;
                    SigTYPE = null;
                    SigFACTOR = 1;
                    SigOFFSET = 0;
                    SigUNIT = null;
                    SigMODULS.Clear();
                    SigMINIMUM = 0;
                    SigMAXIMUM = 0;
                    SigMULTIPLEX_VALUE = null;
                    SigMULTIPLEXER = false;
                    SigShortNAME = null;
                    SigCODING = null;


                    SigNAME = patternSigDBC.Match(line).Groups[1].Value;
                    SigPOSITION = Convert.ToInt16(patternSigDBC.Match(line).Groups[2].Value);
                    SigLENGTH = Convert.ToInt16(patternSigDBC.Match(line).Groups[3].Value);

                    if (patternSigDBC.Match(line).Groups[4].Value.ToString() == "1")
                    {
                        SigFORMAT = "INTEL";

                    }
                    else if (patternSigDBC.Match(line).Groups[4].Value.ToString() == "0")
                    {

                        SigFORMAT = "MOTOROLA";
                    }
                    if (patternSigDBC.Match(line).Groups[4].Value.ToString() == null) { throw new Exception("Signal Format Error"); };

                    if (patternSigDBC.Match(line).Groups[5].Value.ToString() == "+")
                    {
                        SigTYPE = "UNSIGNED";

                    }
                    else if (patternSigDBC.Match(line).Groups[5].Value.ToString() == "-")
                    {

                        SigTYPE = "SIGNED";
                    }
                    if (patternSigDBC.Match(line).Groups[5].Value.ToString() == null) { throw new Exception("Signal Type Error"); };



                    SigFACTOR = Convert.ToDouble(patternSigDBC.Match(line).Groups[6].Value);
                    SigOFFSET = Convert.ToDouble(patternSigDBC.Match(line).Groups[7].Value);
                    SigMINIMUM = Convert.ToDouble(patternSigDBC.Match(line).Groups[8].Value);
                    SigMAXIMUM = Convert.ToDouble(patternSigDBC.Match(line).Groups[9].Value);
                    SigUNIT = patternSigDBC.Match(line).Groups[10].Value.ToString();


                    char[] delimiters = new char[] { ',', };
                    SigMODULS = patternSigDBC.Match(line).Groups[11].Value.ToString().Split(delimiters).ToList<string>();
                    SigMODULS.ForEach(X => X.Replace(" ", ""));

                    if (patternSigMultiplexer.IsMatch(line))
                    {
                        SigMULTIPLEXER = true;
                    }

                    if (patternSigMultiplexed_value.IsMatch(line))
                    {
                        SigMULTIPLEX_VALUE = patternSigMultiplexed_value.Match(line).Groups[1].Value;
                    }

                    DbcSignal newSig = new DbcSignal
                    {
                        Name = SigNAME,
                        StartBit = SigPOSITION,
                        Length = SigLENGTH,
                        ByteOrder = SigFORMAT,
                        ValueType = SigTYPE,
                        Factor = SigFACTOR,
                        Offset = SigOFFSET,
                        Maximum = SigMAXIMUM,
                        Minimum = SigMINIMUM,
                        Receivers = new ObservableCollection<string>(SigMODULS),
                        Unit = SigUNIT,

                    };


                    if (SigMULTIPLEXER)
                    {
                        newSig.IsMultiPlexor = true;
                        newMessage.Multiplexer = newSig;
                    }
                    if (SigMULTIPLEX_VALUE != null) { newSig.MultiPlex_value = SigMULTIPLEX_VALUE; }

                    foreach (var rxNode in SigMODULS)
                    {
                        if ((newMessage != null) && (!newMessage.Receivers.Contains(rxNode)))
                        {
                            newMessage.Receivers.Add(rxNode);
                        }

                    }
                    newSig.InMessage = newMessage.ID;
                    newMessage.Signals.Add(newSig);


                }//end of if signal
                // END OF " SG_ ST_QUAL_GPS : 56|8@1+ (0.5,0) [0|0] "%"  ACC,EGS_SSG"

                // COLLECT CYCLETIME
                // BA_ "GenMsgCycleTime" BO_ 431 20;
                if (Regex.IsMatch(line, @"BA_\s*\""GenMsgCycleTime\""\s*BO_\s*(\d+)\s*(\d+)\s*;"))
                {
                    MsgID = Convert.ToInt64(Regex.Match(line, @"BA_\s*\""GenMsgCycleTime\""\s*BO_\s*(\d+)\s*(\d+)\s*;").Groups[1].Value);
                    MsgCycle = Convert.ToInt16(Regex.Match(line, @"BA_\s*\""GenMsgCycleTime\""\s*BO_\s*(\d+)\s*(\d+)\s*;").Groups[2].Value);

                    _allMessage.First(x => x.ID == MsgID).CycleTime = MsgCycle;
                }
                // Send Type
                // BA_ "GenMsgSendType" BO_ 759 0;
                if (Regex.IsMatch(line, @"BA_\s*\""GenMsgSendType\""\s*BO_\s*(\d+)\s*(\d+)\s*;"))
                {
                    MsgID = Convert.ToInt64(Regex.Match(line, @"BA_\s*\""GenMsgSendType\""\s*BO_\s*(\d+)\s*(\d+)\s*;").Groups[1].Value);
                    MsgSendType = (MsgSendType)Convert.ToInt16(Regex.Match(line, @"BA_\s*\""GenMsgSendType\""\s*BO_\s*(\d+)\s*(\d+)\s*;").Groups[2].Value);

                    _allMessage.First(x => x.ID == MsgID).SendType = MsgSendType;
                }


                // VAL_ 273 TQI_ACORValid 1 "Valid" 0 "Invalid" ;
                if (Regex.IsMatch(line, @"VAL_ (\d+) (\w+) (.*)\s*;$"))
                {
                    MsgID = Convert.ToInt64(Regex.Match(line, @"VAL_ (\d+) (\w+) (.*)\s*;$").Groups[1].Value);
                    SigNAME = Regex.Match(line, @"VAL_ (\d+) (\w+) (.*)\s*;$").Groups[2].Value;
                    SigCODING = Regex.Match(line, @"VAL_ (\d+) (\w+) (.*)\s*;$").Groups[3].Value;

                    if (SigNAME == "ACC_InterSysInfoDisp")
                    {
                        Console.WriteLine("It is successful");
                    }
                    DbcMessage tempMsg = _allMessage.First(x => x.ID == MsgID);

                    ObservableCollection<KeyValuePair<double, string>> tempDic = new ObservableCollection<KeyValuePair<double, string>>();

                    Regex patternSigCoding = new Regex(@"((0x[\dA-Fa-f]+)|(\d+\.?\d*))\s+\""\s*((\w+\(*\)*\s*)*\.?)\s*\""", RegexOptions.IgnoreCase);

                    MatchCollection coding = patternSigCoding.Matches(SigCODING);

                    if (coding.Count >= 0)   //get count tp get the exception in lazy evaluation
                    {
                        foreach (Match pair in coding)
                    {
                        double value = Convert.ToDouble(pair.Groups[1].Value);
                        string Description = pair.Groups[4].Value;
                        tempDic.Add(new KeyValuePair<double, string>(value, Description));

                    }

                    }

                    if (tempDic.Count >= 0)
                    {
                        if (tempMsg.Signals.FirstOrDefault(x => x.Name == SigNAME) != null)
                    {
                        tempMsg.Signals.First(x => x.Name == SigNAME).Coding = tempDic;
                    }
                    }

                }


            }//end of while

        }//end of ctor


        /// <summary>
        /// Get Raw Messages from DBC
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<DbcMessage> GetAllMessages()
        {
            return _allMessage;
        }
        /// <summary>
        /// Get Raw Signals from DBC
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<DbcSignal> GetAllSignals()
        {
            ObservableCollection<DbcSignal> tempAllSigs = new ObservableCollection<DbcSignal>();
            foreach (var Msg in _allMessage)
            {
                foreach (var Sig in Msg.Signals)
                {
                    tempAllSigs.Add(Sig);
                }
            }

            return tempAllSigs;
        }



        /// <summary>
        /// Get the NOTUSED BITS INLUDED in Messages
        /// </summary>
        /// <returns></returns>

        public ObservableCollection<DbcMessage> ExtGetAllMessages()
        {

            _extAllMessages.Clear();

            foreach (DbcMessage tempCANMsg in _allMessage)
            {
                //loop to get empty bits
                string msgByteOrder = null;
                bool[] arrFlag = new bool[64];
                foreach (DbcSignal tempCANSig in tempCANMsg.Signals)
                {
                    if (msgByteOrder == null) { msgByteOrder = tempCANSig.ByteOrder; };
                    if (tempCANSig.ByteOrder == "MOTOROLA")
                    {
                        for (int i = dicInvIndex[tempCANSig.StartBit]; i < dicInvIndex[tempCANSig.StartBit] + tempCANSig.Length; i++)
                        {
                            arrFlag[dicInvIndex[i]] = true;
                        }
                    }
                    else if (tempCANSig.ByteOrder == "INTEL")
                    {
                        for (int i = tempCANSig.StartBit; i < tempCANSig.StartBit + tempCANSig.Length; i++)
                        {
                            arrFlag[i] = true;
                        }

                    }
                    else
                    {
                        throw new Exception(String.Format("Byte Order of Message {0} Invalid", tempCANMsg.ID));

                    }

                }

                //Add signals of signal bit into List of CANDBCMessages if not used
                //Naming convention NotUsed_MsgXXX_ByteX_BitXX
                if (msgByteOrder == "MOTOROLA")
                {
                    bool last = true;
                    int startBit = 0;

                    for (int i = 0; i < tempCANMsg.DLC * 8; i++)
                    {
                        if (!arrFlag[dicInvIndex[i]])
                        {
                            if (last == true) //new start blank bit found
                            {
                                startBit = dicInvIndex[i];
                            }
                            else if (i == tempCANMsg.DLC * 8 - 1)
                            {

                                DbcSignal newSignal = CreateNewSignal(msgByteOrder, tempCANMsg, startBit, i - dicInvIndex[startBit] + 1);
                                tempCANMsg.Signals.Add(newSignal);

                            }

                        }
                        else                 //last is false and current is true, create new Notused signal
                        {
                            if (last == false)
                            {
                                DbcSignal newSignal = CreateNewSignal(msgByteOrder, tempCANMsg, startBit, i - dicInvIndex[startBit]);
                                tempCANMsg.Signals.Add(newSignal);
                            }
                        }

                        last = arrFlag[dicInvIndex[i]];
                    }
                }
                else if (msgByteOrder == "INTEL")
                {
                    bool last = true;
                    int startBit = 0;

                    for (int i = 0; i < tempCANMsg.DLC * 8; i++)
                    {
                        if (!arrFlag[i])
                        {
                            if (last == true) //new start blank bit found
                            {
                                startBit = i;
                            }
                            else if (i == tempCANMsg.DLC * 8 - 1)
                            {

                                DbcSignal newSignal = CreateNewSignal(msgByteOrder, tempCANMsg, startBit, i - startBit + 1);
                                tempCANMsg.Signals.Add(newSignal);

                            }

                        }
                        else                 //last is false and current is true, create new Notused signal
                        {
                            if (last == false)
                            {
                                DbcSignal newSignal = CreateNewSignal(msgByteOrder, tempCANMsg, startBit, i - startBit);
                                tempCANMsg.Signals.Add(newSignal);
                            }
                        }

                        last = arrFlag[i];
                    }
                }


                _extAllMessages.Add(tempCANMsg);
            }

            return _extAllMessages;

        }


        /// <summary>
        /// Get the NOTUSED BITS INLUDED in Signals
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<DbcSignal> ExtGetAllSignals()
        {
            ObservableCollection<DbcSignal> tempAllSigs = new ObservableCollection<DbcSignal>();
            foreach (var Msg in _extAllMessages)
            {
                foreach (var Sig in Msg.Signals)
                {
                    tempAllSigs.Add(Sig);
                }
            }

            return tempAllSigs;
        }

        /// <summary>
        /// private function to create new signals
        /// </summary>
        /// <param name="msgByteOrder"></param>
        /// <param name="MsgName"></param>
        /// <param name="startBit"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        private DbcSignal CreateNewSignal(string msgByteOrder, DbcMessage Msg, int startBit, int Length)
        {
            DbcSignal newSignal = new DbcSignal();
            newSignal.ByteOrder = msgByteOrder;
            newSignal.Name = "NotUsed_" + Msg.Name + "_bit" + startBit + "_L" + Length;
            newSignal.StartBit = startBit;
            newSignal.Length = Length;
            newSignal.ValueType = "Unsigned";
            newSignal.InitValue = 0;
            newSignal.Factor = 1;
            newSignal.Offset = 0;
            newSignal.Transmitters = Msg.Transmitters;

            return newSignal;

        }
    }
}
