// MIT License

// Copyright (c) 2020 Simon Schulze

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using iRLeagueDatabase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer
{
    [DataContract]
    public abstract class BaseDTO
    {
        private static IDictionary<Type, IDictionary<string, PropertyInfo>> DerivedDataMembers { get; }
        public List<KeyValuePair<string, PropertyInfo>> SerializableProperties { get; private set; }

        static BaseDTO()
        {
            DerivedDataMembers = new Dictionary<Type, IDictionary<string, PropertyInfo>>();

            // Find all derived classes in assembly
            var assemblies = new List<Assembly> { typeof(BaseDTO).Assembly };

            var derivedTypes = assemblies
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(BaseDTO).IsAssignableFrom(x));

            // Find all defined datamembers for derived types (including base class)
            foreach(var derivedType in derivedTypes)
            {
                if (DerivedDataMembers.ContainsKey(derivedType) == false)
                {
                    var dataMemberList = derivedType.GetProperties()
                        .Where(x => x.GetCustomAttribute<DataMemberAttribute>() != null)
                        .Select(x => new KeyValuePair<string, PropertyInfo>(x.GetCustomAttribute<DataMemberAttribute>().Name ?? x.Name, x));
                    IDictionary<string, PropertyInfo> dataMembers = new Dictionary<string, PropertyInfo>();
                    try
                    {
                        foreach(var dataMember in dataMemberList)
                        {
                            if (dataMembers.ContainsKey(dataMember.Key) == false)
                            {
                                dataMembers.Add(dataMember);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    DerivedDataMembers.Add(derivedType, dataMembers);
                }
            }
        }

        public BaseDTO()
        {
            var type = this.GetType();
            if (type.IsGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }
            SerializableProperties = DerivedDataMembers[type].ToList();
        }

        public void SetSerializableProperties(string[] fields, bool exclude = false)
        {
            // get all properties that are declared as DataMember
            var dataMembers = DerivedDataMembers[this.GetType()];

            if (exclude == true)
            {
                // if in exclude mode get all properties that should be serialized by default
                SerializableProperties = dataMembers.ToList();
            }
            else
            {
                // else start with a blank list
                SerializableProperties = new List<KeyValuePair<string, PropertyInfo>>();
            }

            if (fields != null && fields.Count() > 0)
            {
                // if some fields where provided group by first field
                // this will make sure that if multiple children are selected on the same field the first field value will still be unique
                var fieldGrouping = fields
                    .Select(x => x.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries))
                    .Where(x => x.Count() > 0)
                    .GroupBy(x => x[0], x => string.Join(".", x.Skip(1)));
                var type = this.GetType();

                foreach(var field in fieldGrouping)
                {
                    // check for each field and see if it is a valid DataMember
                    var key = field.Key;

                    if (dataMembers.ContainsKey(key))
                    {
                        var property = new KeyValuePair<string, PropertyInfo>(key, dataMembers[key]);
                        // if field is a valid DataMember check if object is of type BaseDTO too and set the given child properties
                        var obj = property.Value.GetValue(this);
                        if (obj is IEnumerable enumerable )
                        {
                            var array = enumerable.OfType<object>();
                            foreach(var arrObj in array)
                            {
                                if (arrObj is BaseDTO dto)
                                {
                                    dto.SetSerializableProperties(field.ToArray(), exclude);
                                }
                            }
                        }
                        else if (obj is BaseDTO dto)
                        {
                            dto.SetSerializableProperties(field.ToArray(), exclude);
                        }

                        if (exclude)
                        {
                            // if in exclude mode check if only this field is specifically excluded (e.g. it has no children specified)
                            if (field.Any(x => string.IsNullOrEmpty(x)))
                            {
                                SerializableProperties.Remove(property);
                            }
                        }
                        else
                        {
                            SerializableProperties.Add(property);
                        }
                    }
                }
                return;
            }
        }
    }
}
