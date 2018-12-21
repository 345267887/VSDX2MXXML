using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// $Id: mxCodecRegistry.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007, Gaudenz Alder
/// </summary>
namespace mxGraph.io
{


    using mxCollapseChange = mxGraph.model.mxGraphModel.mxCollapseChange;
    using mxGeometryChange = mxGraph.model.mxGraphModel.mxGeometryChange;
    using mxStyleChange = mxGraph.model.mxGraphModel.mxStyleChange;
    using mxValueChange = mxGraph.model.mxGraphModel.mxValueChange;
    using mxVisibleChange = mxGraph.model.mxGraphModel.mxVisibleChange;

    /// <summary>
    /// Singleton class that acts as a global registry for codecs. See
    /// <seealso cref="mxCodec"/> for an example.
    /// </summary>
    public class mxCodecRegistry
    {

        /// <summary>
        /// Maps from constructor names to codecs.
        /// </summary>
        protected internal static Dictionary<string, mxObjectCodec> codecs = new Dictionary<string, mxObjectCodec>();

        /// <summary>
        /// Maps from classnames to codecnames.
        /// </summary>
        protected internal static Dictionary<string, string> aliases = new Dictionary<string, string>();

        /// <summary>
        /// Holds the list of known packages. Packages are used to prefix short
        /// class names (eg. mxCell) in XML markup.
        /// </summary>
        protected internal static IList<string> packages = new List<string>();

        // Registers the known codecs and package names
        static mxCodecRegistry()
        {
            addPackage("com.mxgraph");
            addPackage("util");
            addPackage("model");
            addPackage("mxGraphview");
            addPackage("java.lang");
            addPackage("java.util");

            register(new mxObjectCodec(new List<object>()));
            register(new mxModelCodec());
            register(new mxCellCodec());
            register(new mxStylesheetCodec());

            register(new mxRootChangeCodec());
            register(new mxChildChangeCodec());
            register(new mxTerminalChangeCodec());
            register(new mxGenericChangeCodec(new mxValueChange(), "value"));
            register(new mxGenericChangeCodec(new mxStyleChange(), "style"));
            register(new mxGenericChangeCodec(new mxGeometryChange(), "geometry"));
            register(new mxGenericChangeCodec(new mxCollapseChange(), "collapsed"));
            register(new mxGenericChangeCodec(new mxVisibleChange(), "visible"));
        }

        /// <summary>
        /// Registers a new codec and associates the name of the template constructor
        /// in the codec with the codec object. Automatically creates an alias if the
        /// codename and the classname are not equal.
        /// </summary>
        public static mxObjectCodec register(mxObjectCodec codec)
        {
            if (codec != null)
            {
                string name = codec.Name;
                codecs[name] = codec;

                string classname = getName(codec.Template);

                if (!classname.Equals(name))
                {
                    addAlias(classname, name);
                }
            }

            return codec;
        }

        /// <summary>
        /// Adds an alias for mapping a classname to a codecname.
        /// </summary>
        public static void addAlias(string classname, string codecname)
        {
            if (aliases.ContainsKey(classname))
            {
                aliases.Add(classname, codecname);
            }
            else
            {
                aliases[classname] = codecname;
            }
        }

        /// <summary>
        /// Returns a codec that handles the given object, which can be an object
        /// instance or an XML node.
        /// </summary>
        /// <param name="name"> Java class name. </param>
        public static mxObjectCodec getCodec(string name)
        {
            string tmp = aliases.ContainsKey(name) ? aliases[name] : null;

            if (!string.ReferenceEquals(tmp, null))
            {
                name = tmp;
            }

            mxObjectCodec codec = codecs.ContainsKey(name) ? codecs[name] : null;

            // Registers a new default codec for the given name
            // if no codec has been previously defined.
            if (codec == null)
            {
                object instance = getInstanceForName(name);

                if (instance != null)
                {
                    try
                    {
                        codec = new mxObjectCodec(instance);
                        register(codec);
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }
            }

            return codec;
        }

        /// <summary>
        /// Adds the given package name to the list of known package names.
        /// </summary>
        /// <param name="packagename"> Name of the package to be added. </param>
        public static void addPackage(string packagename)
        {
            packages.Add(packagename);
        }

        /// <summary>
        /// Creates and returns a new instance for the given class name.
        /// </summary>
        /// <param name="name"> Name of the class to be instantiated. </param>
        /// <returns> Returns a new instance of the given class. </returns>
        public static object getInstanceForName(string name)
        {
            Type clazz = getClassForName(name);

            if (clazz != null)
            {
                if (clazz.IsEnum)
                {
                    // For an enum, use the first constant as the default instance
                    //return clazz.EnumConstants[0];

                    FieldInfo[] fieldinfo = clazz.GetFields(); //获取字段信息对象集合
                    return fieldinfo[0].GetRawConstantValue();
                }
                else
                {
                    try
                    {
                        return clazz.Assembly.CreateInstance(clazz.Name);
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a class that corresponds to the given name.
        /// </summary>
        /// <param name="name"> </param>
        /// <returns> Returns the class for the given name. </returns>
        public static Type getClassForName(string name)
        {
            try
            {
                return Type.GetType(name);
            }
            catch (Exception)
            {
                // ignore
            }

            for (int i = 0; i < packages.Count; i++)
            {
                try
                {
                    string s = packages[i];

                    return Type.GetType(s + "." + name);
                }
                catch (Exception)
                {
                    // ignore
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the name that identifies the codec associated
        /// with the given instance..
        /// 
        /// The I/O system uses unqualified classnames, eg. for a
        /// <code>model.mxCell</code> this returns
        /// <code>mxCell</code>.
        /// </summary>
        /// <param name="instance"> Instance whose node name should be returned. </param>
        /// <returns> Returns a string that identifies the codec. </returns>
        public static string getName(object instance)
        {
            Type type = instance.GetType();

            if (type.IsArray || type.IsSubclassOf(typeof(ICollection)) || type.IsSubclassOf(typeof(IDictionary)))
            {
                return "Array";
            }
            else
            {
                if (packages.Contains(type.Name))
                {
                    return type.Name;
                }
                else
                {
                    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                    return type.Name;
                }
            }
        }

    }

}