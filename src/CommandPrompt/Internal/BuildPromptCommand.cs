using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace CommandPrompt.Internal
{
    internal class BuildPromptCommand
    {
        internal static List<PromptCommand> ScanForPrompt()
        {
            //var typesWithMyAttribute =
            //    from a in AppDomain.CurrentDomain.GetAssemblies().AsParallel()
            //    from t in a.GetTypes()
            //    let attributes = t.GetCustomAttributes(typeof(PromptAttribute), true)
            //    where attributes != null && attributes.Length > 0
            //    select new PromptCommand { PromptMethodType = t, Ca = attributes.Cast<PromptAttribute>()} ;

            var list = new List<PromptCommand>();


            var temp = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.FullName.StartsWith("System")).ToList();
            var temp2 = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GlobalAssemblyCache) continue;
                if (assembly.GetName().Name.StartsWith("System")) continue;

                var ts = assembly.GetTypes();
                foreach (var c in ts)
                {
                    var m = c.GetMethods();
                    foreach (var methodInfo in m)
                    {
                        if (!(methodInfo.GetCustomAttribute(typeof(PromptAttribute), true) is PromptAttribute p))
                            continue;

                        list.Add(GetPromptInformation(c, methodInfo, p));

                    }
                }
            }

            return list;
        }

        private static PromptCommand GetPromptInformation(Type classType, MethodInfo promptMethodInfo,
            PromptAttribute promptAttribute)
        {
            //  var p = (PromptAttribute)promptAttribute;
            var command = new PromptCommand();
            command.PromptMethodType = promptAttribute.GetType();
            command.HelpText = promptAttribute.HelpText;
            command.Command = promptAttribute.Command;
            command.CommandLower = promptAttribute.Command.ToLower();
            command.PromptAttributeTypeId = promptAttribute.TypeId;
            command.MethodInfo = promptMethodInfo.Name;
            command.ClassType = classType;

            // Create a instance to get the prompt parameters 
            var parameters = classType.GetMethod(command.MethodInfo)?.GetParameters();
            command.Parameters = parameters;

            if (parameters != null && parameters.Length > 0)
            {
                command.StartWith = true;
            }

            //ParameterInfo[] parameters = methodInfo;

            // Get ClassType Interface.
            var test = command.ClassType.GetCustomAttributes(typeof(PromptClassAttribute), true).FirstOrDefault() as PromptClassAttribute;
          //  command.test = test;
          if (test != null)
          {
              command.ClassKeep = test.Keep;
              command.Folder = test.Folder;

          }


          return command;
        }

        //public static IEnumerable<Type> GetTypesWithHelpAttribute2(Assembly assembly)
        //{
        //    return assembly.GetTypes()
        //        .Where(type => type.GetCustomAttributes(typeof(PromptAttribute), true).Length > 0);
        //}

        //static IEnumerable<Type> GetTypesWithHelpAttribute(Assembly assembly)
        //{
        //    foreach (Type type in assembly.GetTypes())
        //    {
        //        if (type.GetCustomAttributes(typeof(PromptAttribute), true).Length > 0)
        //        {
        //            yield return type;
        //        }
        //    }
        //}
    }
}
