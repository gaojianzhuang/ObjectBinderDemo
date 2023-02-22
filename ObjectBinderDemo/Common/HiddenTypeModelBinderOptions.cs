using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObjectBinderDemo
{
    public class HiddenTypeModelBinderOptions
    {
        public HiddenTypeModelBinderOptions()
        {
            SupportedTypes = new List<Type>();
        }
        public List<Type> SupportedTypes { get; }
    }
}
