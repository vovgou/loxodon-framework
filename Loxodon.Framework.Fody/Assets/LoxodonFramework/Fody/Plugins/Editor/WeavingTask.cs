/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using Fody;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEngine;
using FieldAttributes = Mono.Cecil.FieldAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;
using TypeSystem = Fody.TypeSystem;

namespace Loxodon.Framework.Fody.Editos
{
    public class WeavingTask : IWeavingTask
    {
        protected XElement config;
        protected string assemblyFilePath;
        protected string weaverAssemblyRoot;
        protected TestAssemblyResolver assemblyResolver;
        protected ModuleDefinition moduleDefinition;
        protected TypeCache typeCache;
        protected TypeSystem typeSystem;
        protected List<BaseModuleWeaver> weavers = new List<BaseModuleWeaver>();

        public WeavingTask(string assemblyFilePath, string weaverAssemblyRoot, XElement config)
        {
            this.assemblyFilePath = assemblyFilePath;
            this.weaverAssemblyRoot = weaverAssemblyRoot;
            this.config = config;
        }

        public void Execute()
        {
            this.assemblyResolver = new TestAssemblyResolver();
            bool hasSymbols = false;
            this.moduleDefinition = ReadModule(assemblyFilePath, out hasSymbols);
            if (HasWeavingInfo())
                return;
            InitializeWeavers();
            ExecuteWeavers();
            AddWeavingInfo();
            WriteModule(assemblyFilePath, hasSymbols);
        }

        protected virtual void InitializeWeavers()
        {
            typeCache = new TypeCache(assemblyResolver.Resolve);
            foreach (XElement element in config.Elements())
            {
                var weaverName = element.Name.LocalName;
                var assembly = FindAssembly(weaverName);
                var weaverType = FindType(assembly, "ModuleWeaver");
                BaseModuleWeaver weaver = (BaseModuleWeaver)Activator.CreateInstance(weaverType);
                InitializeWeaver(weaver, element);
                weavers.Add(weaver);
            }
            typeSystem = new TypeSystem(typeCache.FindType, this.moduleDefinition);
            weavers.ForEach(m => m.TypeSystem = typeSystem);
        }

        protected void InitializeWeaver(BaseModuleWeaver weaver, XElement config)
        {
            weaver.Config = config;
            weaver.ModuleDefinition = moduleDefinition;
            weaver.AssemblyFilePath = assemblyFilePath;
            weaver.FindType = typeCache.FindType;
            weaver.TryFindType = typeCache.TryFindType;
            weaver.ResolveAssembly = assemblyResolver.Resolve;
            weaver.AssemblyResolver = assemblyResolver;
            typeCache.BuildAssembliesToScan(weaver);
        }

        protected virtual void ExecuteWeavers()
        {
            foreach (var weaver in weavers)
            {
                weaver.Execute();
            }
        }

        protected ModuleDefinition ReadModule(string assemblyFilePath, out bool hasSymbols)
        {
            var readerParameters = new ReaderParameters
            {
                AssemblyResolver = this.assemblyResolver,
                InMemory = true
            };

            var module = ModuleDefinition.ReadModule(assemblyFilePath, readerParameters);
            hasSymbols = false;
            try
            {
                module.ReadSymbols();
                hasSymbols = true;
            }
            catch { }
            return module;
        }

        protected virtual void WriteModule(string assemblyFilePath, bool hasSymbols)
        {
            var parameters = new WriterParameters
            {
                //StrongNameKeyPair = StrongNameKeyPair,
                WriteSymbols = hasSymbols
            };

            //ModuleDefinition.Assembly.Name.PublicKey = PublicKey;
            moduleDefinition.Write(assemblyFilePath, parameters);
        }

        protected virtual bool HasWeavingInfo()
        {
            var weavingInfoClassName = GetWeavingInfoClassName();
            if (moduleDefinition.Types.Any(x => x.Name == weavingInfoClassName))
            {
                Debug.LogWarning($"The assembly has already been processed by Fody. Weaving aborted. Path: {assemblyFilePath}");
                return true;
            }
            return false;
        }

        protected virtual void AddWeavingInfo()
        {
            const TypeAttributes typeAttributes = TypeAttributes.NotPublic | TypeAttributes.Class;
            var typeDefinition = new TypeDefinition(null, GetWeavingInfoClassName(), typeAttributes, typeSystem.ObjectReference);
            moduleDefinition.Types.Add(typeDefinition);

            AddVersionField(typeof(BaseModuleWeaver).Assembly, "FodyVersion", typeDefinition);

            foreach (var weaver in weavers)
            {
                var configAssembly = weaver.GetType().Assembly;
                var name = weaver.Config.Name.LocalName;
                AddVersionField(configAssembly, name, typeDefinition);
            }
        }

        protected void AddVersionField(Assembly assembly, string name, TypeDefinition typeDefinition)
        {
            var weaverVersion = "0.0.0.0";
            var attrs = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute));
            var fileVersionAttribute = (AssemblyFileVersionAttribute)attrs.FirstOrDefault();
            if (fileVersionAttribute != null)
                weaverVersion = fileVersionAttribute.Version;

            const FieldAttributes fieldAttributes = FieldAttributes.Assembly |
                                                    FieldAttributes.Literal |
                                                    FieldAttributes.Static |
                                                    FieldAttributes.HasDefault;
            var field = new FieldDefinition(name, fieldAttributes, typeSystem.StringReference)
            {
                Constant = weaverVersion
            };

            typeDefinition.Fields.Add(field);
        }

        protected string GetWeavingInfoClassName()
        {
            var classPrefix = moduleDefinition.Assembly.Name.Name.Replace(".", "");
            return $"{classPrefix}_ProcessedByFody";
        }

        protected Assembly FindAssembly(string weaverName)
        {
            var weaverAssemblyPath = $"{weaverAssemblyRoot}{weaverName}.Fody/{weaverName}.Fody.dll";
            if (File.Exists(weaverAssemblyPath))
                return Assembly.LoadFrom(weaverAssemblyPath);

            Assembly[] listAssembly = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in listAssembly)
            {
                if (Regex.IsMatch(assembly.FullName, $"^{weaverName}.Fody"))
                    return assembly;
            }
            return null;
        }

        protected static Type FindType(Assembly assembly, string typeName)
        {
            try
            {
                return assembly
                    .GetTypes()
                    .FirstOrDefault(x => x.Name == typeName);
            }
            catch (ReflectionTypeLoadException exception)
            {
                var message = string.Format(
                    @"Could not load '{0}' from '{1}' due to ReflectionTypeLoadException.{2}", typeName, assembly.FullName, exception);
                throw new WeavingException(message);
            }
        }
    }
}