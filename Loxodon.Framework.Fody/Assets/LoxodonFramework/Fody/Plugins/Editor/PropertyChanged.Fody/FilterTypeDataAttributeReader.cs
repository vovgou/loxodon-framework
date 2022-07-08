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
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Loxodon.Framework.Fody.Editos
{
    public partial class PropertyChangedWeaver
    {
        private List<TypeDefinition> allClasses;
        private Func<TypeDefinition, bool> filter;
        private bool IsWeavingAll()
        {
            var attrEle = this.Config.Attribute(XName.Get("defaultWeaving"));
            if (attrEle == null)
                return true;
            return Boolean.Parse(attrEle.Value);
        }

        private Func<TypeDefinition, bool> GetFilter()
        {
            if (filter == null)
            {
                if (IsWeavingAll())
                    this.filter = t => !NamespaceFilters.Any() || NamespaceFilters.Any(filter => Regex.IsMatch(t.FullName, filter));
                else
                {
                    this.filter = t => (!NamespaceFilters.Any() || NamespaceFilters.Any(filter => Regex.IsMatch(t.FullName, filter))) && IsEnableWeaving(t);
                }
            }
            return filter;
        }

        public new void BuildTypeNodes()
        {
            // setup a filter delegate to apply the namespace filters
            Func<TypeDefinition, bool> extraFilter = GetFilter();

            allClasses = ModuleDefinition
                .GetTypes()
                .Where(x => x.IsClass && x.BaseType != null)
                .Where(extraFilter)
                .ToList();
            Nodes = new List<TypeNode>();
            NotifyNodes = new List<TypeNode>();
            TypeDefinition typeDefinition;
            while ((typeDefinition = allClasses.FirstOrDefault()) != null)
            {
                AddClass(typeDefinition);
            }

            PopulateINotifyNodes(Nodes);
            foreach (var notifyNode in NotifyNodes)
            {
                Nodes.Remove(notifyNode);
            }
            PopulateInjectedINotifyNodes(Nodes);
        }

        private void PopulateINotifyNodes(List<TypeNode> typeNodes)
        {
            foreach (var node in typeNodes)
            {
                if (HierarchyImplementsINotify(node.TypeDefinition))
                {
                    NotifyNodes.Add(node);
                    continue;
                }
                PopulateINotifyNodes(node.Nodes);
            }
        }

        private void PopulateInjectedINotifyNodes(List<TypeNode> typeNodes)
        {
            foreach (var node in typeNodes)
            {
                if (HasNotifyPropertyChangedAttribute(node.TypeDefinition))
                {
                    if (HierarchyImplementsINotify(node.TypeDefinition))
                    {
                        throw new WeavingException($"The type '{node.TypeDefinition.FullName}' already implements INotifyPropertyChanged so [AddINotifyPropertyChangedInterfaceAttribute] is redundant.");
                    }
                    if (node.TypeDefinition.GetPropertyChangedAddMethods().Any())
                    {
                        throw new WeavingException($"The type '{node.TypeDefinition.FullName}' already has a PropertyChanged event. If type has a [AddINotifyPropertyChangedInterfaceAttribute] then the PropertyChanged event can be removed.");
                    }
                    InjectINotifyPropertyChangedInterface(node.TypeDefinition);
                    NotifyNodes.Add(node);
                    continue;
                }
                PopulateInjectedINotifyNodes(node.Nodes);
            }
        }



        private TypeNode AddClass(TypeDefinition typeDefinition)
        {
            allClasses.Remove(typeDefinition);
            var typeNode = new TypeNode
            {
                TypeDefinition = typeDefinition
            };
            if (typeDefinition.BaseType.Scope.Name != ModuleDefinition.Name)
            {
                Nodes.Add(typeNode);
            }
            else
            {
                var baseType = Resolve(typeDefinition.BaseType);
                var filter = this.GetFilter();
                if (filter(baseType))
                {
                    var parentNode = FindClassNode(baseType, Nodes);
                    if (parentNode == null)
                    {
                        parentNode = AddClass(baseType);
                    }
                    parentNode.Nodes.Add(typeNode);
                }
                else
                {
                    Nodes.Add(typeNode);
                }
            }
            return typeNode;
        }

        private TypeNode FindClassNode(TypeDefinition type, IEnumerable<TypeNode> typeNode)
        {
            foreach (var node in typeNode)
            {
                if (type == node.TypeDefinition)
                {
                    return node;
                }
                var findNode = FindClassNode(type, node.Nodes);
                if (findNode != null)
                {
                    return findNode;
                }
            }
            return null;
        }

        private static bool IsEnableWeaving(TypeDefinition typeDefinition)
        {
            return HasNotifyPropertyChangedAttribute(typeDefinition, true) && !(HasDoNotNotifyAttribute(typeDefinition));
        }

        private static bool HasNotifyPropertyChangedAttribute(TypeDefinition typeDefinition, bool inherit = false)
        {
            if (inherit)
                return typeDefinition.GetAllCustomAttributes().ContainsAttribute("PropertyChanged.AddINotifyPropertyChangedInterfaceAttribute");

            return typeDefinition.CustomAttributes.ContainsAttribute("PropertyChanged.AddINotifyPropertyChangedInterfaceAttribute");
        }

        private static bool HasDoNotNotifyAttribute(TypeDefinition typeDefinition)
        {
            return typeDefinition.CustomAttributes.ContainsAttribute("PropertyChanged.DoNotNotifyAttribute");
        }
    }
}
