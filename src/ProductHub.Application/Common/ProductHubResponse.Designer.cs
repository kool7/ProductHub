﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProductHub.Application.Common {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ProductHubResponse {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ProductHubResponse() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ProductHub.Application.Common.ProductHubResponse", typeof(ProductHubResponse).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid page number {0}. Page number must be greater than 0..
        /// </summary>
        public static string InvalidPageNumber {
            get {
                return ResourceManager.GetString("InvalidPageNumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Page size can not be {0}. Page size must be greater than 0..
        /// </summary>
        public static string InvalidPageSize {
            get {
                return ResourceManager.GetString("InvalidPageSize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Product data is missing..
        /// </summary>
        public static string InvalidProductData {
            get {
                return ResourceManager.GetString("InvalidProductData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid product Id..
        /// </summary>
        public static string InvalidProductId {
            get {
                return ResourceManager.GetString("InvalidProductId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid sort parameter {0}. It must be &apos;asc&apos; or &apos;desc&apos;..
        /// </summary>
        public static string InvalidSortParameter {
            get {
                return ResourceManager.GetString("InvalidSortParameter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Price must be greater than 0..
        /// </summary>
        public static string PriceValidation {
            get {
                return ResourceManager.GetString("PriceValidation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A product with the name &apos;{0}&apos; already exists..
        /// </summary>
        public static string ProductAlreadyExists {
            get {
                return ResourceManager.GetString("ProductAlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Product Description is required..
        /// </summary>
        public static string ProductDescriptionValidation {
            get {
                return ResourceManager.GetString("ProductDescriptionValidation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Product name is required..
        /// </summary>
        public static string ProductNameValidation {
            get {
                return ResourceManager.GetString("ProductNameValidation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Product does not exists..
        /// </summary>
        public static string ProductNotFound {
            get {
                return ResourceManager.GetString("ProductNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Units must be greater than or equal to 0..
        /// </summary>
        public static string UnitsValidation {
            get {
                return ResourceManager.GetString("UnitsValidation", resourceCulture);
            }
        }
    }
}
