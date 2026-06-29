using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

public class ClassAnalyzer
{
    private Type _type;

    public ClassAnalyzer(Type type)
    {
        _type = type;
    }
    
    public IEnumerable<string> GetPublicMethods()
    {
         var res = _type.GetMethods().Select(m=>m.Name);
         return res;
    }
    public IEnumerable<string> GetMethodParams(string methodname)
    {
        var res = _type.GetMethod(methodname);
        var res_type = res.ReturnType.Name;
        var args = res.GetParameters().Select(p=>p.Name);
        return args.Prepend(res_type);
    }

    public IEnumerable<string> GetAllFields()
    {
        var fields = _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var selFields = fields.Select(f=>f.Name);
        return selFields;
    }
    
    public IEnumerable<string> GetProperties()
    {
        var prop = _type.GetProperties();
        var selProp = prop.Select(p=>p.Name);
        return selProp;
    }

    public bool HasAttribute<T>() where T : Attribute
    {
        var res = _type.IsDefined(typeof(T), false);
        return res;
    }
}