﻿/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OTAPI
{
    public static class Modifier
    {
        private static IEnumerable<ModificationAttribute> Discover(IEnumerable<Assembly> assemblies)
        {
            foreach (var asm in assemblies)
            {
                Type[] types;
                try
                {
                    types = asm.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types;
                }

                var modificationTypes = types.Where(x => x != null); // && !x.IsAbstract);

                foreach (var type in modificationTypes)
                {
                    var modificationAttr = type.GetCustomAttribute<ModificationAttribute>();
                    if (modificationAttr != null)
                    {
                        //modificationAttr.InstanceType = type;
                        modificationAttr.MethodBase = type.GetConstructors().Single();
                        yield return modificationAttr;
                    }

                    var methods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
                    foreach (var method in methods)
                    {
                        modificationAttr = method.GetCustomAttribute<ModificationAttribute>();
                        if (modificationAttr != null)
                        {
                            modificationAttr.MethodBase = method;
                            yield return modificationAttr;
                        }
                    }
                }
            }
        }

        static void IterateMods(IEnumerable<ModificationAttribute> mods, Action<ModificationAttribute> action)
        {
            var queue = mods.ToDictionary(i => i, k => false);
            bool complete = queue.Count == 0;

            Func<ModificationAttribute, bool> areDepsCompleted = (mod) =>
            {
                if (mod.Dependencies != null)
                {
                    return mod.Dependencies.All(d => mods.Any(m => m.MethodBase.DeclaringType == d && queue[m]));
                }
                return true;
            };

            do
            {
                foreach (var pair in queue
                    .Where(x => !x.Value)
                    .ToDictionary(k => k.Key, v => v.Value)
                )
                {
                    var is_ready = areDepsCompleted(pair.Key);
                    if (is_ready)
                    {
                        action(pair.Key);
                        queue[pair.Key] = true;
                    }
                    else Console.WriteLine($"[OTAPI] Awaiting dependencies for {pair.Key.MethodBase.DeclaringType.FullName}");
                }

                complete = queue.All(x => x.Value);
            } while (!complete);
        }

        public static void Apply(ModType modType, MonoMod.MonoModder modder = null, IEnumerable<Assembly> assemblies = null)
        {
            Console.WriteLine($"[OTAPI:{modType}] Applying mods...");
            var availableParameters = new List<object>()
            {
                modType,
            };

            if (modder != null) availableParameters.Add(modder);

            var modifications = Discover(assemblies ?? OTAPI.Plugins.PluginLoader.Assemblies).Where(x => x.Type == modType);
            IterateMods(modifications, (modification) =>
            {
                Console.WriteLine($"[OTAPI:{modType}] {modification.Description}");

                MethodBase modCtor = modification.MethodBase; //.DeclaringType.GetConstructors().Single();
                var modCtorParams = modCtor.GetParameters();

                // bind arguments
                var args = new object[modCtorParams.Count()];
                {
                    for (var i = 0; i < modCtorParams.Count(); i++)
                    {
                        var param = modCtorParams.ElementAt(i);
                        var paramValue = availableParameters.SingleOrDefault(p =>
                            param.ParameterType.IsAssignableFrom(p.GetType())
                        );
                        if (paramValue != null)
                        {
                            args[i] = paramValue;
                        }
                        else throw new Exception($"No valid for parameter ${param.Name} in modification {modification.MethodBase.DeclaringType.FullName}");
                    }
                }

                if (modification.MethodBase.IsConstructor)
                    Activator.CreateInstance(modification.MethodBase.DeclaringType, args, null);
                else modification.MethodBase.Invoke(modification.Instance, args);
            });
        }
    }
}
