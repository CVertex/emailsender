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
        
        IEnumerable<ConstructorInfo> GetCtorsOrderedDescByParameterLength()
        {
            return _ctors.OrderByDescending(ct => ct.GetParameters().Length);
        }

        IEnumerable<PropertyInfo> GetSettableProperties() {
            return _props.Where(p => p.CanWrite);
        }

        

        #endregion



        public IEmailSender Instantiate(IDictionary<string,string> inputArgs)
        {

            IEnumerable<ConstructorInfo> ctors = GetCtorsOrderedDescByParameterLength();

            
            // look for a best matching ctor
            foreach (var ctor in ctors)
            {
                var ps = ctor.GetParameters();
                bool fail = false;

                //
                // look for a parameter that can't be matched to fail this ctor
                //
                foreach (var p in ps)
                {


                    if (!inputArgs.ContainsKey(p.Name) && !p.IsOptional)
                    {
                        fail = true; 
                    }
                }

                //
                // if the ctor passed the matchup gauntlet, create an arg list and invoke it
                //
                if (!fail)
                {
                    //
                    // foreach ctor param, convert to the param type and add to actual param object list
                    //

                    object[] parameters = new object[ps.Length];
                    
                    for (int i=0; i<ps.Length; i++)
                    {
                        var p = ps[i];
                        if (!inputArgs.ContainsKey(p.Name))
                        {
                            parameters[i] = null; 
                        } else
                        {

                            object value = inputArgs[p.Name].ToType(p.ParameterType);
                            parameters[i] = value;
                        }
                        
                    }

                    // Invoke the ctor with actual parameters to instantiate the concrete type
                    IEmailSender senderInstance = ctor.Invoke(parameters) as IEmailSender;
                    if (senderInstance != null)
                    {
                        // Property inject
                        SetProperties(senderInstance,inputArgs);
                        return senderInstance;
                    }


                }
                
            }

            throw new SenderCreationException("No usable constructor was matched with the given required parameters");
            
        }

        void SetProperties(object senderInstance, IDictionary<string,string> inputArgs)
        {
            
            foreach (var p in GetSettableProperties())
            {
                if (inputArgs.ContainsKey(p.Name))
                {
                    try
                    {
                        p.SetValue(senderInstance, inputArgs[p.Name].ToType(p.PropertyType), null);
                    } catch (NotSupportedException)
                    {
                        // if exception thrown, just ignore this property
                    }
                }
            }

        }



        public SenderTypeUsageInfo[] GetUsageInfos()
        {
            var usageInfos = new List<SenderTypeUsageInfo>();
            
            //
            // for each ctor, there's a different usage
            //
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