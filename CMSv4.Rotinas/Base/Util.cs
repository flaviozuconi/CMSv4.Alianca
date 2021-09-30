using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CMSv4.Rotinas
{
    public class Util
    {
        /// <summary>
        /// Retorna uma lista de classes que herdem o tipo informado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="constructorArgs"></param>
        /// <returns></returns>
        public static IEnumerable<T> ListarClassesPorHeranca<T>(params object[] constructorArgs) where T : class//, IComparable<T>
        {
            List<T> objects = new List<T>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));
            }
            //objects.Sort();
            return objects;
        }
    }
}
