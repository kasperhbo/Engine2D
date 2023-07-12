// See https://aka.ms/new-console-template for more information

#region

using Engine2D.Core;

#endregion


//Disable the engine so ui wont be rendered
//Enable the engine so ui will be rendered
Settings.s_IsEngine = true;

Engine.Get().Run();