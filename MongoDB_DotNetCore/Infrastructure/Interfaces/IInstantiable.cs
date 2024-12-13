using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IInstantiable<T>
{
    public static abstract T CreateInstance(JObject data, DataContainer parentContainer, bool allowEdit = false, bool allowInPlaceEditing = false);
}
