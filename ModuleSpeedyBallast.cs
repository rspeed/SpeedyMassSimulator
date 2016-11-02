//
// ModuleSpeedyBallast.cs
//
// Author:
//       Rob Speed <speed.rob@gmail.com>
//
// Copyright (c) 2016 Rob Speed
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEngine;


namespace SpeedyRockets {
	/// <summary>
	/// This is heavy, Doc
	/// </summary>
	public class ModuleSpeedyBallast : PartModule, IPartMassModifier {
		[KSPField]
		public float maximumMass = 20f;

		[KSPField]
		public float stepIncrement = 1f;

		[UI_FloatRange(scene = UI_Scene.Editor)]
		[KSPField(guiName = "Mass", guiFormat = "F2", guiUnits = "t", isPersistant = true, guiActive = true, guiActiveEditor = true)]
		public float partMass = 0f;

		// Our mass changes when we damn-well feel like it!
		public ModifierChangeWhen GetModuleMassChangeWhen() {
			return ModifierChangeWhen.FIXED;
		}


		public override void OnStart (PartModule.StartState state) {
			// Actions only for the editor
			if (state == PartModule.StartState.Editor) {
				// Set the minimum and maximum amount of ballast
				UI_FloatRange slider = this.Fields["partMass"].uiControlEditor as UI_FloatRange;
				slider.minValue = this.part.prefabMass;
				slider.maxValue = this.maximumMass;
				slider.stepIncrement = this.stepIncrement;

				// Make sure the part's mass is within the allowed range
				this.partMass = Mathf.Min(Mathf.Max(this.partMass, slider.minValue), slider.maxValue);
			}
		}


		// Just say what ya weigh
		public float GetModuleMass(float defaultMass, ModifierStagingSituation situation) {
			// Return just the mass of the ballast
			return this.partMass - defaultMass;
		}


		// Simple utility method for debugging output
		protected void log (string message, bool warn = false) {
			message = "[ModuleSpeedyBallast] " + message;

			if (warn)
				Debug.LogWarning(message);
			else
				Debug.Log(message);
		}
	}
}
