using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Imagicut")]
[assembly: AssemblyDescription("Imagicut")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("xo group/ GZSDC Co Ltd.")]
[assembly: AssemblyProduct("Imagicut")]
[assembly: AssemblyCopyright("Copyright (c) 2013")]
[assembly: AssemblyTrademark("All rights reserved")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("ef212598-b9c0-4af0-a553-d39f8820f359")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace Imagicut
{
    public static class AssemblyInfo
    {
        private static Assembly assembly = Assembly.GetExecutingAssembly();
        private static string fileName = System.IO.Path.GetFileNameWithoutExtension(assembly.CodeBase);
        private static string title = null;
        private static string company = null;
        private static string product = null;
        private static string copyright = null;
        private static string trademark = null;
        private static string version = null;

        public static string WorkingDirectory
        {
            get
            {
                return Environment.CurrentDirectory;
            }
        }

        public static string Title
        {
            get
            {
                if (string.IsNullOrEmpty(title))
                {
                    object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);

                    if (attributes.Length > 0)
                    {
                        AssemblyTitleAttribute attribute = (AssemblyTitleAttribute)attributes[0];
                        if (attribute.Title.Length > 0)
                        {
                            title = attribute.Title;
                        }
                        else
                        {
                            title = fileName;
                        }
                    }
                    else
                    {
                        title = fileName;
                    }
                }

                return title;
            }
        }

        public static string Company
        {
            get
            {
                if (string.IsNullOrEmpty(company))
                {
                    object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);

                    if (attributes.Length > 0)
                    {
                        AssemblyCompanyAttribute attribute = (AssemblyCompanyAttribute)attributes[0];
                        if (attribute.Company.Length > 0)
                        {
                            company = attribute.Company;
                        }
                        else
                        {
                            company = fileName;
                        }
                    }
                    else
                    {
                        company = fileName;
                    }
                }

                return company;
            }
        }

        public static string Product
        {
            get
            {
                if (string.IsNullOrEmpty(product))
                {
                    object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);

                    if (attributes.Length > 0)
                    {
                        AssemblyProductAttribute attribute = (AssemblyProductAttribute)attributes[0];
                        if (attribute.Product.Length > 0)
                        {
                            product = attribute.Product;
                        }
                        else
                        {
                            product = fileName;
                        }
                    }
                    else
                    {
                        product = fileName;
                    }
                }

                return product;
            }
        }

        public static string Copyright
        {
            get
            {
                if (string.IsNullOrEmpty(copyright))
                {
                    object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

                    if (attributes.Length > 0)
                    {
                        AssemblyCopyrightAttribute attribute = (AssemblyCopyrightAttribute)attributes[0];
                        if (attribute.Copyright.Length > 0)
                        {
                            copyright = attribute.Copyright;
                        }
                        else
                        {
                            copyright = fileName;
                        }
                    }
                    else
                    {
                        copyright = fileName;
                    }
                }

                return copyright;
            }
        }

        public static string Trademark
        {
            get
            {
                if (string.IsNullOrEmpty(trademark))
                {
                    object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyTrademarkAttribute), false);

                    if (attributes.Length > 0)
                    {
                        AssemblyTrademarkAttribute attribute = (AssemblyTrademarkAttribute)attributes[0];
                        if (attribute.Trademark.Length > 0)
                        {
                            trademark = attribute.Trademark;
                        }
                        else
                        {
                            trademark = fileName;
                        }
                    }
                    else
                    {
                        trademark = fileName;
                    }
                }

                return trademark;
            }
        }

        public static string Version
        {
            get
            {
                if (string.IsNullOrEmpty(version))
                {
                    version = assembly.GetName().Version.ToString();
                }

                return version;
            }
        }
    }
}