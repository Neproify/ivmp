/***********************************************************************
	This file is a part of scocl project by Alexander Blade (c) 2011 
***********************************************************************/

#include <natives.h>
#include <common.h>
#include <strings.h>
#include <types.h>
#include <consts.h>

//#include <globals.h>




void CreateScript(char * name)
{
	REQUEST_SCRIPT(name);
	while ( !HAS_SCRIPT_LOADED(name) ) WAIT(0);
	uint script = START_NEW_SCRIPT(name, 1024);
	MARK_SCRIPT_AS_NO_LONGER_NEEDED(name);	
}


void InitThisScript(void)
{
	int randint;
	GENERATE_RANDOM_INT(&randint);
	SET_RANDOM_SEED(randint);

	THIS_SCRIPT_SHOULD_BE_SAVED();
	SET_THIS_SCRIPT_CAN_REMOVE_BLIPS_CREATED_BY_ANY_SCRIPT(TRUE);
}


void InitGlobals(void)
{
	
}


void InitPlayer(void)
{
	SET_PLAYER_CONTROL(GetPlayerIndex(), TRUE);
	SET_CHAR_RELATIONSHIP_GROUP(GetPlayerPed(), 0);
}

void HideLoadingScreen(int time)
{
	FORCE_LOADING_SCREEN(FALSE);
	DO_SCREEN_FADE_IN_UNHACKED(time);
}

void InitVisual(void)
{
	RELEASE_WEATHER();
	RELEASE_TIME_OF_DAY();
}

void main(void)
{
	InitThisScript();
	InitGlobals();
	InitPlayer();
	InitVisual();
	
	// script population natives affects all pop config
	SPECIFY_SCRIPT_POPULATION_ZONE_NUM_PEDS(100); 
	SPECIFY_SCRIPT_POPULATION_ZONE_NUM_SCENARIO_PEDS(100);
	ACTIVATE_SCRIPT_POPULATION_ZONE();

	WAIT(2000);
	HideLoadingScreen(4000);

	while (TRUE) 
	{
		WAIT(0);
	}
}
