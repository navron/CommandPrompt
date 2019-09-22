using System;

namespace CommandPrompt.Extension.AutoFac
{
    public class PromptConfigurationWithAutoFac : PromptConfiguration, IPromptConfiguration
    {
        /// <summary>
        /// Gets an Object that may be passed to an CommandText Prompt method or class
        /// Uses AutoFac configuration for creating  
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public new object GetObjectOfType(Type type)
        {

            // BIG TODO ITEM,  Need to learn how to use AutoFac
            return null;
        }
    }
}
