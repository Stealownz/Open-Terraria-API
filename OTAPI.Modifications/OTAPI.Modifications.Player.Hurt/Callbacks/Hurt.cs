﻿using Terraria.DataStructures;

namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Player
	{
		internal static bool HurtBegin(ref double returnValue, global::Terraria.Player player, ref PlayerDeathReason damageSource, ref int Damage, ref int hitDirection, ref bool pvp, ref bool quiet, ref bool Crit, ref int cooldownCounter)
		{
			var res = Hooks.Player.Hurt?.Invoke(ref returnValue, player, ref damageSource, ref Damage, ref hitDirection, ref pvp, ref quiet, ref Crit, ref cooldownCounter);
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}
	}
}
