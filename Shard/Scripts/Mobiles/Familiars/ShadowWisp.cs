/*
 *	This program is the CONFIDENTIAL and PROPRIETARY property 
 *	of Tomasello Software LLC. Any unauthorized use, reproduction or
 *	transfer of this computer program is strictly prohibited.
 *
 *      Copyright (c) 2004 Tomasello Software LLC.
 *	This is an unpublished work, and is subject to limited distribution and
 *	restricted disclosure only. ALL RIGHTS RESERVED.
 *
 *			RESTRICTED RIGHTS LEGEND
 *	Use, duplication, or disclosure by the Government is subject to
 *	restrictions set forth in subparagraph (c)(1)(ii) of the Rights in
 * 	Technical Data and Computer Software clause at DFARS 252.227-7013.
 *
 *	Angel Island UO Shard	Version 1.0
 *			Release A
 *			March 25, 2004
 */

/* Scripts/Mobiles/Familiars/ShadowWisp.cs
 * ChangeLog
 *	07/23/08, weaver
 *		Automated IPooledEnumerable optimizations. 1 loops updated.
 *	7/26/05, erlein
 *		Automated removal of AoS resistance related function calls. 6 lines removed.
 *  6/5/04, Pix
 *		Merged in 1.0RC0 code.
 */

using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName("a shadow wisp corpse")]
	public class ShadowWispFamiliar : BaseFamiliar
	{
		public ShadowWispFamiliar()
		{
			Name = "a shadow wisp";
			Body = 165;
			Hue = 0x901;
			BaseSoundID = 466;

			SetStr(50);
			SetDex(60);
			SetInt(100);

			SetHits(50);
			SetStam(60);
			SetMana(0);

			SetDamage(5, 10);

			SetSkill(SkillName.Wrestling, 40.0);
			SetSkill(SkillName.Tactics, 40.0);

			ControlSlots = 1;
		}

		private DateTime m_NextFlare;

		public override void OnThink()
		{
			base.OnThink();

			if (DateTime.Now < m_NextFlare)
				return;

			m_NextFlare = DateTime.Now + TimeSpan.FromSeconds(5.0 + (25.0 * Utility.RandomDouble()));

			this.FixedEffect(0x37C4, 1, 12, 1109, 6);
			this.PlaySound(0x1D3);

			Timer.DelayCall(TimeSpan.FromSeconds(0.5), new TimerCallback(Flare));
		}

		private void Flare()
		{
			Mobile caster = this.ControlMaster;

			if (caster == null)
				caster = this.SummonMaster;

			if (caster == null)
				return;

			ArrayList list = new ArrayList();

			IPooledEnumerable eable = this.GetMobilesInRange(5);
			foreach (Mobile m in eable)
			{
				if (m.Player && m.Alive && !m.IsDeadBondedPet && m.Karma <= 0)
					list.Add(m);
			}
			eable.Free();

			for (int i = 0; i < list.Count; ++i)
			{
				Mobile m = (Mobile)list[i];
				bool friendly = true;

				for (int j = 0; friendly && j < caster.Aggressors.Count; ++j)
					friendly = (((AggressorInfo)caster.Aggressors[j]).Attacker != m);

				for (int j = 0; friendly && j < caster.Aggressed.Count; ++j)
					friendly = (((AggressorInfo)caster.Aggressed[j]).Defender != m);

				if (friendly)
				{
					m.FixedEffect(0x37C4, 1, 12, 1109, 3); // At player
					m.Mana += 1 - (m.Karma / 1000);
				}
			}
		}

		public ShadowWispFamiliar(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}
