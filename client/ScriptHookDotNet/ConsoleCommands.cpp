/*
* Copyright (c) 2009-2011 Hazard (hazard_x@gmx.net / twitter.com/HazardX)
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

#include "stdafx.h"

#include "ConsoleCommands.h"

#include "Blip.h"
#include "Console.h"
#include "Game.h"
#include "Ped.h"
#include "Player.h"
#include "ScriptDomain.h"
#include "Vehicle.h"
#include "World.h"
#include "mHost.h"

#pragma managed

namespace GTA {

	GTA::Console^ ConsoleCommands::Console::get() {
		return NetHook::LocalConsole;
	}
	GTA::Player^ ConsoleCommands::Player::get() {
		return GTA::Game::LocalPlayer;
	}

	bool ConsoleCommands::ProcessCommand(ConsoleEventArgs^ e) {
		String^ cmd = e->Command->ToLower();

		if (cmd == "help") { // HELP
			Console->Print("Commands:\n\
				AbortScripts       - Abort all .Net scripts. Useful if you want to play multiplayer.\n\
				Exit               - Leave the game\n\
				ReloadScripts      - Reload any .Net scripts from disk\n\
				RunningScripts     - Lists all currently running .Net scripts.\n\
				ShowPosition       - Displays the current position of the player and writes it to the log file.\n\
				StartScripts       - Start scripts again if they were aborted earlier.");
			return true;

		}
		else if (cmd == "abortscripts") { // ABORTSCRIPTS
			if (NetHook::isInsideScript) {
				Console->Print("You can't call 'AbortScripts' from inside a script!");
				return true;
			}
			NetHook::RequestScriptAction(ScriptAction::AbortScripts);
			return true;

		}
		else if (cmd == "loadedscripts") { // LOADED SCRIPTS
			Console->Print("Loaded scripts (" + NetHook::ScriptDomain->LoadedScriptCount + "):");
			for each (String^ sn in GTA::NetHook::ScriptDomain->GetLoadedScriptNames()) {
				Console->Print(" - " + sn);
			}
			return true;

		}
		else if (cmd == "runningscripts") { // RUNNING SCRIPTS
			Console->Print("Currently running scripts (" + NetHook::ScriptDomain->RunningScriptCount + "):");
			for each (String^ sn in GTA::NetHook::ScriptDomain->GetRunningScriptNames()) {
				Console->Print(" - " + sn);
			}
			return true;
		}
		else if (cmd == "reloadscripts") { // RELOAD SCRIPTS
			if (NetHook::isInsideScript) {
				Console->Print("You can't call 'ReloadScripts' from inside a script!");
				return true;
			}
			NetHook::ReloadScriptDomain();
			NetHook::RequestScriptAction(ScriptAction::ReloadAndStartScripts);
			//Console->Print(NetHook::ScriptDomain->LoadedScriptCount + " Scripts reloaded!");
			return true;

		}
		else if (cmd == "showposition") { // SHOWPOSITION
			NetHook::Log("Current Position: " + Player->Character->Position.ToString() + " Heading:" + Helper::FloatToString(Player->Character->Heading));
			return true;

		}
		else if (cmd == "startscripts") { // STARTSCRIPTS
			if (NetHook::isInsideScript) {
				Console->Print("You can't call 'StartScripts' from inside a script!");
				return true;
			}
			NetHook::RequestScriptAction(ScriptAction::StartScripts);
			return true;
		}
		return false;
	}

}