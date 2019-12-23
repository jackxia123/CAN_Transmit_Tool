using RBT.Universal.Keywords;
using System.Runtime.Serialization;

namespace RBT.Universal
{
    [DataContract]
    public class Variant: Model
    {
        private int _rbVariantNo;
#pragma warning disable CS0169 // The field 'Variant.cusVariantNo' is never used
        private string cusVariantNo;
#pragma warning restore CS0169 // The field 'Variant.cusVariantNo' is never used
        [DataMember]
        public int RbVariant
        { get { return _rbVariantNo; }
            set { _rbVariantNo = value; SetProperty(ref _rbVariantNo, value); }
        }

        public Variant() { }
        public Variant(int rbVarNo)
        {

            _rbVariantNo = rbVarNo;
        }
        public override string ToString()
        {
            return "Var"+_rbVariantNo.ToString();
        }
    }
}
