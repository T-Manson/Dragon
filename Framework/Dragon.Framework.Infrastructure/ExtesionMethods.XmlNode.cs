using System;
using System.Xml;

namespace Dragon.Framework.Infrastructure
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    static partial class ExtensionMethods
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">node 不是元素节点或属性节点</exception>
        public static string GetXPath(this XmlNode node, string separator = "/")
        {
            if (node.NodeType != XmlNodeType.Element && node.NodeType != XmlNodeType.Attribute)
            {
                throw new ArgumentException("the type of node is not correct.");
            }
            var nodeName = node.Name;
            if (node.NodeType == XmlNodeType.Attribute)
            {
                nodeName = $"@{nodeName}";
            }
            while (node.ParentNode != null)
            {
                nodeName = String.Format("{0}{1}{0}{2}", separator.IfNullOrEmpty("/"), node.Name, node.ParentNode.Name);
                node = node.ParentNode;
            }
            return nodeName;
        }

        public static string GetAttribute(this XmlNode node, string attributeName)
        {
            return GetAttribute(node, attributeName, null);
        }

        public static string GetAttribute(this XmlNode node, string attributeName, string defaultValue)
        {
            if (node.Attributes == null) return null;
            var attribute = node.Attributes[attributeName];
            return attribute?.InnerText ?? defaultValue;

        }

        public static void SetAttribute(this XmlNode node, string name, string value)
        {
            if (node?.Attributes == null) return;
            var attribute = node.Attributes[name, node.NamespaceURI];
            if (attribute == null)
            {
                if (node.OwnerDocument != null)
                    attribute = node.OwnerDocument.CreateAttribute(name, node.OwnerDocument.NamespaceURI);
                node.Attributes.Append(attribute ?? throw new InvalidOperationException());
            }
            attribute.InnerText = value;
        }
    }
}
