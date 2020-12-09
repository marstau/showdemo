using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Reflection;
public class GameUtil {
    public static T DeepCopyByBin<T>(T obj) {
        object retval;
        using (MemoryStream ms = new MemoryStream())
        {
            BinaryFormatter bf = new BinaryFormatter();
            //序列化成流
            bf.Serialize(ms, obj);
            ms.Seek(0, SeekOrigin.Begin);
            //反序列化成对象
            retval = bf.Deserialize(ms);
            ms.Close();
        }
        return (T)retval;
    }
    /// <summary>
    /// 深拷贝，通过反射实现
    /// </summary>
    /// <typeparam name="T">深拷贝的类类型</typeparam>
    /// <param name="obj">深拷贝的类对象</param>
    /// <returns></returns>
    public static T CloneModel<T>(T obj)
    {
        //如果是字符串或值类型则直接返回
        if (obj is string || obj.GetType().IsValueType) return obj;

        object retval = Activator.CreateInstance(obj.GetType());
        FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        foreach (FieldInfo field in fields)
        {
            try { field.SetValue(retval, CloneModel(field.GetValue(obj))); }
            catch { }
        }
        return (T)retval;
    }


}
