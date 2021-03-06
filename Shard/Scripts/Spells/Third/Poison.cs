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

/* Scripts\Spells\Third\Poison.cs
 * ChangeLog:
 *	5/27/10, adam
 *		Change the chance to poison from 10% chance to 18.21% per Akarius
 *	3/18/10, adam
 *		Added ability to adjust resistability via the PoisonStickMCi (console)
 *  2/12/07 Taran Kain
 *		Removed WW test and logging code.
 * 10/19/06 Taran Kain
 *		Fixed mathematical error in WW test.
 * 10/17/06 Taran Kain
 *		Added logging code and Wald-Wolfowitz test ([poisontest) to see if we do or do not have a RNG problem.
 *  7/4/04, Pix
 *		Added Damage call so that the caster will be added to the target's aggressors list.
	6/5/04, Pix
		Merged in 1.0RC0 code.
*/

using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Third
{
    public class PoisonSpell : Spell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Poison", "In Nox",
                SpellCircle.Third,
                203,
                9051,
                Reagent.Nightshade
            );

        public PoisonSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        SpellCircle SpellCircle { get { return Server.Items.Consoles.PoisonStickMCi.SpellCircle; } }

        public override double GetResistPercent(Mobile target)
        {
            return GetResistPercentForCircle(target, SpellCircle);
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile m)
        {
            if (!Caster.CanSee(m))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckHSequence(m))
            {
                SpellHelper.Turn(Caster, m);

                SpellHelper.CheckReflect((int)this.Circle, Caster, ref m);

                if (m.Spell != null)
                    m.Spell.OnCasterHurt();

                m.Paralyzed = false;

                if (CheckResisted(m))
                {
                    m.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                }
                else
                {
                    int level;

                    if (Core.AOS)
                    {
                        if (Caster.InRange(m, 2))
                        {
                            int total = (Caster.Skills.Magery.Fixed + Caster.Skills.Poisoning.Fixed) / 2;

                            if (total >= 1000)
                                level = 3;
                            else if (total > 850)
                                level = 2;
                            else if (total > 650)
                                level = 1;
                            else
                                level = 0;
                        }
                        else
                        {
                            level = 0;
                        }
                    }
                    else
                    {
                        double total = Caster.Skills[SkillName.Magery].Value + Caster.Skills[SkillName.Poisoning].Value;

                        double dist = Caster.GetDistanceToSqrt(m);

                        if (dist >= 3.0)
                            total -= (dist - 3.0) * 10.0;
                        // adam: change from 10% to 18.21% per Akarius' recomendations
                        if (total >= 200.0 && (Core.AOS || Utility.RandomChance(18.21)))
                            level = 3;
                        else if (total > (Core.AOS ? 170.1 : 170.0))
                            level = 2;
                        else if (total > (Core.AOS ? 130.1 : 130.0))
                            level = 1;
                        else
                            level = 0;
                    }

                    //Pix- 7/4/04 - this assures that the caster is added to the target's aggressors list.
                    SpellHelper.Damage(TimeSpan.FromSeconds(0.1), m, Caster, 1, 0, 0, 0, 100, 0);

                    m.ApplyPoison(Caster, Poison.GetPoison(level));
                }

                m.FixedParticles(0x374A, 10, 15, 5021, EffectLayer.Waist);
                m.PlaySound(0x474);
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private PoisonSpell m_Owner;

            public InternalTarget(PoisonSpell owner)
                : base(12, false, TargetFlags.Harmful)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                {
                    m_Owner.Target((Mobile)o);
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}