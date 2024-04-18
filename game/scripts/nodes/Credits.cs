using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Chess.Nodes;

public partial class Credits : Control
{
	private static class NodePaths
	{
		public static readonly NodePath BackButton = new("%BackButton");
		public static readonly NodePath ENetLicense = new("%EnetLicense");
		public static readonly NodePath FreeTypeLicense = new("%FreeTypeLicense");
		public static readonly NodePath GameLicense = new("%GameLicense");
		public static readonly NodePath GodotLicense = new("%GodotLicenseText");
	}
	
	private static class CopyrightKeys
	{
		public const string Name = "name";
		public const string Parts = "parts";
		public const string Copyright = "copyright";
	}
	
	private static class LicenseTexts
	{
		public const string Game = "Copyright (c) 2024 Peter Lunneberg\n\nPermission is hereby granted, free of charge, to any person obtaining a copy\nof this software and associated documentation files (the \"Software\"), to deal\nin the Software without restriction, including without limitation the rights\nto use, copy, modify, merge, publish, distribute, sublicense, and/or sell\ncopies of the Software, and to permit persons to whom the Software is\nfurnished to do so, subject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in all\ncopies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR\nIMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,\nFITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE\nAUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER\nLIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,\nOUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE\nSOFTWARE.";
		public const string ENet = "Copyright (c) {dates} Lee Salzman\n\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.";
		public const string FreeType = "Portions of this software are copyright @ {dates} The FreeType Project (www.freetype.org). All rights reserved.";
	}
	
	private static class ModuleNames
	{
		public const string ENet = "ENet";
		public const string FreeType = "The FreeType Project";
	}
	
	[Signal]
	public delegate void BackPressedEventHandler();
	
	private const string DatesReplace = "{dates}";
	private const string RegexReplace = "$1";
	
	[GeneratedRegex(@"^(\d{4}-\d{4}).*$")]
	private static partial Regex CopyrightDatesRegex();
	
	public override void _Ready()
	{
		GetNode<Button>(NodePaths.BackButton).Pressed += () => EmitSignal(SignalName.BackPressed);
		
		GetNode<Label>(NodePaths.GameLicense).Text = LicenseTexts.Game;
		GetNode<Label>(NodePaths.GodotLicense).Text = Engine.GetLicenseText();
		
		List<string> names = [
			ModuleNames.ENet,
			ModuleNames.FreeType,
		];
		
		Engine.GetCopyrightInfo()
			.Where(dict => names.Contains((string)dict[CopyrightKeys.Name]))
			.ToList()
			.ForEach(dict => {
				var parts = (Dictionary)((Array)dict[CopyrightKeys.Parts])[0];
				var copyright = (string)((Array)parts[CopyrightKeys.Copyright])[0];
				var regex = CopyrightDatesRegex();
				
				var dates = string.Empty;
				if(regex.IsMatch(copyright))
					dates = regex.Replace(copyright, RegexReplace);
				
				switch(dict[CopyrightKeys.Name].ToString())
				{
					case ModuleNames.ENet:
						GetNode<Label>(NodePaths.ENetLicense).Text = LicenseTexts.ENet.Replace(DatesReplace, dates);
						break;
					
					case ModuleNames.FreeType:
						GetNode<Label>(NodePaths.FreeTypeLicense).Text = LicenseTexts.FreeType.Replace(DatesReplace, dates);
						break;
				}
			});
	}
}
