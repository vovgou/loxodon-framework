using System;

namespace Loxodon.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Enum)]
    public class RemarkAttribute : Attribute
    {
        private string remark;
        public RemarkAttribute(string remark)
        {
            this.remark = remark;
        }

        public string Remark { get { return this.remark; } }
    }
}
