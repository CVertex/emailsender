using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace EmailSender.Console
{
    public class SenderType
    {
        private readonly Type _type;
        private readonly ConstructorInfo[] _ctors;
        private readonly PropertyInfo[] _props;

        public string Name
        {
            get
            {
                return _type.Name;
            }
        }

        public SenderType(Type senderType)
        {
            _type = senderType;
            _ctors = _type.GetConstructors();
            _props = _type.GetProperties();
        }

        #region Helpers

        IEnumerable<ConstructorInfo> GetCtors()
        {
            var cs = _ctors.OrderByDescending(ct => ct.GetParameters().Length);
            foreach (var c in cs)
            {
                yield return c;
            }
        }

        IEnumerable<PropertyInfo> GetSettableProperties() {
            return _props.Where(p => p.CanWrite);
        }

        object Convert(Type targetType, string value)
        {
            var converter = TypeDescriptor.GetConverter(targetType);
            if (!converter.CanConvertFrom(typeof(string)))
            {
                throw new NotSupportedException();
            }
            return converter.ConvertFromString(value);
        }

        #endregion



        public IEmailSender Instantiate(IDictionary<string,string> arguments)
        {

            IEnumerable<ConstructorInfo> ctors = GetCtors();

            
            // look for a best matching ctor
            foreach (var ctor in ctors)
            {
                var ps = ctor.GetParameters();
                bool fail = false;

                // look for a parameter that can't be matched
                foreach (var p in ps)
                {


                    if (!arguments.ContainsKey(p.Name) && !p.IsOptional)
                    {
                        fail = true; 
                    }
                }

                if (!fail)
                {
                    object[] actualParameters = new object[ps.Length];
                    
                    // foreach ctor param, convert to the local type and add to the actual param list
                    for (int i=0; i<ps.Length; i++)
                    {
                        var p = ps[i];
                        if (!arguments.ContainsKey(p.Name))
                        {
                            actualParameters[i] = null; 
                        } else
                        {
                            object value = Convert(p.ParameterType,arguments[p.Name]);
                            actualParameters[i] = value;
                        }
                        
                    }

                    // Invoke the ctor to instantiate the concrete type
                    IEmailSender senderInstance = ctor.Invoke(actualParameters) as IEmailSender;
                    if (senderInstance != null)
                    {
                        SetProperties(senderInstance,arguments);
                        return senderInstance;
                    }


                }
                
            }

            throw new SenderCreationException("No usable constructor was matched with the given required parameters");
            
        }

        void SetProperties(object senderInstance, IDictionary<string,string> args)
        {
            
            foreach (var p in GetSettableProperties())
            {
                if (args.ContainsKey(p.Name))
                {
                    try
                    {
                        p.SetValue(senderInstance, Convert(p.PropertyType, args[p.Name]), null);
                    } catch (NotSupportedException)
                    {
                        // avoid this property
                    }
                }
            }

        }



        public SenderTypeUsageInfo[] GetUsageInfos()
        {
            var usageInfos = new List<SenderTypeUsageInfo>();
            

            foreach (var ctor in _ctors)
            {
                var info = new SenderTypeUsageInfo
                               {
                                   Name = Name
                               };
                var parameters = ctor.GetParameters();

                info.RequiredParameters = (from p in parameters
                                           select p.Name).ToArray();
                info.OptionalParameters = (from p in _props
                                           select p.Name).ToArray();

                usageInfos.Add(info);
            }

            return usageInfos.ToArray();
        }
        

    }
}