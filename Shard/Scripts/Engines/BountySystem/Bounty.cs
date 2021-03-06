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

/* Engines/BountySystem/Bounty.cs
 * CHANGELOG:
 *	5/17/10, Adam
 *		o Add a TownCrierEntryID handle to the bounty system so that we can locate/delete/update associated messages.
 *	3/1/10, Adam
 *		Set m_bLordBritishBonus=true in the Bounty command for the [macroer comand
 *	1/27/05, Pix
 *		Added optional Placer name to override Placer.Name on the bounty board.
 *	5/24/04, Pixie
 *		Put checks in Save that all the data in the bounty 
 *		is non-null.
 *	5/16/04, Pixie
 *		Initial Version
 */

using System;
using System.Xml;
using Server.Mobiles;

namespace Server.BountySystem
{
	/// <summary>
	/// Summary description for Bounty.
	/// </summary>
	public class Bounty
	{
		private PlayerMobile player_wanted;
		private PlayerMobile player_placed;
		private DateTime m_datePlaced;
		private int m_reward;
		private bool m_bLordBritishBonus;
		private string m_placername;
		private double m_TownCrierEntryID;		// any town crier message associated with this bounty

		public Bounty(PlayerMobile placer, PlayerMobile wanted, int rewardamount)
			: this(placer, wanted, rewardamount, false, null)
		{
		}

		public Bounty(PlayerMobile placer, PlayerMobile wanted, int rewardamount, bool LBBonus)
			: this(placer, wanted, rewardamount, LBBonus, null)
		{
		}

		/// <summary>
		/// Special case for [macroer command
		/// Bounty will use "PlacerName" instead of placer.Name for the bounty issuer.
		/// </summary>
		public Bounty(PlayerMobile placer, PlayerMobile wanted, int rewardamount, string PlacerName)
			: this(placer, wanted, rewardamount, true, PlacerName)
		{
		}

		public Bounty(PlayerMobile placer, PlayerMobile wanted, int rewardamount, bool LBBonus, string PlacerName)
		{
			player_placed = placer;
			player_wanted = wanted;
			m_reward = rewardamount;
			m_bLordBritishBonus = LBBonus;
			m_placername = PlacerName;

			m_datePlaced = DateTime.Now;
		}

		public Bounty(XmlElement node)
		{
			m_datePlaced = BountyKeeper.GetDateTime(BountyKeeper.GetText(node["date"], null), DateTime.Now);
			m_reward = BountyKeeper.GetInt32(BountyKeeper.GetText(node["amount"], "0"), 0);

			int serial = BountyKeeper.GetInt32(BountyKeeper.GetText(node["wanted"], "0"), 0);
			player_wanted = (PlayerMobile)World.FindMobile(serial);

			serial = BountyKeeper.GetInt32(BountyKeeper.GetText(node["placed"], "0"), 0);
			player_placed = (PlayerMobile)World.FindMobile(serial);

			string strValue = BountyKeeper.GetText(node["LBBonus"], "true");
			if (strValue.Equals("true"))
			{
				m_bLordBritishBonus = true;
			}
			else
			{
				m_bLordBritishBonus = false;
			}

			string strTest = BountyKeeper.GetText(node["placername"], null);
			if (strTest != null)
			{
				m_placername = strTest;
			}

			m_TownCrierEntryID = XmlUtility.GetDouble(XmlUtility.GetText(node["TownCrierEntryID"], "0"), m_TownCrierEntryID);
		}

		public double TownCrierEntryID
		{
			get { return m_TownCrierEntryID; }
			set { m_TownCrierEntryID = value; }
		}

		public int Reward
		{
			get { return m_reward; }
		}

		public PlayerMobile Placer
		{
			get { return player_placed; }
		}

		public PlayerMobile WantedPlayer
		{
			get { return player_wanted; }
		}

		public DateTime RewardDate
		{
			get { return m_datePlaced; }
		}

		public bool LBBonus
		{
			get { return m_bLordBritishBonus; }
		}

		public string PlacerName
		{
			get
			{
				if (m_placername != null)
				{
					return m_placername;
				}
				else
				{
					return Placer.Name;
				}
			}
		}

		public void Save(XmlTextWriter xml)
		{
			string strWanted = player_wanted.Serial.Value.ToString();
			string strPlaced = player_placed.Serial.Value.ToString();
			string strDate = XmlConvert.ToString(m_datePlaced);
			string strAmount = m_reward.ToString();

			if (strWanted == null ||
				strPlaced == null ||
				strDate == null ||
				strAmount == null)
			{
			}
			else
			{
				xml.WriteStartElement("bounty");

				xml.WriteStartElement("wanted");
				xml.WriteString(strWanted);
				xml.WriteEndElement();

				xml.WriteStartElement("placed");
				xml.WriteString(strPlaced);
				xml.WriteEndElement();

				xml.WriteStartElement("date");
				xml.WriteString(strDate);
				xml.WriteEndElement();

				xml.WriteStartElement("amount");
				xml.WriteString(strAmount);
				xml.WriteEndElement();

				xml.WriteStartElement("LBBonus");
				if (m_bLordBritishBonus)
				{
					xml.WriteString("true");
				}
				else
				{
					xml.WriteString("false");
				}
				xml.WriteEndElement();

				if (m_placername != null)
				{
					xml.WriteStartElement("placername");
					xml.WriteString(m_placername);
					xml.WriteEndElement();
				}

				if (m_TownCrierEntryID != 0.0)
				{
					xml.WriteStartElement("TownCrierEntryID");
					xml.WriteString(m_placername.ToString());
					xml.WriteEndElement();
				}

				xml.WriteEndElement();
			}
		}

	}
}
