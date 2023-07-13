// See https://aka.ms/new-console-template for more information

#region

using Engine2D.Core;

#endregion


//Disable the engine so ui wont be rendered
//Enable the engine so ui will be rendered
Settings.s_IsEngine = true;

//Change this to the location of your project "(your repo location)\Engine2D\src\MarioLVL01\"
ProjectSettings.SetProject(@"D:\dev\Engine2D\src\MarioLVL01\", "MarioLVL01");

Engine.Get().Run();