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
        AnimationSet CrouchAnimations;
        RemotePlayer Player;
        Shared.RemotePlayerAnimations CurrentAnimation;
        Vector3 LastRunToCord;
        Vector3 LastWalkToCord;

        public RemotePlayerAnimationManager(RemotePlayer Player)
        {
            this.Player = Player;
            WalkAnimations = new AnimationSet("move_m@casual");
            CrouchAnimations = new AnimationSet("move_crouch");
            CurrentAnimation = (int)Shared.RemotePlayerAnimations.StandStill;
            LastRunToCord = Vector3.Zero;
            LastWalkToCord = Vector3.Zero;
            Initialized = true;
        }

        public void PlayAnimation(Shared.RemotePlayerAnimations Animation)
        {
            if (!Player.GameReference.Exists() || !Initialized)
            {
                return;
            }
            if (Animation != CurrentAnimation)
            {
                CurrentAnimation = Animation;
                if (Animation == Shared.RemotePlayerAnimations.RunTo && Player.GameReference.Position.DistanceTo(Player.GetPosition()) > 1.0f)
                {
                    if (LastRunToCord.DistanceTo(Player.GetPosition()) > 1.0f)
                    {
                        Vector3 Position = Player.GetPosition();
                        Player.GameReference.Task.RunTo(Position);
                        LastRunToCord = Position;
                    }
                    return;
                }
                if (Animation == Shared.RemotePlayerAnimations.WalkTo && Player.GameReference.Position.DistanceTo(Player.GetPosition()) > 1.0f)
                {
                    if (LastWalkToCord.DistanceTo(Player.GetPosition()) > 1.0f)
                    {
                        Vector3 Position = Player.GetPosition();
                        Player.GameReference.Task.GoTo(Position);
                        LastWalkToCord = Position;
                    }
                    return;
                }

                Player.GameReference.Task.AlwaysKeepTask = true;
                switch (Animation)
                {
                    case Shared.RemotePlayerAnimations.Run:
                        {
                            Player.GameReference.Animation.Play(WalkAnimations, "sprint", 1.0f, AnimationFlags.Unknown01 | AnimationFlags.Unknown05);
                            break;
                        }
                    case Shared.RemotePlayerAnimations.Walk:
                        {
                            Player.GameReference.Animation.Play(WalkAnimations, "walk", 1.0f, AnimationFlags.Unknown01 | AnimationFlags.Unknown05);
                            break;
                        }
                    case Shared.RemotePlayerAnimations.StandStill:
                        {
                            Player.GameReference.Animation.Play(WalkAnimations, "idle", 1.0f, AnimationFlags.Unknown01 | AnimationFlags.Unknown05);
                            break;
                        }
                    case Shared.RemotePlayerAnimations.Jump:
                        {
                            GTA.Native.Function.Call("TASK_JUMP", Player.GameReference, 1);
                            break;
                        }
                    case Shared.RemotePlayerAnimations.Crouch:
                        {
                            Player.GameReference.Animation.Play(CrouchAnimations, "wstart", 1.0f, AnimationFlags.Unknown01 | AnimationFlags.Unknown05);
                            break;
                        }
                }
            }
        }
    }
}
