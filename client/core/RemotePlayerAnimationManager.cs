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
        public bool bInitialized = false;

        AnimationSet walkAnimations;
        RemotePlayer player;
        Shared.RemotePlayerAnimations currentAnimation;
        Vector3 lastRunToCord;
        Vector3 lastWalkToCord;

        public RemotePlayerAnimationManager(RemotePlayer player)
        {
            this.player = player;
            walkAnimations = new AnimationSet("move_m@casual");
            currentAnimation = (int)Shared.RemotePlayerAnimations.StandStill;
            lastRunToCord = Vector3.Zero;
            lastWalkToCord = Vector3.Zero;
            bInitialized = true;
        }

        public void PlayAnimation(Shared.RemotePlayerAnimations animation)
        {
            if(!player.ped.Exists() || !bInitialized)
            {
                return;
            }
            if(animation != currentAnimation)
            {
                currentAnimation = animation;
                if(animation == Shared.RemotePlayerAnimations.RunTo && player.ped.Position.DistanceTo(player.GetPosition()) > 1.0f)
                {
                    if (lastRunToCord.DistanceTo(player.GetPosition()) > 1.0f)
                    {
                        Vector3 position = player.GetPosition();
                        player.ped.Task.RunTo(position);
                        lastRunToCord = position;
                    }
                    return;
                }
                if (animation == Shared.RemotePlayerAnimations.WalkTo && player.ped.Position.DistanceTo(player.GetPosition()) > 1.0f)
                {
                    if (lastWalkToCord.DistanceTo(player.GetPosition()) > 1.0f)
                    {
                        Vector3 position = player.GetPosition();
                        player.ped.Task.GoTo(position);
                        lastWalkToCord = position;
                    }
                    return;
                }

                player.ped.Task.AlwaysKeepTask = true;
                switch (animation)
                {
                    case Shared.RemotePlayerAnimations.Run:
                        {
                            player.ped.Animation.Play(walkAnimations, "sprint", 1.0f, AnimationFlags.Unknown01 | AnimationFlags.Unknown05);
                            break;
                        }
                    case Shared.RemotePlayerAnimations.Walk:
                        {
                            player.ped.Animation.Play(walkAnimations, "walk", 1.0f, AnimationFlags.Unknown01 | AnimationFlags.Unknown05);
                            break;
                        }
                    case Shared.RemotePlayerAnimations.StandStill:
                        {
                            player.ped.Animation.Play(walkAnimations, "idle", 1.0f, AnimationFlags.Unknown01 | AnimationFlags.Unknown05);
                            break;
                        }
                    case Shared.RemotePlayerAnimations.Jump:
                        {
                            GTA.Native.Function.Call("TASK_JUMP", player.ped, 1);
                            break;
                        }
                }
            }
        }
    }
}
