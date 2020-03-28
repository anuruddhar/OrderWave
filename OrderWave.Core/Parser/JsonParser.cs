#region Modification Log
/*------------------------------------------------------------------------------------------------------------------------------------------------- 
   System      -   OrderWave
   Module      -   Core

Modification History:
==================================================================================================================================================
Date              Version      		Modify by              					Description
--------------------------------------------------------------------------------------------------------------------------------------------------
31/05/2015         	  1.0           Anuruddha   					        Initial Version
--------------------------------------------------------------------------------------------------------------------------------------------------*/
#endregion

using OrderWave.Core.Attributes;
using OrderWave.Core.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OrderWave.Core.Parser {
    public class JsonParser {

        /// <summary>
        /// Method to convert an object into JSON string. This method get an generic type and return a string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="modeltoConvert"></param>
        /// <returns></returns>
        public static string GetJson<T>(T modeltoConvert) {
            string _strJsonString = string.Empty;

            try {
                _strJsonString = JsonConvert.SerializeObject(SetDefaultValuestoModels(modeltoConvert));

                return _strJsonString;
            } catch {
                return _strJsonString;
            }
        }

        /// <summary>
        /// Convert View model into DB model and convert it into Json String
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="modeltoConvert"></param>
        /// <param name="modeltype"></param>
        /// <returns></returns>
        public static string GetDBEntityJson<T>(T modeltoConvert, Type modeltype, bool iterateSubModel = true, string overrideColumnAttributeKey = "") {
            string _strJsonString = string.Empty;
            string attname = string.Empty;
            //var inst = Activator.CreateInstance(modeltype);
            //inst = modeltoConvert;
            try {
                modeltoConvert = SetDefaultValuestoModels(modeltoConvert);
                var properties = DataTableExtensions.GetPropertiesForType<T>(modeltype);

                //Get Attribute name of the model to keep it as a Json object Arrary name
                if (!string.IsNullOrEmpty(overrideColumnAttributeKey)) {
                    attname = overrideColumnAttributeKey;
                } else {
                    attname = GetAttributeName(properties);
                }

                if ( (modeltoConvert.GetType().FullName.Contains("System.Collections.Generic.List") || ( modeltoConvert.GetType().FullName.Contains("System.Collections.Generic.IList") )) 
                      && iterateSubModel) {
                    var expando = new ExpandoObject() as IDictionary<string, object>;
                    Type type = modeltoConvert.GetType().GetGenericArguments()[0];

                    var list = CreateListObject<object>(modeltoConvert, type);
                    var listResult = new List<IDictionary<string, object>>();
                    foreach (var item in list) {
                        var prop = DataTableExtensions.GetPropertiesForType<T>(item.GetType());
                        //Get the DB Entity from custom Attribute and create a object
                        listResult.Add(GenerateDBEntityFromProperty(item, prop, iterateSubModel));
                    }
                    if (!string.IsNullOrEmpty(attname)) {
                        expando.Add(attname, listResult);
                        _strJsonString = GetJson(expando);
                    } else {
                        _strJsonString = GetJson(listResult);
                    }
                } else {
                    //Get the DB Entity from custom Attribute and create a object
                    var dbmodel = GenerateDBEntityFromProperty(modeltoConvert, properties, iterateSubModel);
                    if (!string.IsNullOrEmpty(attname)) {
                        var expando = new ExpandoObject() as IDictionary<string, object>;
                        expando.Add(attname, dbmodel);
                        _strJsonString = GetJson(expando);
                    } else {
                        _strJsonString = GetJson(dbmodel);
                    }
                }
                return _strJsonString;
            } catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Generate Dictonary type based on models property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="modeltoConvert"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        private static IDictionary<string, object> GenerateDBEntityFromProperty<T>(T modeltoConvert, IList<PropertyInfo> properties,bool iterateSubModel) {
            var expando = new ExpandoObject() as IDictionary<string, object>;

            try {
                foreach (var property in properties) {
                    if ( ( property.PropertyType.FullName.Contains("System.Collections.Generic.List") || property.PropertyType.FullName.Contains("System.Collections.Generic.IList") )
                        && iterateSubModel) {
                        var submodel = property.GetValue(modeltoConvert);
                        if (submodel != null) {
                            Type type = submodel.GetType().GetGenericArguments()[0];
                            //Type submodeltype = submodel.GetType();

                            var list = CreateListObject(submodel, type);
                            var listResult = new List<IDictionary<string, object>>();

                            foreach (var itm in list) {
                                var subproperties = DataTableExtensions.GetPropertiesForType<T>(type);
                                listResult.Add(GenerateDBEntityFromProperty(itm, subproperties, iterateSubModel));
                            }
                            var objname = GetAttributeName(property);

                            expando.Add(( objname == string.Empty ? property.Name.ToUpper() : objname ), listResult);
                        }
                    } else if (property.PropertyType.FullName.Contains("GTS.BusinessEntity")) {

                        var submodel = property.GetValue(modeltoConvert);
                        if (submodel != null) {
                            submodel = SetDefaultValuestoModels(submodel);
                            Type submodeltype = submodel.GetType();
                            var subproperties = DataTableExtensions.GetPropertiesForType<T>(submodeltype);
                            string attname = GetAttributeName(submodel);
                            if (!string.IsNullOrEmpty(attname)) {
                                expando.Add(attname.ToUpper(), GenerateDBEntityFromProperty(submodel, subproperties, iterateSubModel));
                            } else {
                                //Recursive call for the sub models
                                if (iterateSubModel) {
                                    expando.Add(property.Name.ToUpper(), GenerateDBEntityFromProperty(submodel, subproperties, iterateSubModel));
                                }
                            }
                        }
                    }


                    var att = property.GetCustomAttributes(true);
                    ColumnAttribute p = null;

                    if (att.Length > 0) {
                        foreach (var itm in att) {
                            if (typeof(ColumnAttribute).IsEquivalentTo(itm.GetType())) {
                                p = itm as ColumnAttribute;
                                if (p != null && !string.IsNullOrEmpty(p.FieldName)) {
                                    var val = property.GetValue(modeltoConvert);

                                    if (val != null && val.GetType() == typeof(bool) && p.IsNotBoolean) {
                                        expando.Add(p.FieldName.ToUpper(), (bool)val ? "Y" : "N");
                                    } else if (val != null && val.GetType() == typeof(DateTime)) {
                                        expando.Add(p.FieldName.ToUpper(), ( (DateTime)val ).ToString("yyyy-MM-dd HH:mm:ss"));
                                    } else {
                                        expando.Add(p.FieldName.ToUpper(), val);
                                    }
                                }
                            }
                        }

                    }
                }

                return expando;

            } catch (Exception ex) {
                return expando;
            }
        }

        private static IEnumerable<T> CreateListObject<T>(T submodel, Type type) {
            var result = (IEnumerable<T>)submodel;
            return result;
        }

        /// <summary>
        /// This method not in use
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        private static string GetAttributeName<T>(T property) {
            string attributename = string.Empty;
            try {
                var att = property.GetType().GetCustomAttributes(true);
                ColumnAttribute p = null;

                if (att.Length > 0) {
                    foreach (var itm in att) {
                        if (typeof(ColumnAttribute).IsEquivalentTo(itm.GetType())) {
                            p = itm as ColumnAttribute;
                            if (p != null) {
                                attributename = p.FieldName;
                                //attributename = p.FieldName.ToUpper();
                            }
                        }
                    }

                }
                return attributename;
            } catch {
                return attributename;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonModel"></param>
        /// <returns></returns>
        public static T GetViewEntityFromJson<T>(string jsonModel) //where T : new()
        {
            //TO DO CONVERT JSON OBJECT TO MODEL
            var rootObj = JObject.Parse(jsonModel);
            return rootObj.ToObject<T>();
            //return (T) Convert.ChangeType(rootObj, typeof(T));
        }

        /// <summary>
        /// Get the value of keystring from Json string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="keyString"></param>
        /// <returns></returns>
        public static dynamic GetValues(string jsonString, string typeString, string keyString) {
            // var rootObj = JObject.Parse(jsonString);
            //var quoteObj = rootObj["quote"];
            //var quote = quoteObj.ToObject<T>();

            //var rootObj = JObject.Parse(jsonString);
            //var quoteObj = rootObj[typeString];
            //var _result = (dynamic)quoteObj[keyString];

            var rootObj = JObject.Parse(jsonString);
            var _result = rootObj[keyString];

            return _result != null ? _result : string.Empty;
        }

        public static T SetDefaultValuestoModels<T>(T model) {
            foreach (var propertyInfo in model.GetType().GetProperties()) {
                if (propertyInfo.PropertyType == typeof(string)) {
                    if (propertyInfo.GetValue(model, null) == null) {
                        propertyInfo.SetValue(model, string.Empty, null);
                    }
                }
            }
            return model;
        }

        public static string MergeJsonString(string json1, string json2) {
            var parent = JObject.Parse(json1);
            var child = JObject.Parse(json2);

            parent.Merge(child);

            return parent.ToString();
        }
    }
}
