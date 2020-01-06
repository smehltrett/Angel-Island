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

/* ./Scripts/Mobiles/Animals/Misc/Gorilla.cs
 *	ChangeLog :
 *  8/16/06, Rhiannon
 *		Changed speed settings to match SpeedInfo table.
 *	7/26/05, erlein
 *		Automated removal of AoS resistance related function calls. 4 lines removed.
*/

using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName("a gorilla corpse")]
	public class Gorilla : BaseCreature
	{
		[Constructable]
		public Gorilla()
			: base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.25, 0.5)
		{
			Name = "a gorilla";
			Body = 0x1D;
			BaseSoundID = 0x9E;

			SetStr(53, 95);
			SetDex(36, 55);
			SetInt(36, 60);

			SetHits(38, 51);
			SetMana(0);

			SetDamage(4, 10);

			SetSkill(SkillName.MagicResist, 45.1, 60.0);
			SetSkill(SkillName.Tactics, 43.3, 58.0);
			SetSkill(SkillName.Wrestling, 43.3, 58.0);

			Fame = 450;
			Karma = 0;

			VirtualArmor = 20;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = -18.9;
		}

		public override int Meat { get { return 1; } }
		public override int Hides { get { return 6; } }
		public override FoodType FavoriteFood { get { return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public Gorilla(Serial serial)
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