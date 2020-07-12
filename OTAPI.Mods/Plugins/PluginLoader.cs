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
using System.IO;
using System.Reflection;

namespace OTAPI.Plugins
{
    public static class PluginLoader
    {
        private static List<Assembly> _assemblies;

        public static IEnumerable<Assembly> Assemblies => _assemblies;

        public static IAssemblyLoader AssemblyLoader { get; set; } = new DefaultAssemblyLoader();

        public static void AddAssembly(Assembly assembly)
        {
            _assemblies.Add(assembly);
        }

        public static bool TryLoad()
        {
            if (_assemblies == null)
            {
                _assemblies = new List<Assembly>();
                _assemblies.Add(Assembly.GetExecutingAssembly());

                if (Directory.Exists("modifications"))
                {
                    foreach (var file in Directory.EnumerateFiles("modifications", "*.dll", SearchOption.AllDirectories))
                    {
                        try
                        {
                            Console.WriteLine($"[OTAPI:Startup] Loading {file}");
                            var asm = AssemblyLoader.Load(file);
                            _assemblies.Add(asm);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[OTAPI:Startup] Load failed {ex}");
                        }
                    }
                }
                return true;
            }
            return false;
        }
    }
}
