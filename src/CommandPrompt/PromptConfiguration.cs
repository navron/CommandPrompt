
using System;
using System.Collections.Generic;

namespace CommandPrompt
{
    public interface IPromptConfiguration
    {
        string PromptPreFix { get; }
        bool ScanAssemblies { get; }
        string[] Types { get; }

        string GetOption(string option);
        object GetObjectOfType(Type type);
        object[] Objects { get; set; }
        bool HasClassInstance(Type pCmdClassType);
        object GetClassInstance(Type classType);
        void StoreClassInstance(object classInstance);
    }

    public class PromptConfiguration : IPromptConfiguration
    {
        public string PromptPreFix { get; set; } = ">";
        public string HistoryFile { get; set; } = string.Empty;

        public bool ScanAssemblies { get; }
        public string[] Types { get; }
        public string GetOption(string option)
        {
            switch (option)
            {
                case "PromptPostFix": return PromptPreFix;
                case "HistoryFile": return string.Empty;
            }

            return string.Empty;
        }

        public object GetObjectOfType(Type type)
        {
            if (type.IsInterface)
            {
                // Scan the Objects List for any  matching type and return it
                foreach (object ob in Objects)
                {
                    if(type.IsInstanceOfType(ob))
                        return ob;
                }
            }
            // Scan the Objects List for an concrete matching type and return it
            foreach (object ob in Objects)
            {
                if (ob.GetType() == type)
                    return ob;
            }


            return null; // Not Found
        }
        public object[] Objects { get; set; }


        public bool HasClassInstance(Type pCmdClassType)
        {
            throw new NotImplementedException();
        }


        public object GetClassInstance(Type classType)
        {
            foreach (object promptClass in CmdPromptClasses)
            {
                if (promptClass.GetType() == classType)
                    return promptClass;
            }

            return null; // Not Found
        }

        public void StoreClassInstance(object classInstance)
        {
            CmdPromptClasses.Add(classInstance);
        }
        internal List<object> CmdPromptClasses = new List<object>();

    }
}
