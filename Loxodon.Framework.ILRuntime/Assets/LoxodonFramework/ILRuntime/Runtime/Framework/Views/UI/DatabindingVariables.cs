using Loxodon.Framework.Views.Variables;
using UnityEngine;

namespace Loxodon.Framework.Views
{

    public class DatabindingVariables : MonoBehaviour
    {
        public VariableArray variablesArray;

        public T Get<T>(string name)
        {
            return variablesArray.Get<T>(name);
        }
    }
}
