﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class MajorExplosion : GenericCriticalHit
    {
        public MajorExplosion()
        {
            Name = "Major Explosion";
            Type = CriticalCardType.Ship;
            ImageUrl = "http://i.imgur.com/6aASBM9.jpg";
        }

        public override void ApplyEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Roll 1 attack die. On a Hit result, suffer 1 critical damage.");
            Game.UI.AddTestLogEntry("Roll 1 attack die. On a Hit result, suffer 1 critical damage.");
            RollForDamage(host);
        }

        private void RollForDamage(Ship.GenericShip host)
        {
            Selection.ActiveShip = host;
            Phases.StartTemporarySubPhase("Major Explosion", typeof(SubPhases.MajorExplosionCheckSubPhase));
        }

    }

}

namespace SubPhases
{

    public class MajorExplosionCheckSubPhase : DiceRollCheckSubPhase
    {

        public override void Prepare()
        {
            dicesType = "attack";
            dicesCount = 1;

            finishAction = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            if (CurrentDiceRoll.DiceList[0].Side == DiceSide.Success)
            {
                Game.UI.ShowError("Major Explosion: Suffer 1 additional critical damage");
                Game.UI.AddTestLogEntry("Major Explosion: Suffer 1 additional critical damage");

                DealDamage();
            }

            Phases.FinishSubPhase(this.GetType());
        }

        private void DealDamage()
        {
            DamageSourceEventArgs eventArgs = new DamageSourceEventArgs();
            eventArgs.Source = new CriticalHitCard.MajorExplosion();
            eventArgs.DamageType = DamageTypes.CriticalHitCard;
            Triggers.RegisterTrigger(new Trigger() { Name = "Draw faceup damage card", TriggerOwner = Selection.ActiveShip.Owner.PlayerNo, triggerType = TriggerTypes.OnDamageCardIsDealt, eventHandler = Selection.ActiveShip.DealFaceupCritCard });
            //OldTriggers.AddTrigger("Draw faceup damage card", TriggerTypes.OnDamageCardIsDealt, Selection.ActiveShip.DealFaceupCritCard, Selection.ActiveShip, Selection.ActiveShip.Owner.PlayerNo, eventArgs);
            //TODO: add callbacks
            Triggers.ResolveTriggersByType(TriggerTypes.OnDamageCardIsDealt);
            Triggers.ResolveTriggersByType(TriggerTypes.OnCritDamageCardIsDealt);
        }

    }

}