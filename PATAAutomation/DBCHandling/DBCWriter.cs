using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace DBCHandling
{
    public class DBCWriter
     {
        private StreamWriter swDBC;
        private List<string > nodes = new List<string>();
        public DBCWriter(string filePath)
        {
            FileStream fsDBC = new FileStream(filePath,FileMode.Create,FileAccess.ReadWrite);
            swDBC = new StreamWriter(fsDBC);
        }

        public void close()
        {
            swDBC.Close();
        }

         public void writeDBC(string version,string customer,string project,string baudRate, ObservableCollection<DbcMessage> Messages)
         {
             //Get all transmitter in Messages as Nodes
             foreach (var Message in Messages)
             {
                 foreach (var transmitter in Message.Transmitters)
                 {
                     if (!nodes.Contains(transmitter))
                     {
                         nodes.Add(transmitter);
                     }
                 }

             }

             this.writeHeader(version);
             swDBC.WriteLine();//blank new line
             this.writeBS();
             swDBC.WriteLine();
             writeNode(nodes.ToArray());
             swDBC.WriteLine();
             writeMessage(Messages);
             swDBC.WriteLine();
             writeMessageTransmitter(Messages);
             swDBC.WriteLine();
             writeUserDefAttrDef();
             swDBC.WriteLine();
            
             writeDBAttr(customer,project,baudRate,nodes.ToArray());
             swDBC.WriteLine();
             writeMessageAttr(Messages);
             swDBC.WriteLine();
             writeSigInitVal(Messages);
             swDBC.WriteLine();
             writeSigValDesc(Messages);

             
         }

         private void writeHeader(string version)
         {
             /*4 Version and New Symbol Specification
                The DBC files contain a header with the version and the new symbol entries. The
                version either is empty or is a string used by CANdb editor.
                version = ['VERSION' '"' { CANdb_version_string } '"' ];
                new_symbols = [ '_NS' ':' ['CM_'] ['BA_DEF_'] ['BA_'] ['VAL_']
                ['CAT_DEF_'] ['CAT_'] ['FILTER'] ['BA_DEF_DEF_'] ['EV_DATA_']
                ['ENVVAR_DATA_'] ['SGTYPE_'] ['SGTYPE_VAL_'] ['BA_DEF_SGTYPE_']
                ['BA_SGTYPE_'] ['SIG_TYPE_REF_'] ['VAL_TABLE_'] ['SIG_GROUP_']
                ['SIG_VALTYPE_'] ['SIGTYPE_VALTYPE_'] ['BO_TX_BU_']
                ['BA_DEF_REL_'] ['BA_REL_'] ['BA_DEF_DEF_REL_'] ['BU_SG_REL_']
                ['BU_EV_REL_'] ['BU_BO_REL_'] ];*/

             //leave Version empty
             swDBC.WriteLine(@"VERSION "+"\""+version+"\"");
             swDBC.WriteLine();

             //Write New Symbol Specification  

             swDBC.WriteLine(@"NS_ : 
	NS_DESC_
	CM_
	BA_DEF_
	BA_
	VAL_
	CAT_DEF_
	CAT_
	FILTER
	BA_DEF_DEF_
	EV_DATA_
	ENVVAR_DATA_
	SGTYPE_
	SGTYPE_VAL_
	BA_DEF_SGTYPE_
	BA_SGTYPE_
	SIG_TYPE_REF_
	VAL_TABLE_
	SIG_GROUP_
	SIG_VALTYPE_
	SIGTYPE_VALTYPE_
	BO_TX_BU_
	BA_DEF_REL_
	BA_REL_
	BA_DEF_DEF_REL_
	BU_SG_REL_
	BU_EV_REL_
	BU_BO_REL_
	SG_MUL_VAL_");

         }

         private void writeBS()
        {
            /*5 Bit Timing Definition
            The bit timing section defines the baudrate and the settings of the BTR registers of
            the network This section is obsolete and not used any more. Nevertheless he
            keyword 'BS_' must appear in the DBC file.
            bit_timing = 'BS_:' [baudrate ':' BTR1 ',' BTR2 ] ;
            baudrate = unsigned_integer ;
            BTR1 = unsigned_integer ;
            BTR2 = unsigned_integer ;*/

             swDBC.WriteLine("BS_:");
        }
        
        private void writeNode(string [] nodes)
        {
            /*6 Node Definitions
                The node section defines the names of all participating nodes The names defined
                in this section have to be unique within this section.
                nodes = 'BU_:' {node_name} ;
                node_name = C_identifier ;*/
            swDBC.WriteLine("BU_: "+String.Join(" ",nodes));
        }
        
        private void writeValueTable(List<DbcSignal> allSignals)
        {
            /*7 Value Table Definitions
                The value table section defines the global value tables. The value descriptions in
                value tables define value encodings for signal raw values. In commonly used DBC
                files the global value tables aren't used, but the value descriptions are defined for
                each signal independently.
                value_tables = {value_table} ;
                value_table = 'VAL_TABLE_' value_table_name {value_description}
                ';' ;
                value_table_name = C_identifier ;
                7.1 Value Descriptions (Value Encodings)
                A value description defines a textual description for a single value. This value may
                either be a signal raw value transferred on the bus or the value of an environment
                variable in a remaining bus simulation.
                value_description = double char_string ;*/
            foreach (DbcSignal signal in allSignals)
            {   
                //Todo: To clarify the Keyword is valid or not
                //swDBC.WriteLine(); 
            }
                
        }

         private void writeMessage(ObservableCollection<DbcMessage> AllMessages)
         {
             /*8 Message Definitions
                The message section defines the names of all frames in the cluster as well as their
                properties and the signals transferred on the frames.
                messages = {message} ;
                message = BO_ message_id message_name ':' message_size transmitter
                {signal} ;
                message_id = unsigned_integer ;
                The message's CAN-ID. The CAN-ID has to be unique within the DBC file. If the
                most significant bit of the CAN-ID is set, the ID is an extended CAN ID. The extended
                CAN ID can be determined by masking out the most significant bit with the
                mask 0xCFFFFFFF.
                message_name = C_identifier ;
                The names defined in this section have to be unique within the set of messages.
                message_size = unsigned_integer ;
                The message_size specifies the size of the message in bytes.
                transmitter = node_name | 'Vector__XXX' ;
                The transmitter name specifies the name of the node transmitting the message.
                The sender name has to be defined in the set of node names in the node section.
                If the massage shall have no sender, the string 'Vector__XXX' has to be given*/


             /*
              8.1 Signal Definitions
             The message's signal section lists all signals placed on the message, their position
             in the message's data field and their properties.
             signal = 'SG_' signal_name multiplexer_indicator ':' start_bit '|'
             signal_size '@' byte_order value_type '(' factor ',' offset ')'
             '[' minimum '|' maximum ']' unit receiver {',' receiver} ;
             signal_name = C_identifier ;
             The names defined here have to be unique for the signals of a single message.
             multiplexer_indicator = ' ' | 'M' | m multiplexer_switch_value ;
             The multiplexer indicator defines whether the signal is a normal signal, a multiplexer
             switch for multiplexed signals, or a multiplexed signal. A 'M' (uppercase)
             character defines the signal as the multiplexer switch. Only one signal within a
             single message can be the multiplexer switch. A 'm' (lowercase) character followed
             by an unsigned integer defines the signal as being multiplexed by the multiplexer
             switch. The multiplexed signal is transferred in the message if the switch value of
             the multiplexer signal is equal to its multiplexer_switch_value.
             start_bit = unsigned_integer ;
             The start_bit value specifies the position of the signal within the data field of the
             frame. For signals with byte order Intel (little endian) the position of the leastsignificant
             bit is given. For signals with byte order Motorola (big endian) the position
             of the most significant bit is given. The bits are counted in a sawtooth manner.
             The startbit has to be in the range of 0 to (8 * message_size - 1).
             signal_size = unsigned_integer ;
             The signal_size specifies the size of the signal in bits
             byte_order = '0' | '1' ; (* 0=big endian, 1=little endian *)
             The byte_format is 0 if the signal's byte order is Motorola (big endian) or 1 if the
             byte order is Intel (little endian).
             value_type = '+' | '-' ; (* +=unsigned, -=signed *)
             The value_type defines the signal as being of type unsigned (-) or signed (-).
             factor = double ;
             offset = double ;
             The factor and offset define the linear conversion rule to convert the signals raw
             value into the signal's physical value and vice versa:
             physical_value = raw_value * factor + offset
             raw_value = (physical_value – offset) / factor
             As can be seen in the conversion rule formulas the factor must not be 0.
             minimum = double ;
             maximum = double ;
              */
             
             foreach (var Message in AllMessages)
             {
                 string valuTypeDBC;
                 swDBC.WriteLine(String.Format("BO_ {0} {1}: {2} {3}", Message.ID, Message.Name, Message.DLC, Message.Transmitters.Count()==0 ? "Vector__XXX" : Message.Transmitters[0]));

                 foreach (var signal in Message.Signals)
                 {
                     /*Signals with value types 'float' and 'double' have additional entries in the signal_
                       valtype_list section.
                       signal_extended_value_type_list = 'SIG_VALTYPE_' message_id signal_
                       name signal_extended_value_type ';' ;
                       signal_extended_value_type = '0' | '1' | '2' | '3' ; (* 0=signed or
                       unsigned integer, 1=32-bit IEEE-float, 2=64-bit IEEE-double *)
                     */

                     //Multiplexer is not supported currently
                     /*multiplexer_indicator = ' ' | 'M' | m multiplexer_switch_value ;
                        The multiplexer indicator defines whether the signal is a normal signal, a multiplexer
                        switch for multiplexed signals, or a multiplexed signal. A 'M' (uppercase)
                        character defines the signal as the multiplexer switch. Only one signal within a
                        single message can be the multiplexer switch. A 'm' (lowercase) character followed
                        by an unsigned integer defines the signal as being multiplexed by the multiplexer
                        switch. The multiplexed signal is transferred in the message if the switch value of
                        the multiplexer signal is equal to its multiplexer_switch_value.
                      */
                     if (signal.IsExtValType)
                     {
                         switch (signal.ValueType)
                         {
                             case "Unsigned":
                                 valuTypeDBC = "0";
                                 break;
                             case "Signed":
                                 valuTypeDBC = "0";
                                 break;
                             case "Float":
                                 valuTypeDBC = "1";
                                 break;
                             case "Double":
                                 valuTypeDBC = "2";
                                 break;
                             default:
                                 throw new ArgumentOutOfRangeException("ValueType out of range");
                                 
                         }

                     }
                     else
                     {
                         valuTypeDBC = String.Compare(signal.ValueType, "Signed", true) == 0 ? "-" : "+";
                     }

                     swDBC.WriteLine(String.Format(" SG_ {0} : {1}|{2}@{3}{4} ({5},{6}) [{7}|{8}] \"{9}\" {10}",
                         signal.Name, signal.StartBit,signal.Length, String.Compare(signal.ByteOrder,"Intel",true) ==  0?1:0, valuTypeDBC,
                         signal.Factor, signal.Offset, signal.Minimum, signal.Maximum, signal.Unit, signal.Receivers.Count()!=0?String.Join(",",signal.Receivers.ToArray()):"Vector__XXX"));
                    


                 }//end of foreach signal

                    swDBC.WriteLine();
             }//end of foreach message


         }

         private void writeMessageTransmitter(ObservableCollection<DbcMessage> AllMessages)
         {
             /*8.2 Definition of Message Transmitterses
       The message transmitter section enables the definition of multiple transmitter
       nodes of a single node. This is used to describe communication data for higherlayer
       protocols. This is not used to define CAN layer-2 communication.
       message_transmitters = {message_transmitter} ;
       Message_transmitter = 'BO_TX_BU_' message_id ':' {transmitter} ';' ;
            */
             foreach (var Message in AllMessages)
             {
                 if (Message.Transmitters.Count() > 1)
                 {
                     swDBC.WriteLine(String.Format("BO_TX_BU_ {0} : {1};", Message.ID, String.Join(",", Message.Transmitters.ToArray())));
                 }
             }
             
         }

         private void writeSigValDesc(ObservableCollection<DbcMessage> AllMessages)
         {
             foreach (var Message in AllMessages)
             {
                 foreach (var signal in Message.Signals)
                 {
                     if(signal.Coding.Count>0)
                     {
                         string strSigCoding="";
                         foreach (var codPair in signal.Coding)
                         {
                             strSigCoding += string.Format("{0} \"{1}\" ",codPair.Key,codPair.Value);
                         }
                         strSigCoding = strSigCoding + ";";
                         swDBC.WriteLine(string.Format("VAL_ {0} {1} {2}", Message.ID, signal.Name, strSigCoding));

                     }
                    
                 }
                 
             }
         }

         private void writeUserDefAttrDef()
         {
             string def = @"BA_DEF_  ""NmMessageCount"" INT 0 0;
BA_DEF_  ""DBName"" STRING ;
BA_DEF_ BO_  ""GenMsgStartDelayTime"" INT 0 0;
BA_DEF_ BO_  ""GenMsgDelayTime"" INT 0 0;
BA_DEF_ BO_  ""GenMsgNrOfRepetition"" INT 0 0;
BA_DEF_ BO_  ""GenMsgCycleTimeFast"" INT 0 0;
BA_DEF_ BO_  ""GenMsgCycleTime"" INT 0 0;
BA_DEF_ BO_  ""GenMsgSendType"" ENUM  ""Periodic"",""Event"",""Mixed"";
BA_DEF_ SG_  ""GenSigStartValue"" INT 0 0;
BA_DEF_ SG_  ""GenSigInactiveValue"" INT 0 0;
BA_DEF_ SG_  ""GenSigCycleTimeActive"" INT 0 0;
BA_DEF_ SG_  ""GenSigCycleTime"" INT 0 0;
BA_DEF_ SG_  ""GenSigSendType"" ENUM  ""Periodic"",""Event"",""Mixed"",""OnWrite"",""OnWriteWithRepetition"",""OnChange"",""OnChangeWithRepetition"",""IfActive"",""IfActiveWithRepetition"",""NoSigSendType"",""vector_leerstring"";
BA_DEF_  ""Baudrate"" INT 0 1000000;
BA_DEF_  ""BusType"" STRING ;
BA_DEF_  ""NmType"" STRING ;
BA_DEF_  ""Manufacturer"" STRING ;
BA_DEF_ BU_  ""NmStationAddress"" INT 0 63;
BA_DEF_ BU_  ""NmNode"" ENUM  ""no"",""yes"";
BA_DEF_ BO_  ""NmMessage"" ENUM  ""no"",""yes"";
BA_DEF_  ""NmBaseAddress"" HEX 1024 1087;
BA_DEF_ BO_  ""GenMsgILSupport"" ENUM  ""No"",""Yes"";
BA_DEF_DEF_  ""NmMessageCount"" 0;
BA_DEF_DEF_  ""DBName"" """";
BA_DEF_DEF_  ""GenMsgStartDelayTime"" 0;
BA_DEF_DEF_  ""GenMsgDelayTime"" 0;
BA_DEF_DEF_  ""GenMsgNrOfRepetition"" 0;
BA_DEF_DEF_  ""GenMsgCycleTimeFast"" 0;
BA_DEF_DEF_  ""GenMsgCycleTime"" 0;
BA_DEF_DEF_  ""GenMsgSendType"" ""Periodic"";
BA_DEF_DEF_  ""GenSigStartValue"" 0;
BA_DEF_DEF_  ""GenSigInactiveValue"" 0;
BA_DEF_DEF_  ""GenSigCycleTimeActive"" 0;
BA_DEF_DEF_  ""GenSigCycleTime"" 0;
BA_DEF_DEF_  ""GenSigSendType"" ""Periodic"";
BA_DEF_DEF_  ""Baudrate"" 125000;
BA_DEF_DEF_  ""BusType"" """";
BA_DEF_DEF_  ""NmType"" """";
BA_DEF_DEF_  ""Manufacturer"" ""XXX"";
BA_DEF_DEF_  ""NmStationAddress"" 0;
BA_DEF_DEF_  ""NmNode"" ""no"";
BA_DEF_DEF_  ""NmMessage"" ""no"";
BA_DEF_DEF_  ""NmBaseAddress"" 1024;
BA_DEF_DEF_  ""GenMsgILSupport"" ""Yes"";";

             swDBC.WriteLine(def);
             swDBC.WriteLine();
         }
         private void writeDBAttr(string customer, string project, string baudrate, string[] nodes)
         {
             string dbAttr =
                 string.Format(
                     @"BA_ ""Manufacturer"" ""{0}"";
BA_ ""NmType"" """";
BA_ ""BusType"" ""CAN"";
BA_ ""Baudrate"" {2};
BA_ ""DBName"" ""{1}"";",customer,project,baudrate );
             swDBC.WriteLine(dbAttr);

             int cntNode = 0;
             foreach (var node in nodes)
             {
                if (node == "Vector__XXX") { continue; }
                 swDBC.WriteLine(string.Format(@"BA_ ""NmNode"" BU_ {0} {1};",node,cntNode));
                 cntNode++;
             }
             
             swDBC.WriteLine();
         }

         private void writeMessageAttr(ObservableCollection<DbcMessage> AllMessages)
         {
             foreach (var message in AllMessages)
             {
                 if (message.SendType != MsgSendType.Event)
                 {
                     swDBC.WriteLine(string.Format(@"BA_ ""GenMsgCycleTime"" BO_ {0} {1};", message.ID, message.CycleTime));
                 }
                 
                 swDBC.WriteLine(string.Format(@"BA_ ""GenMsgSendType"" BO_ {0} {1};", message.ID,(int)message.SendType));

             }
         }

         private void writeSigInitVal(ObservableCollection<DbcMessage> AllMessages)
         {

             foreach (var Message in AllMessages)
             {
                 foreach (var signal in Message.Signals)
                 {

                     swDBC.WriteLine(string.Format(@"BA_ ""GenSigStartValue"" SG_ {0} {1} {2};",Message.ID,signal.Name,signal.InitValue));

                 }

             }

         }

     }
}
