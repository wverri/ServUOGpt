#region References
using System;

using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Spells.Ninjitsu;
#endregion

namespace Server.Items
{
    public abstract class BaseRanged : BaseMeleeWeapon
    {
        public abstract int EffectID { get; }
        public abstract Type AmmoType { get; }
        public abstract Item Ammo { get; }

        public override int DefHitSound { get { return 0x234; } }
        public override int DefMissSound { get { return 0x238; } }

        public override SkillName DefSkill { get { return SkillName.Archery; } }
        public override WeaponType DefType { get { return WeaponType.Ranged; } }
        public override WeaponAnimation DefAnimation { get { return WeaponAnimation.ShootXBow; } }

        public override SkillName AccuracySkill { get { return SkillName.Archery; } }

        private Timer m_RecoveryTimer; // so we don't start too many timers
        private int m_Velocity;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Balanced
        {
            get { return Attributes.WeaponSkillDamage > 0; }
            set
            {
                if (value)
                    Attributes.WeaponSkillDamage = 1;
                else
                    Attributes.WeaponSkillDamage = 0;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Velocity
        {
            get { return m_Velocity; }
            set
            {
                m_Velocity = value;
                InvalidateProperties();
            }
        }

        public BaseRanged(int itemID)
            : base(itemID)
        { }

        public BaseRanged(Serial serial)
            : base(serial)
        { }

        private void Atira(Mobile attacker, IDamageable damageable)
        {
            if (OnFired(attacker, damageable))
            {
                if (CheckHit(attacker, damageable))
                {
                    OnHit(attacker, damageable);
                    if (damageable is PlayerMobile)
                        attacker.PrivateOverheadMessage(MessageType.Regular, 38, true, "!", ((PlayerMobile)damageable).NetState);
                }
                else
                {
                    OnMiss(attacker, damageable);
                }
            }
        }

        public void SphereShotSwing(Mobile attacker, IDamageable damageable, double damageBonus, TimeSpan delay)
        {
            if (attacker.HitPronto)
            {
                Atira(attacker, damageable);
                return;
            }
            if (attacker.TimerAtaque != null)
            {
                return;
            }

            var frames = delay.TotalSeconds * 1.5;
            var half = delay.TotalMilliseconds * 0.6;
            var buffer = 20;
            if (attacker.RP)
            {
                SpellHelper.Turn(attacker, damageable);
                //attacker.Freeze(TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 0.8));
                Timer.DelayCall(TimeSpan.FromMilliseconds(buffer), () =>
                {
                    PlaySwingAnimation(attacker, (int)Math.Round(frames));
                });
            }
            else
            {
                PlaySwingAnimation(attacker, (int)Math.Round(frames));
            }

            attacker.TimerAtaque = Timer.DelayCall(TimeSpan.FromMilliseconds(half + buffer), () =>
            {
                if (!attacker.Alive || attacker.Deleted || attacker.TimerAtaque == null)
                {
                    attacker.HitPronto = false;
                    attacker.TimerAtaque = null;
                    OnMiss(attacker, damageable);
                    return;
                }

                if (!attacker.RP)
                {
                    attacker.HitPronto = true;
                }

                attacker.TimerAtaque = null;
                if (attacker.GetDistance(damageable) <= attacker.Weapon.MaxRange)
                {
                    Atira(attacker, damageable);
                }
            });
        }

        public override TimeSpan OnSwing(Mobile attacker, IDamageable damageable)
        {
            long nextShoot;

            if(attacker.GetDistance(damageable) <= 2)
            {
                if(!attacker.IsCooldown("dicatiro"))
                {
                    attacker.SetCooldown("dicatiro", TimeSpan.FromDays(1));
                    attacker.SendMessage(78, "[DICA] Voce esta muito proximo para atirar flechas no alvo");
                }
                return TimeSpan.FromSeconds(1);
            }

            if (attacker is PlayerMobile)
                nextShoot = ((PlayerMobile)attacker).NextMovementTime + 900 - (attacker.Dex * 5);
            else
                nextShoot = attacker.LastMoveTime + attacker.ComputeMovementSpeed();

            // Make sure we've been standing still for .25/.5/1 second depending on Era
            if (nextShoot <= Core.TickCount ||
                (WeaponAbility.GetCurrentAbility(attacker) is MovingShot))
            {
                bool canSwing = (!attacker.Paralyzed && !attacker.Frozen);

                if (canSwing)
                {
                    canSwing = AnimalForm.GetContext(attacker) == null;
                }

                if (canSwing)
                {
                    Spell sp = attacker.Spell as Spell;
                    canSwing = (sp == null || !sp.IsCasting || !sp.BlocksMovement);
                }

                var delay = GetDelay(attacker, damageable as Mobile);

                if (canSwing && attacker.HarmfulCheck(damageable))
                {
                   
                    if (attacker is BaseCreature)
                    {
                        BaseCreature bc = (BaseCreature)attacker;
                        WeaponAbility ab = bc.TryGetWeaponAbility();

                        if (ab != null)
                        {
                            if (bc.WeaponAbilityChance > Utility.RandomDouble())
                            {
                                WeaponAbility.SetCurrentAbility(bc, ab);
                            }
                            else
                            {
                                WeaponAbility.ClearCurrentAbility(bc);
                            }
                        }
                    }
                    attacker.DisruptiveAction();
                    attacker.Send(new Swing(0, attacker, damageable));

                    
                    if (Shard.COMBATE_SPHERE)
                    {
                        delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 1.25);
                        SphereShotSwing(attacker, damageable, 0, delay);
                        return delay;
                    }

                    Atira(attacker, damageable);
                }

                attacker.RevealingAction();

                return delay;
            }

            attacker.RevealingAction();

            return TimeSpan.FromSeconds(0.25);
        }

        public override void OnHit(Mobile attacker, IDamageable damageable, double damageBonus)
        {
            if (AmmoType != null && attacker.Player && damageable is Mobile && !((Mobile)damageable).Player && (((Mobile)damageable).Body.IsAnimal || ((Mobile)damageable).Body.IsMonster) &&
          0.75 >= Utility.RandomDouble())
            {
                var ammo = Ammo;
                if (ammo != null)
                {
                    ((Mobile)damageable).AddToBackpack(ammo);
                }
            } 
            base.OnHit(attacker, damageable, damageBonus);
        }

        public override void OnMiss(Mobile attacker, IDamageable damageable)
        {
            if (attacker.Player && 0.4 >= Utility.RandomDouble())
            {
                if (Core.SE)
                {
                    PlayerMobile p = attacker as PlayerMobile;

                    if (p != null && AmmoType != null)
                    {
                        Type ammo = AmmoType;

                        if (p.RecoverableAmmo.ContainsKey(ammo))
                        {
                            p.RecoverableAmmo[ammo]++;
                        }
                        else
                        {
                            p.RecoverableAmmo.Add(ammo, 1);
                        }

                        if (!p.Warmode)
                        {
                            if (m_RecoveryTimer == null)
                            {
                                m_RecoveryTimer = Timer.DelayCall(TimeSpan.FromSeconds(10), p.RecoverAmmo);
                            }

                            if (!m_RecoveryTimer.Running)
                            {
                                m_RecoveryTimer.Start();
                            }
                        }
                    }
                }
                else
                {
                    Point3D loc = damageable.Location;

                    var ammo = Ammo;

                    if (ammo != null)
                    {
                        ammo.MoveToWorld(
                            new Point3D(loc.X + Utility.RandomMinMax(-1, 1), loc.Y + Utility.RandomMinMax(-1, 1), loc.Z),
                            damageable.Map);
                    }
                }
            }

            base.OnMiss(attacker, damageable);
        }

        public virtual bool OnFired(Mobile attacker, IDamageable damageable)
        {
            WeaponAbility ability = WeaponAbility.GetCurrentAbility(attacker);

            // Respect special moves that use no ammo
            if (ability != null && ability.ConsumeAmmo == false)
            {
                return true;
            }

            if (attacker.Player)
            {
                BaseQuiver quiver = attacker.FindItemOnLayer(Layer.Cloak) as BaseQuiver;
                Container pack = attacker.Backpack;

                int lowerAmmo = AosAttributes.GetValue(attacker, AosAttribute.LowerAmmoCost);

                if (quiver == null || Utility.Random(100) >= lowerAmmo)
                {
                    // consume ammo
                    if (quiver != null && quiver.ConsumeTotal(AmmoType, 1))
                    {
                        quiver.InvalidateWeight();
                    }
                    else if (pack == null || !pack.ConsumeTotal(AmmoType, 1))
                    {
                        return false;
                    }
                }
                else if (quiver.FindItemByType(AmmoType) == null && (pack == null || pack.FindItemByType(AmmoType) == null))
                {
                    // lower ammo cost should not work when we have no ammo at all
                    return false;
                }
            }

            attacker.MovingEffect(damageable, EffectID, 18, 1, false, false);

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(4); // version

            writer.Write(m_Velocity);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 4:
                case 3:
                    {
                        if (version == 3 && reader.ReadBool())
                            Attributes.WeaponSkillDamage = 1;

                        m_Velocity = reader.ReadInt();

                        goto case 2;
                    }
                case 2:
                case 1:
                    {
                        break;
                    }
                case 0:
                    {
                        /*m_EffectID =*/
                        reader.ReadInt();
                        break;
                    }
            }

            if (version < 2)
            {
                WeaponAttributes.MageWeapon = 0;
                WeaponAttributes.UseBestSkill = 0;
            }
        }
    }
}
