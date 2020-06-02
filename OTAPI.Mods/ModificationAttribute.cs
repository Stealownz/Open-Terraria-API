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

namespace OTAPI
{
    /// <summary>
    /// Defines when the modification is applied to the target assembly
    /// </summary>
    public enum ModificationType
    {
        Patchtime = 1,
        Runtime = 2
    }

    /// <summary>
    /// Defines the priority of the modification opposed to every other mod loaded
    /// </summary>
    public enum ModificationPriority
    {
        Early = -100,
        Normal = 0,
        Late = 100
    }

    /// <summary>
    /// Describes an OTAPI modification instance
    /// </summary>
    public class ModificationAttribute : Attribute
    {
        /// <summary>
        /// Description of what the mod is doing
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// What type of modifiction
        /// </summary>
        public ModificationType Type { get; set; }

        /// <summary>
        /// When the modification should run
        /// </summary>
        public ModificationPriority Priority { get; set; }

        /// <summary>
        /// What this modification needs to wait for in order to be executed
        /// </summary>
        public Type[] Dependencies { get; set; }

        public ModificationAttribute(ModificationType type, string description,
            ModificationPriority priority = ModificationPriority.Normal,
            Type[] dependencies = null
        )
        {
            this.Description = description;
            this.Type = type;
            this.Priority = priority;
            this.Dependencies = dependencies;
        }
    }
}