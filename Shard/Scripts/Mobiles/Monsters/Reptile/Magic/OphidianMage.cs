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

/* Scripts/Mobiles/Monsters/Reptile/Magic/OphidianMage.cs
 * ChangeLog
 *	7/26/05, erlein
 *		Automated removal of AoS resistance related function calls. 6 lines removed.
 *	7/6/04, Adam
 *		1. implement Jade's new Category Based Drop requirements
 *  6/5/04, Pix
 *		Merged in 1.0RC0 code.
 */

using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName("an ophidian corpse")]
	[TypeAlias("Server.Mobiles.OphidianShaman")]
	public class OphidianMage : BaseCreature
	{
		private static string[] m_Names = new string[]
			{
				"an ophidian apprentice mage",
				"an ophidian shaman"
			};

		[Constructable]
		public OphidianMage()
			: base(AIType.AI_Mage, FightMode.All | FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Name = m_Names[Utility.Random(m_Names.Length)];
			Body = 85;
			BaseSoundID = 639;

			SetStr(181, 205);
			SetDex(191, 215);
			SetInt(96, 120);

			SetHits(109, 123);

			SetDamage(5, 10);

			SetSkill(SkillName.EvalInt, 85.1, 100.0);
			SetSkill(SkillName.Magery, 85.1, 100.0);
			SetSkill(SkillName.MagicResist, 75.0, 97.5);
			SetSkill(SkillName.Tactics, 65.0, 87.5);
			SetSkill(SkillName.Wrestling, 20.2, 60.0);

			Fame = 4000;
			Karma = -4000;

			VirtualArmor = 30;
		}

		public override int Meat { get { return 1; } }
		public override int TreasureMapLevel { get { return Core.UOAI || Core.UOAR ? 2 : 0; } }

		public OphidianMage(Serial serial)
			: base(serial)
		{
		}

		public override void GenerateLoot()
		{
			if (Core.UOAI || Core.UOAR)
			{
				PackGold(60, 90);
				PackScroll(1, 3);
				PackScroll(1, 6);
				PackReg(10);
				// Category 2 MID
				PackMagicItem(1, 1, 0.05);
			}
			else
			{
				if (Core.UOSP || Core.UOMO)
				{	// http://web.archive.org/web/20020220114118/uo.stratics.com/hunters/ophappmage.shtml
					// http://web.archive.org/web/20020414130837/uo.stratics.com/hunters/ophshaman.shtml
					// (same creature)
					// 50 to 150 Gold, Potions, Arrows, Scrolls (circles 1 to 7), Reagents

					if (Spawning)
					{
						PackGold(50, 150);
					}
					else
					{
						PackPotion(0.9);
						PackPotion(0.6);
						PackPotion(0.2);
						PackItem(new Arrow(Utility.Random(1, 4)));
						PackScroll(1, 7);
						PackScroll(1, 7);
						PackScroll(1, 7, 0.3);
						PackReg(10);
					}
				}
				else
				{	// Standard RunUO
					if (Spawning)
						PackReg(10);

					AddLoot(LootPack.Average);
					AddLoot(LootPack.LowScrolls);
					AddLoot(LootPack.MedScrolls);
					AddLoot(LootPack.Potions);
				}
			}
		}

		public override OppositionGroup OppositionGroup
		{
			get { return OppositionGroup.TerathansAndOphidians; }
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
