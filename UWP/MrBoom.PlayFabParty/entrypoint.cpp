#include "pch.h"


using namespace Party;

extern "C" __declspec(dllexport)
void __stdcall MrBoom_Test()
{
    PartyManager& partyManager = PartyManager::GetSingleton();
    PartyError err;
}
