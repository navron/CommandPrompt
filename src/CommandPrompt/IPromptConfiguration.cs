﻿using System;
using System.Reflection;

namespace CommandPrompt
{
    public interface IPromptConfiguration
    {
        /// <summary>
        /// Gets an 
        /// </summary>
        /// <param name="option"></param>
        /// <value>
        /// PreFix = The string preened before the current folder name 
        /// PostFix = The string appended after the current folder name 
        /// </value>
        string GetOption(string option);

        /// <summary>
        /// Assemblies to scan for command Prompts, if not set the all user assemblies will be scanned
        /// </summary>
        Assembly[] GetScanForAssemblies { get; }

        /// <summary>
        /// Gets an Object that may be passed to an CommandText Prompt method or class
        /// </summary>
        /// <param name="type">The type of object to get</param>
        /// <returns>An instance of the type</returns>
        /// <remarks>Objects may be stored between commands, </remarks>
        object GetObjectOfType(Type type);

        /// <summary>
        /// Converts a string to an parameter type
        /// </summary>
        /// <param name="text">string to be converted</param>
        /// <param name="parameterType">The Type of the parameter to be converted to</param>
        /// <returns>The Object that will be passed to the method for the given parameter</returns>
        /// <remarks>This method can be extended to allow for custom or missing conversions</remarks>
        object ParameterConvert(string text, Type parameterType);
    }
}