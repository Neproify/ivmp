/*
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE', which is part of this source code package.
 * Copyright (c) 2016 Neproify
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;

namespace ivmp_client_core
{
    class RemotePlayerAnimationManager
    {
        public bool Initialized = false;

        AnimationSet WalkAnimations;
        RemotePlayer Player;
        Shared.RemotePlayerAnimations CurrentAnimation;
        Vector3 LastRunToCord;
        Vector3 LastWalkToCord;

        public RemotePlayerAnimationManager(RemotePlayer Player)
        {
            this.Player = Player;
            WalkAnimations = new AnimationSet("move_m@casual");
            CurrentAnimation = (int)Shared.RemotePlayerAnimations.StandStill;
            LastRunToCord = Vector3.Zero;
            LastWalkToCord = Vector3.Zero;
            Initialized = true;
        }

        public void PlayAnimation(Shared.RemotePlayerAnimations Animation)
        {
            if(!Player.Ped.Exists() || !Initialized)
            {
                return;
            }
            if(Animation != CurrentAnimation)
            {
                CurrentAnimation = Animation;
                if(Animation == Shared.RemotePlayerAnimations.RunTo && Player.Ped.Position.DistanceTo(Player.GetPosition()) > 1.0f)
                {
                    if (LastRunToCord.DistanceTo(Player.GetPosition()) > 1.0f)
                    {
                        Vector3 Position = Player.GetPosition();
                        Player.Ped.Task.RunTo(Position);
                        LastRunToCord = Position;
                    }
                    return;
                }
                if (Animation == Shared.RemotePlayerAnimations.WalkTo && Player.Ped.Position.DistanceTo(Player.GetPosition()) > 1.0f)
                {
                    if (LastWalkToCord.DistanceTo(Player.GetPosition()) > 1.0f)
                    {
                        Vector3 Position = Player.GetPosition();
                        Player.Ped.Task.GoTo(Position);
                        LastWalkToCord = Position;
                    }
                    return;
                }

                Player.Ped.Task.AlwaysKeepTask = true;
                switch (Animation)
                {
                    case Shared.RemotePlayerAnimations.Run:
                        {
                            Player.Ped.Animation.Play(WalkAnimations, "sprint", 1.0f, AnimationFlags.Unknown01 | AnimationFlags.Unknown05);
                            break;
                        }
                    case Shared.RemotePlayerAnimations.Walk:
                        {
                            Player.Ped.Animation.Play(WalkAnimations, "walk", 1.0f, AnimationFlags.Unknown01 | AnimationFlags.Unknown05);
                            break;
                        }
                    case Shared.RemotePlayerAnimations.StandStill:
                        {
                            Player.Ped.Animation.Play(WalkAnimations, "idle", 1.0f, AnimationFlags.Unknown01 | AnimationFlags.Unknown05);
                            break;
                        }
                    case Shared.RemotePlayerAnimations.Jump:
                        {
                            GTA.Native.Function.Call("TASK_JUMP", Player.Ped, 1);
                            break;
                        }
                }
            }
        }
    }
}
